using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour {

	// Use this for initialization
	public void IncreaseLayers () {
		if (SoundManager.instance.activeLayers < SoundManager.instance.maxLayers) {
			SoundManager.instance.activeLayers++;
		}
	}

	public void DecreaseLayers () {
		if (SoundManager.instance.activeLayers > 1) {
			SoundManager.instance.activeLayers--;
		}
	}
}
