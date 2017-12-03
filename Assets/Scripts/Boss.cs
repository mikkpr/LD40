using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    // Challenges issued by this Boss.
    public List<Challenge> challenges = null;

    [System.Serializable]
    public class Challenge
    {
        // Start time of the Challenge.
        public float start = 0.0f;

        // Length of the Challenge.
        public float length = 0.0f;

        // Offsets from the start of the Challenge when notes are played.
        public List<float> offsets = null;
    }

    // Called by RhythmManager when the boss has to wind up for a new key.
    public void OnWindup()
    {
    }

    // Called by RhythmManager when the boss has to hit a key ("note").
    public void OnNote(KeyCode key)
    {
    }
}
