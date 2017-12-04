using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {	
	public void onButtonPress(string type) {
		SoundManager.instance.PlaySound(type);
	}
}
