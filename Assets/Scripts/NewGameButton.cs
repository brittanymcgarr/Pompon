////////////////////////////////////////////////////////////////////////////////
// NewGameButton.cs                                                           //
// Gaze selection to transport the player to the Pompon Village scene.        //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class NewGameButton : MonoBehaviour, IGvrGazeResponder {
	// Private variables
	private float timeToHold = 3.0f;
	private float heldTime;
	private bool gazeIn = false;
	private GameObject controller;
	private GameObject loading;
	
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
	
	// Find the loading screen on awake
	void Awake() {
		//loading = GameObject.Find("Loading");
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
		if(heldTime <= 0.0f) {
			// Perform the event and reset the timer and boolean
			// Debug.Log("Time Triggered!");
			heldTime = timeToHold;
			gazeIn = false;
			PlayerPrefs.SetInt("sceneId", Application.loadedLevel);
			//loading.SetActive(true);
			LoadScene();
		}
	}
	
	// Load the Pompon World Map
	public void LoadScene() {
		GameControl.control.EraseSave();
		Application.LoadLevel(1);
	}
}