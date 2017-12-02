using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour {

    public float tact = 1.0f;
    public List<KeyCode> keyCodes = null;

    void Awake() {
        keyCodes = new List<KeyCode>();
    }

    void Update () {
    }

    void OnBeatStart() {
    }

    void OnBeatEnd() {
    }

    void OnBeatHit() {
    }

    void OnBeatMissed() {
    }

    void OnBeatDouble() {
    }

    void OnOutOfBeat() {
    }
}
