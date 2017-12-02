using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public int startHealth = 3;
    public float interval = 1.0f;
    public List<KeyCode> keyCodes = null;
    public UnitGroup group = null;
    public float scrollSpeed = 0.2f; // TODO get from world
    public Vector3 scrollDirection = new Vector3(-1.0f, 0.0f, 0.0f);

    private UnitGroup candidateGroup = null;
    private int health = 0;

    void Awake() {
        keyCodes = new List<KeyCode>();
        health = startHealth;
    }

    void Update () {
        if (group == null) {
            Vector3 position = transform.position;
            transform.position += scrollDirection * Time.deltaTime;
        } else {
            // TODO move inside group
        }
    }

    public void OnBeatStart() {
        // Time window for unit key presses started
    }

    public void OnBeatEnd(bool success) {
        // Time window for unit key presses ended
        if (success) {
            // This unit is successful
            if (group == null) {
                // Not in the army yet
                if (candidateGroup != null) {
                    // Join the army
                    candidateGroup.AddUnit(this);
                    group = candidateGroup;
                }
            } else {
                // We are in the army
                // Keep up the good work
            }
        } else {
            // This unit failed
            if (group != null) {
                // Reduce this unit's health
                health -= 1;
                if (health <= 0) {
                    // This unit has been removed from the army
                    group.RemoveUnit(this);
                    group = null;
                }
            }
        }
    }

    public void OnBeatHit() {
        // Input was pressed in time window
    }

    public void OnBeatDouble() {
        // Double input was given in time window
    }

    public void OnOutOfBeat() {
        // Keys were pressed out of time window
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (candidateGroup != null) {
            return;
        }
        if (other.gameObject.tag == "Army") {
            candidateGroup = other.gameObject.GetComponent<UnitGroup>();
        }
    }

    private void OnTriggerExit2D(Collider2D other) {
        if (candidateGroup == null) {
            return;
        }
        if (other.gameObject.tag == "Army") {
            candidateGroup = null;
        }
    }
}
