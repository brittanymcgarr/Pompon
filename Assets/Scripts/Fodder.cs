////////////////////////////////////////////////////////////////////////////////
// Fodder.cs                                                                  //
// Gaze selection for filling the fodder bin of the selected stall.           //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Fodder : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public bool filled = false;
	public Material material;
	public DateTime fillTime;
	public const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";

	// Private variables
	private float timeToHold = 1.5f;
	private float heldTime;
	private bool gazeIn = false;
	private int fodderNumber = 0;

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
		
		// Get the parent's number
		string parentName = gameObject.transform.name;
		fodderNumber = parentName[6] - '0';
		
		// Get the last time the fodder was filled for the cow
		if(fodderNumber < 5) {
			string cowData = GameControl.control.cows[fodderNumber];
			string[] tokens = cowData.Split(',');
			
			if(tokens.Length >= 8) {
				fillTime = System.DateTime.ParseExact(tokens[7], datetimeFormat, CultureInfo.InvariantCulture);
				
				if(fillTime.AddDays(1) <= System.DateTime.Now) {
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_fodder", typeof(Material));
					filled = true;
				} else {
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard-textures-5", typeof(Material));
					filled = false;
				}
				
				GetComponent<Renderer>().sharedMaterial = material;
			}
		}
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
			
			if(!filled) {
				filled = true;
				
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_fodder", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
				
				string cowData = GameControl.control.cows[fodderNumber];
				string[] tokens = cowData.Split(',');
				DateTime now = System.DateTime.Now;
				string nowTime = now.ToString(datetimeFormat);
				
				tokens[7] = nowTime;
				string returnData = String.Join(",", tokens);
				GameControl.control.cows[fodderNumber] = returnData;
				
				GameControl.control.Save();
			}
		}
	}
}
