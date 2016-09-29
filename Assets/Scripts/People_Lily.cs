////////////////////////////////////////////////////////////////////////////////
// People_Lily.cs                                                             //
// Lily, the flower cart girl, interacts with the character to deal in seeds  //
// and produce. She is very friendly and helpful.                             //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class People_Lily : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	
	// Private variables
	private float timeToHold = 1.5f;
	private float timeToEmote = 3.0f;
	private float heldTime;
	private float emoteTime;
	private Material material;
	private bool gazeIn = false;
	private bool animateIn = false;

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
		gazeIn = false;
		heldTime = timeToHold;
		
		material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily_happy", typeof(Material));
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
			heldTime = timeToHold;
			gazeIn = false;
			
			// STUB: material changing script for the plant plot
			GetComponent<Renderer>().sharedMaterial = material;
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily", typeof(Material));
			
			emoteTime = timeToEmote;
			animateIn = true;
		}
		
		if(animateIn) {
			emoteTime -= Time.deltaTime;
			
			if(emoteTime <= 0.0f) {
				GetComponent<Renderer>().sharedMaterial = material;
				animateIn = false;
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily_happy", typeof(Material));
			}
		}
	}
}
