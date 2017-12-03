using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour {	
	public void onButtonPress() {
		SoundManager.instance.PlayMusic();
	}
}
