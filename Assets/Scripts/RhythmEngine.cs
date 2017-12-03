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

    // KeyCodes that are allocated by the RhythmEngine to Units.
    public List<KeyCode> allowedKeys = null;

    // KeyCode sequences that are allocated by the Rhythmengine to Units.
    public List<KeyCodeSequence> allowedSequences = null;

    // Units that are following a rhythm.
    Dictionary<Unit, Tracking> units = null;

    // KeyCodes that are already allocated to some Units.
    HashSet<KeyCode> allocatedKeys = null;

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
        allocatedKeys = new HashSet<KeyCode>();
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
            do
            {
                keyCodes = allowedSequences[Random.Range(0, allowedSequences.Count)].sequence;
            }
            while (allocatedKeys.Overlaps(keyCodes));
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
        foreach (KeyCode key in keyCodes)
        {
            allocatedKeys.Add(key);
        }
        unit.keyCodes.AddRange(keyCodes);

        // Start tracking the unit.
        units.Add(unit, new Tracking());
    }

    public void RemoveMarching(Unit unit)
    {
        foreach (KeyCode key in unit.keyCodes)
        {
            allocatedKeys.Remove(key);
        }
        units.Remove(unit);
    }

    public void Clear()
    {
        units.Clear();
        allocatedKeys.Clear();
    }

    void Update()
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
                Debug.Log("OnBeatStart: " + key);

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
                    //Debug.Log("OnOutOfBeat: " + key);
                    unit.OnOutOfBeat();
                    return;
                }

                // If this is the last key in the sequence.
                if (tracking.index == unit.keyCodes.Count - 1)
                {
                    if (tracking.pressed > 0)
                    {
                        //Debug.Log("OnBeatDouble: " + key);
                        unit.OnBeatDouble();
                    }
                    else
                    {
                        //Debug.Log("OnBeatHit: " + key);
                        unit.OnBeatHit();
                    }
                    tracking.pressed++;
                }

                tracking.index = (tracking.index + 1) % unit.keyCodes.Count;
            }
        }
    }
}
