////////////////////////////////////////////////////////////////////////////////
// BarnDoorToBarn.cs                                                          //
// Gaze selection to transport the player to the Barn Interior.               //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class BarnDoorToBarn : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public Vector3[] SpawnPoints = new Vector3[2];
	
	// Private variables
	private float timeToHold = 3.0f;
	private float heldTime;
	private bool gazeIn = false;
	private GameObject controller;

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
		
		SpawnPoints[0] = new Vector3(-7.39f, 0.0f, 6.17f);
		SpawnPoints[1] = new Vector3(-4.8f, 0.0f, -8.0f);
		
		controller = GameObject.Find("GvrViewerMain");
		controller.transform.position = SpawnPoints[PlayerPrefs.GetInt("sceneId")];
		PlayerPrefs.SetInt("sceneId", Application.loadedLevel);
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
			PlayerPrefs.SetInt("sceneId", Application.loadedLevel);
			LoadScene();
		}
	}
	
	public void LoadScene() {
		Application.LoadLevel(1);
	}
}