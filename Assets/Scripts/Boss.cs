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

    public Vector3 finalRelativePosition = new Vector3(-8.0f, 0.0f, 0.0f);
    public float enterSpeed = 0.5f;

    public float sceneChangeDelta = 10.0f;

    private float grooveStep = 0.0f;
    private Animator animator = null;
    private float sceneChangeTime = float.PositiveInfinity;
    private string sceneChangeName = "";
    private bool endTriggered = false;
    private bool entering = false;
    private Vector3 finalPosition;

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

    void Awake()
    {
        Component[] components = GetComponentsInChildren<Animator>();
        if (components.Length > 0) {
            animator = (Animator)components[0];
        }
    }

    void Start()
    {
        grooveStep = 100.0f / groove;
    }

    void Update()
    {
        float t = Time.time;

        if (entering)
        {
            transform.position = Vector3.Lerp(transform.position, finalPosition, Time.deltaTime * enterSpeed);
        }

        if (t >= sceneChangeTime) {
            SceneManager.LoadScene(sceneChangeName);
        }
    }

    public void EnterLevel()
    {
        entering = true;
        finalPosition = transform.position + finalRelativePosition;
    }

    // Called by RhythmManager when the boss has to wind up for a new key.
    public void OnWindup()
    {
        // Start windup animation.
        Debug.Log("Boss.OnWindup");
        if (animator != null) {
            animator.SetBool("PlayNotes", true);
        }
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
        if (animator != null) {
            animator.SetBool("PlayNotes", false);
        }
    }

    // Called by RhythmManager when the player misses a note.
    public void OnMiss()
    {
        // Decrease groove bar.
        grooveBar.Decrement(grooveStep);

        // Change Scene to failure if groove empty.
        if ((groove--) <= 0)
        {
            TriggerEnd("Lose");
        }
    }

    // Called by RhythmManager when the player completed all Challenges.
    public void OnSuccess()
    {
        TriggerEnd("Win");
    }

    void TriggerEnd(string name) {
        if (!endTriggered) {
            endTriggered = true;

            // Change Scene to success.
            if (animator != null) {
                animator.SetTrigger(name);
            }

            sceneChangeName = name;
            sceneChangeTime = Time.time + sceneChangeDelta;
        }
    }
}
