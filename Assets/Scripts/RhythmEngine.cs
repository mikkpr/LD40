using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhythmEngine : MonoBehaviour
{
    // Time when the music was started.
    public float initial = 0.0f;

    // Allowed difference from the perfect beat.
    public float accuracy = 0.25f;

    // KeyCodes that are allocated by the RhythmEngine to Units.
    public List<KeyCode> allowedKeys = null;

    // Units that are following a rhythm.
    Dictionary<Unit, Tracking> units = null;

    // KeyCodes that are already allocated to some Units.
    HashSet<KeyCode> allocatedKeys = null;

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

    // AddMarching adds a new marching Unit. It will be allocated a KeyCode
    // sequence that must be pressed each unit.interval. The KeyCode sequence
    // will be added to unit.KeyCodes.
    public void AddMarching(Unit unit)
    {
        // Allocate a random unused KeyCode to the unit.
        // TODO: KeyCode sequences.
        // XXX: Will loop forever if all keys are allocated.
        KeyCode key = KeyCode.None; 
        do
        {
            key = allowedKeys[Random.Range(0, allowedKeys.Count)];
        }
        while (allocatedKeys.Contains(key));
        allocatedKeys.Add(key);
        unit.keyCodes.Add(key);

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

            // TODO: Use SoundManager.instance.musicSource.time instead.
            float time = Time.time - initial;

            float sinceBeat = time % unit.interval;
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

            if (tracking.started && Input.GetKeyDown(unit.keyCodes[tracking.index]))
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
}
