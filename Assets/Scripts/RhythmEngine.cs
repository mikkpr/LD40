using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmEngine : MonoBehaviour
{

    // Time of the initial beat.
    public float initial;

    // Interval between beats.
    public float interval;

    // Allowed difference from the perfect beat.
    public float accuracy;

    // Key that must be pressed to hit beat.
    public KeyCode key;

    public Text text;

    bool inBeat;
    bool pressed;

    void Start()
    {
        
    }

    void Update()
    {
        float sinceInitial = Time.time - initial;
        float sinceBeat = sinceInitial % interval;

        bool oldInBeat = inBeat;
        inBeat = sinceBeat < accuracy || sinceBeat > (interval - accuracy);

        if (!oldInBeat && inBeat)
        {
            pressed = false;
            OnBeatStart();
        }
        else if (oldInBeat && !inBeat)
        {
            OnBeatEnd();
            if (!pressed)
            {
                OnBeatMissed();
            }
        }

        if (Input.GetKeyDown(key))
        {
            if (!inBeat)
            {
                OnOutOfBeat();
                return;
            }
            if (pressed)
            {
                OnBeatDouble();
                return;
            }
            OnBeatHit();
            pressed = true;
        }
    }

    void OnBeatStart()
    {
        text.text = "start";
    }

    void OnBeatEnd()
    {
        text.text = "end";
    }

    void OnBeatMissed()
    {
        text.text = "missed";
    }

    void OnOutOfBeat()
    {
        text.text = "out of beat";
    }

    void OnBeatDouble()
    {
        text.text = "double";
    }

    void OnBeatHit()
    {
        text.text = "hit!";
    }
}
