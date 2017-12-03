using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour {

	// Use this for initialization
	public void IncreaseLayers () {
		SoundManager.instance.addMusicLayer ();
	}

	public void DecreaseLayers () {
		SoundManager.instance.removeMusicLayer ();
	}
}
