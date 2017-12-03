using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public int startHealth = 3;
    public int health = 0;
    public float interval = 1.0f;
    public float offset = 0.0f;
    public List<KeyCode> keyCodes = null;
    public UnitGroup group = null;
    public float scrollSpeed = 0.2f; // TODO get from world
    public Vector3 scrollDirection = new Vector3(-1.0f, 0.0f, 0.0f);
    public Vector3 inGroupTargetPosition = Vector3.zero;

    public float maxSpeed = 1.0f;

    private UnitGroup candidateGroup = null;

    //private float lastHealthLostTime = float.NegativeInfinity;

    void Awake() {
        keyCodes = new List<KeyCode>();
        health = startHealth;
    }

    void Update () {
        if (group == null) {
            Vector3 position = transform.position;
            transform.position += scrollDirection * Time.deltaTime;
            //lastHealthLostTime = Time.time + 5.0f;
        } else {
            transform.position = Vector3.Lerp(transform.position, inGroupTargetPosition, Time.deltaTime * maxSpeed);

            //if (lastHealthLostTime < Time.time) {
            //    OnBeatEnd(false);
            //    lastHealthLostTime = Time.time + 5.0f;
            //}
        }
    }

    public void OnBeatStart() {
        // Time window for unit key presses started
    }

    public void OnBeatEnd(bool success) {
        if (health <= 0) {
            // This unit is already dismissed
            return;
        }

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
                if (health > 0) {
                    // Reduce this unit's health
                    health -= 1;
                    if (health == 0) {
                        // This unit is dismissed from the army
                        group.RemoveUnit(this);
                        group = null;
                    }
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
        //OnBeatEnd(true);
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
