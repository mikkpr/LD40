using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Boss : MonoBehaviour
{
    // Number of times the player can miss before their Groove runs out.
    public int groove = 4;

    // GrooveBar containing current Groove.
    public GrooveBar grooveBar = null;

    // Challenges issued by this Boss.
    public List<Challenge> challenges = null;

    private float grooveStep;

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

    void Start()
    {
        grooveStep = 100.0f / groove;
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
        grooveBar.Decrement(grooveStep);

        // Change Scene to failure if groove empty.
        if ((groove--) <= 0)
        {
            SceneManager.LoadScene("Lose");
        }
    }

    // Called by RhythmManager when the player completed all Challenges.
    public void OnSuccess()
    {
        SceneManager.LoadScene("Win");
    }
}
