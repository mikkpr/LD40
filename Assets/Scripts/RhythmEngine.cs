using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RhythmEngine : MonoBehaviour {

	// Time of the initial beat.
	public float initial;

	// Interval between beats.
	public float interval;

	// Allowed difference from the perfect beat.
	public float accuracy;

	// Key that must be pressed to hit beat.
	public KeyCode key;

	public Text text;

	private bool inBeat;
	private bool pressed;

	void Start() {
		inBeat = false;
		pressed = false;
	}

	void Update () {
		float sinceInitial = Time.time - initial;
		float sinceBeat = sinceInitial % interval;

		bool oldInBeat = inBeat;
		inBeat = sinceBeat < accuracy || sinceBeat > (interval - accuracy);

		if (!oldInBeat && inBeat) {
			pressed = false;
			onBeatStart();
		} else if (oldInBeat && !inBeat) {
			onBeatEnd();
			if (!pressed) {
				onBeatMissed();
			}
		}

		if (Input.GetKeyDown(key)) {
			if (!inBeat) {
				onOutOfBeat();
				return;
			}
			if (pressed) {
				onBeatDouble();
				return;
			}
			onBeatHit();
			pressed = true;
		}
	}

	void onBeatStart() {
		text.text = "start";
	}

	void onBeatEnd() {
		text.text = "end";
	}

	void onBeatMissed() {
		text.text = "missed";
	}

	void onOutOfBeat() {
		text.text = "out of beat";
	}

	void onBeatDouble() {
		text.text = "double";
	}

	void onBeatHit() {
		text.text = "hit!";
	}
}
