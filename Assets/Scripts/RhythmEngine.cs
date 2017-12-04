using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmEngine : MonoBehaviour
{
    // Gets the first RhythmEngine instance from the Scene tagged with
    // "RhythmEngine".
    public static RhythmEngine GetTagged()
    {
        GameObject[] tagged = GameObject.FindGameObjectsWithTag("RhythmEngine");
        if (tagged.Length < 1)
        {
            return null;
        }
        return tagged[0].GetComponent<RhythmEngine>();
    }

    // Time before a beat when Units start their windup.
    public float unitWindup = 0.667f;

    // Allowed difference from the perfect beat.
    public float singleAccuracy = 0.1f;
    public float sequenceAccuracy = 0.25f;

    // Time before a note that the Boss starts its wind up.
    public float bossWindup = 0.5f;

    // KeyCodes that are allocated by the RhythmEngine to Units.
    public List<KeyCode> allowedKeys = null;

    // KeyCode sequences that are allocated by the Rhythmengine to Units.
    public List<KeyCodeSequence> allowedSequences = null;

    // Units that are following a rhythm.
    Dictionary<Unit, Tracking> units = null;

    // KeyCodes that are already allocated to some Units. Use List instead of
    // HashSet do that we can randomly sample elements from it.
    List<KeyCode> allocatedKeys = null;

    // The Boss being fought. If null then units are marching.
    Boss boss = null;

    // Parameters for the Boss.Challenges.
    int challengeIndex = 0;             // Index of Boss.challenges.
    List<KeyCode> challengeKeys = null; // Keys allocated to a Boss.Challenge.
    int challengeOffsetIndex = 0;       // Index of Boss.Challenge.offsets.
    bool bossWinding = false;           // Has the Boss started winding up.
    Unit challengeUnit = null;          // The Unit that is allocated the next key.

    // ListWrapper is required so that the Unity Editor can serialize a List of
    // Lists to be filled.
    // https://answers.unity.com/questions/289692/serialize-nested-lists.html
    [System.Serializable]
    public class KeyCodeSequence
    {
        public List<KeyCode> sequence = null;
    }

    // Tracking is the rhythm tracking information for a Unit.
    class Tracking
    {
        // Has an onBeatStart been issued for this Unit? This is used to avoid
        // firing an onBeatEnd if no preceding onBeatStart was called.
        public bool started = false;

        // Has OnBeatWindup already been called this beat.
        public bool winding = false;

        // Is the Unit currently in the rhythm window ("beat"). Units start
        // with inBeat set to avoid firing an onBeatStart if the Unit was added
        // in the middle of a beat.
        public bool inBeat = true;

        // How many times was the KeyCode squence for this Unit pressed in the
        // current beat.
        public int pressed = 0;

        // The index of the next Keycode in the Unit's sequence that must be
        // pressed.
        public int index = 0;
    }

    void Awake()
    {
        units = new Dictionary<Unit, Tracking>();
        allocatedKeys = new List<KeyCode>();
        challengeKeys = new List<KeyCode>();
    }

    void Start()
    {
        SoundManager.instance.PlayMusic();
    }

    // AddMarching adds a new marching Unit. It will be allocated a KeyCode
    // sequence that must be pressed each unit.interval. The KeyCode sequence
    // will be added to unit.KeyCodes.
    public void AddMarching(Unit unit)
    {
        List<KeyCode> keyCodes = new List<KeyCode>();
        if (unit.sequence)
        {
            // XXX: Will loop forever if a key from all sequences is allocated.
            bool overlaps;
            do
            {
                keyCodes = allowedSequences[Random.Range(0, allowedSequences.Count)].sequence;
                overlaps = false;
                foreach (KeyCode key in keyCodes)
                {
                    if (allocatedKeys.Contains(key))
                    {
                        overlaps = true;
                    }
                }
            }
            while (overlaps);
        }
        else
        {
            // As a special case, allocate the first allowed KeyCode to the
            // first added Unit.
            if (units.Count == 0)
            {
                keyCodes.Add(allowedKeys[0]);
            }
            // Otherwise allocate a random unused KeyCode to the unit.
            else
            {
                // XXX: Will loop forever if all keys are allocated.
                KeyCode key = KeyCode.None;
                do
                {
                    key = allowedKeys[Random.Range(0, allowedKeys.Count)];
                }
                while (allocatedKeys.Contains(key));
                keyCodes.Add(key);
            }
        }
        allocatedKeys.AddRange(keyCodes);
        unit.keyCodes.AddRange(keyCodes);

        // Start tracking the unit.
        units.Add(unit, new Tracking());
    }

    public void RemoveMarching(Unit unit)
    {
        // Do not remove KeyCodes allocated to the Unit: we do not wish to
        // reissue during a single level.
        units.Remove(unit);
    }

    void MarchingUpdate()
    {
        foreach (KeyValuePair<Unit, Tracking> entry in units)
        {
            Unit unit = entry.Key;
            Tracking tracking = entry.Value;
            KeyCode key = unit.keyCodes[tracking.index];
            float accuracy = unit.sequence ? sequenceAccuracy : singleAccuracy;

            float sinceBeat = (SoundManager.instance.GetCurrentTime() - unit.offset) % unit.interval;

            if (!tracking.winding && sinceBeat >= (unit.interval - accuracy - unitWindup))
            {
                tracking.winding = true;
                unit.OnBeatWindup();
            }

            bool oldInBeat = tracking.inBeat;
            tracking.inBeat = sinceBeat < accuracy || sinceBeat > (unit.interval - accuracy);

            if (!oldInBeat && tracking.inBeat)
            {
                tracking.started = true;
                tracking.pressed = 0;
                unit.OnBeatStart();
            }
            else if (tracking.started && oldInBeat && !tracking.inBeat)
            {
                tracking.index = 0;
                tracking.winding = false;
                unit.OnBeatEnd(tracking.pressed == 1);
            }

            if (tracking.started && Input.GetKeyDown(key))
            {
                if (!tracking.inBeat)
                {
                    unit.OnOutOfBeat();
                    return;
                }

                // If this is the last key in the sequence.
                if (tracking.index == unit.keyCodes.Count - 1)
                {
                    if (tracking.pressed > 0)
                    {
                        unit.OnBeatDouble();
                    }
                    else
                    {
                        unit.OnBeatHit();
                    }
                    tracking.pressed++;
                }

                tracking.index = (tracking.index + 1) % unit.keyCodes.Count;
            }
        }
    }

    // Sets the RhythmEngine from marching mode into Boss fight mode.
    public void SetBoss(Boss boss)
    {
        this.boss = boss;
        foreach (Unit unit in units.Keys)
        {
            unit.SetBossFightMode();
        }
    }

    void BossUpdate()
    {
        if (challengeIndex >= boss.challenges.Count)
        {
            // Wait for the song to end.
            if (!SoundManager.instance.IsMusicPlaying())
            {
                boss.OnSuccess();
                Clear();
            }
            return;
        }
        Boss.Challenge challenge = boss.challenges[challengeIndex];

        float time = SoundManager.instance.GetCurrentTime();
        if (time < challenge.start)
        {
            return;
        }

        if (time < challenge.start + challenge.length)
        {
            // Bosses turn to pump out notes, if not done already.
            if (challengeOffsetIndex < challenge.offsets.Count)
            {
                float offset = challenge.start + challenge.offsets[challengeOffsetIndex];
                if (!bossWinding && time >= offset - bossWindup)
                {
                    bossWinding = true;
                    boss.OnWindup();
                }
                if (time >= offset)
                {
                    // Choose a random allocated key for the challenge.
                    KeyCode key = allocatedKeys[Random.Range(0, allocatedKeys.Count)];
                    challengeKeys.Add(key);
                    boss.OnNote(key);
                    challengeOffsetIndex++;
                }
            }
        }
        else
        {
            // Our turn to follow what the Boss did.
            if (bossWinding)
            {
                bossWinding = false;
                boss.OnTurnEnd();
                challengeOffsetIndex = 0;
                UpdateChallengeUnit();
            }
            if (challengeOffsetIndex >= challenge.offsets.Count)
            {
                // Nailed it! Set the next challenge.
                challengeIndex++;
                challengeKeys.Clear();
                challengeOffsetIndex = 0;
                return;
            }

            float offset = challenge.start + challenge.length + challenge.offsets[challengeOffsetIndex];
            if (challengeUnit == null)
            {
                // We lost this Unit during marching, auto-fail.
                if (time >= offset)
                {
                    challengeOffsetIndex++;
                    UpdateChallengeUnit();
                    boss.OnMiss();
                }
            }
            else
            {
                // This Unit has to hit the beat.
                Tracking tracking = units[challengeUnit];

                // XXX: Copy-pasta from MarchingUpdate.
                if (!tracking.winding && time >= (offset - singleAccuracy - unitWindup))
                {
                    tracking.winding = true;
                    challengeUnit.OnBeatWindup();
                }

                bool oldInBeat = tracking.inBeat;
                tracking.inBeat = time >= (offset - singleAccuracy) &&
                    time < (offset + singleAccuracy);

                if (!oldInBeat && tracking.inBeat)
                {
                    tracking.pressed = 0;
                    challengeUnit.OnBeatStart();
                }
                else if (oldInBeat && !tracking.inBeat)
                {
                    tracking.winding = false;
                    if (tracking.pressed == 1)
                    {
                        challengeUnit.OnBeatEnd(true);
                    }
                    else
                    {
                        challengeUnit.OnBeatEnd(false);
                        boss.OnMiss();
                    }
                    challengeOffsetIndex++;
                    if (challengeOffsetIndex >= challengeKeys.Count)
                    {
                        return;
                    }
                    UpdateChallengeUnit();
                }

                if (Input.GetKeyDown(challengeKeys[challengeOffsetIndex]))
                {
                    if (!tracking.inBeat)
                    {
                        challengeUnit.OnOutOfBeat();
                        boss.OnMiss();
                        return;
                    }

                    if (tracking.pressed > 0)
                    {
                        challengeUnit.OnBeatDouble();
                        boss.OnMiss();
                    }
                    else
                    {
                        challengeUnit.OnBeatHit();
                    }
                    tracking.pressed++;
                }
            }
        }
    }

    void UpdateChallengeUnit()
    {
        challengeUnit = null;
        if (challengeOffsetIndex >= challengeKeys.Count)
        {
            return;
        }

        KeyCode key = challengeKeys[challengeOffsetIndex];
        foreach (KeyValuePair<Unit, Tracking> entry in units)
        {
            if (entry.Key.keyCodes.Contains(key))
            {
                challengeUnit = entry.Key;
                entry.Value.winding = false;
                entry.Value.inBeat = false;
                entry.Value.pressed = 0;
                return;
            }
        }
    }

    void Update()
    {
        if (boss == null)
        {
            MarchingUpdate();
        }
        else
        {
            BossUpdate();
        }
    }

    public void Clear()
    {
        boss = null;
        units.Clear();
        allocatedKeys.Clear();
    }
}
