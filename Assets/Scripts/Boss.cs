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
        // Start windup animation.
        Debug.Log("Boss.OnWindup");
    }

    // Called by RhythmManager when the boss has to hit a key ("note").
    public void OnNote(KeyCode key)
    {
        // Play note animation.
        Debug.Log("Boss.OnNote(" + key + ")");
    }

    // Called by RhythmManager when the bosses turn ends.
    public void OnTurnEnd()
    {
        // Go back into idle state.
        Debug.Log("Boss.OnTurnEnd");
    }

    // Called by RhythmManager when the player misses a note.
    public void OnMiss()
    {
        // Decrease groove bar.
        // Change Scene to failure if groove empty.
        Debug.Log("Boss.OnMiss");
    }

    // Called by RhythmManager when the player completed all Challenges.
    public void OnSuccess()
    {
        // Change Scene to success.
        Debug.Log("Boss.OnSuccess");
    }
}
