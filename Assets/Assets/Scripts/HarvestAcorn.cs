﻿////////////////////////////////////////////////////////////////////////////////
// HarvestAcorn.cs                                                            //
// Randomly get an acorn from the tree up to ten times a day.                 //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class HarvestAcorn : MonoBehaviour, IGvrGazeResponder {
	// Private variables
	private float timeToHold = 1.5f;
	private float heldTime;
	private bool gazeIn = false;
	private bool harvested = false;

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
	}
	
	// Update is called once per frame
	void Update () {
		// Decrement the timer if the gaze hit the object
		if(gazeIn) {
			heldTime -= Time.deltaTime;
		}
		
		// When the time has reached zero (gaze was held for 2 seconds)
		if(!harvested && heldTime <= 0.0f) {
			// Perform the event and reset the timer and boolean
			// Debug.Log("Time Triggered!");
			heldTime = timeToHold;
			gazeIn = false;
			
			if(GameControl.control.getAcorn()) {
				harvested = true;
				
				GameObject acorn = Instantiate(Resources.Load("Models/item_acorn")) as GameObject;
				acorn.SetActive(true);
				acorn.transform.SetParent(this.transform, false);
				acorn.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
				acorn.transform.localScale = new Vector3(-1.0f, -1.0f, 1.0f);
				Destroy(acorn, 2.0f);
			}
		}
	}
}
