using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float tact = 1.0f;
    public List<KeyCode> keyCodes = null;
    public UnitGroup group = null;

    void Awake() {
        keyCodes = new List<KeyCode>();
    }

    void Update () {
    }

    void OnBeatStart() {
        // Time window for unit key presses started
    }

    void OnBeatEnd(bool success) {
        // Time window for unit key presses ended
    }

    void OnBeatHit() {
        // Input was pressed in time window
    }

    void OnBeatDouble() {
        // Double input was given in time window
    }

    void OnOutOfBeat() {
        // Keys were pressed out of time window
    }
}
