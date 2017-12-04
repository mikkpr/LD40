using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

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
    private TextMeshPro noteText = null;

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

        noteText = GetComponent<TextMeshPro>();
        noteText.text = "A B C D E";
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

        System.Text.StringBuilder sb = new System.Text.StringBuilder (noteText.text);
        sb.Append(" ");

        switch (key) {
            case KeyCode.Space:
                sb.Append ("Spc");
                break;
            case KeyCode.Alpha0:
                sb.Append ("0");
                break;
            case KeyCode.Alpha1:
                sb.Append ("1");
                break;
            case KeyCode.Alpha2:
                sb.Append ("2");
                break;
            case KeyCode.Alpha3:
                sb.Append ("3");
                break;
            case KeyCode.Alpha4:
                sb.Append ("4");
                break;
            case KeyCode.Alpha5:
                sb.Append ("5");
                break;
            case KeyCode.Alpha6:
                sb.Append ("6");
                break;
            case KeyCode.Alpha7:
                sb.Append ("7");
                break;
            case KeyCode.Alpha8:
                sb.Append ("8");
                break;
            case KeyCode.Alpha9:
                sb.Append ("9");
                break;
            default:
                sb.Append (key);
                break;
        }

        noteText.text = sb.ToString();
    }

    // Called by RhythmManager when the bosses turn ends.
    public void OnTurnEnd()
    {
        // Go back into idle state.
        Debug.Log("Boss.OnTurnEnd");
        if (animator != null) {
            animator.SetBool("PlayNotes", false);
        }

        noteText.text = "";
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
