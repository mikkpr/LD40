using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrooveBar : MonoBehaviour {
	public GameObject grooveBar;
	float value = 100;

	public void SetValue (float value) {
		float clampedValue = Mathf.Clamp (value, 0, 100);
		this.value = clampedValue;
	}
	public float GetValue (float value) {
		return this.value;
	}

	public void Decrement (float amount) {
		SetValue (this.value - amount);
	}

	public void Increment (float amount) {
		SetValue (this.value + amount);
	}

	void Update () {
		GameObject bar = grooveBar.transform.Find ("Bar").gameObject;
		Vector3 scale = new Vector3 (this.value / 100f, 1f, 1f);
		bar.transform.localScale = scale;
	}

	public void OnIncrementButtonClick () {
		this.Increment (10f);
	}

	public void OnDecrementButtonClick () {
		this.Decrement (10f);
	}
}
