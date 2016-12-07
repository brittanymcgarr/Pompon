using UnityEngine;
using System.Collections;

public class carrotDetector : MonoBehaviour {

	// Use this for initialization
	void Start () {
		int index = 0;
		
		while((index < 32) && !(GameControl.control.plots[index].Contains("carrot"))) {
			index++;
		}
		
		if(index >= 32) {
			GameControl.control.carrotPlanted = false;
		} else {
			GameControl.control.carrotPlanted = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
