﻿////////////////////////////////////////////////////////////////////////////////
// RightButton.cs                                                             //
// Gaze selection for right button menu item.                                 //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class RightButton : MonoBehaviour, IGvrGazeResponder {

	// Private variables
	private float timeToHold = 1.5f;
	private float heldTime;
	private bool gazeIn = false;
	private GameObject lily;
	private People_Lily lilyScript;

	// On entering the gaze event, set the timer and boolean
	public void OnGazeEnter() {
		heldTime = timeToHold;
		gazeIn = true;
	}
	
	// On leaving the gaze event, reset the timer and gaze boolean
	public void OnGazeExit() {
		heldTime = timeToHold;
		gazeIn = false;
	}
	
	// On pressing the button while viewing (not used)
	public void OnGazeTrigger() {
	
	}

	// Use this for initialization
	void Start () {
		// Initialize the gaze action variables
		heldTime = timeToHold;
		gazeIn = false;
		
		// Find Lily to communicate
		lily = GameObject.Find("town_lily").gameObject;
		lilyScript = lily.GetComponent<People_Lily>();
	}
	
	// Update is called once per frame
	void Update () {
		// Decrement the timer if the gaze hit the object
		if(gazeIn) {
			heldTime -= Time.deltaTime;
		}
		
		// When the time has reached zero (gaze was held for 2 seconds)
		if(heldTime <= 0.0f) {
			// Perform the event and reset the timer and boolean
			// Debug.Log("Time Triggered!");
			heldTime = timeToHold;
			gazeIn = false;
			
			if(lilyScript.currentMenu == "") {
				lilyScript.currentMenu = "sell";
			} else {
				lilyScript.nextMenu = "right";
			}
		}
	}
}
