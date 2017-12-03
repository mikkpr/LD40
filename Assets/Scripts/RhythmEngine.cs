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

    // Parameters for the current Boss.Challenge.
    Boss.Challenge challenge = null;
    List<KeyCode> challengeKeys = null;
    int challengeIndex = 0;
    bool bossWinding = false;

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
    }

    // Issue a new Boss.Challenge. First the Boss plays some notes at the given
    // offsets, then after length has passed, the units have to play the same
    // notes. The actual keys for the offsets are chosen by RhythmEngine from
    // the pool of allocated KeyCodes (even if the corresponding Units are no
    // longer marching).
    public void BossChallenge(Boss.Challenge challenge)
    {
        float time = SoundManager.instance.GetCurrentTime();

        // Only accept a new challenge if we have a Boss and we are not already
        // in one.
        if (boss == null || InChallenge(time))
        {
            Debug.Log("Rejecting new challenge, boss:" + (boss != null) +
                    ", challenge:" + (this.challenge != null));
            return;
        }
        this.challenge = challenge;
        challengeIndex = 0;
        bossWinding = false;
    }

    bool InChallenge(float time)
    {
        return challenge != null && (time >= challenge.start) &&
            (time < challenge.start + 2*challenge.length);
    }

    void BossUpdate()
    {
        float time = SoundManager.instance.GetCurrentTime();
        if (!InChallenge(time))
        {
            return;
        }
        if (time < challenge.start + challenge.length)
        {
            // Bosses turn to pump out notes, if not done already.
            if (challengeIndex < challenge.offsets.Count)
            {
                float offset = challenge.start + challenge.offsets[challengeIndex];
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
                    challengeIndex++;
                }
            }
        }
        else
        {
            // Our turn to follow what the Boss did.
            // TODO
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
