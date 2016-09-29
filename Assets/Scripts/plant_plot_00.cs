////////////////////////////////////////////////////////////////////////////////
// plant_plot_00.cs                                                           //
// Handles the fixed-gaze input response when the plant plot is viewed and    //
// held for 2 seconds. Action gets the next state of the plant and updates    //
// with an animation for the player. Stores the plot's current plant info.    //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// IGvrGazeResponder inheritance handles gaze-driven input
public class plant_plot_00 : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public string plantType = "";
	public Material material;
	public GameObject tool_wateringcan;
	
	// Private variables
	private float timeToHold = 1.5f;
	private float animateHold = 2.0f;
	private float heldTime;
	private float animateTime;
	private bool gazeIn = false;
	private bool toolIn = false;

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
		heldTime = timeToHold;
		gazeIn = false;
		
		// Create the next action tool set
		tool_wateringcan = Instantiate(tool_wateringcan) as GameObject;
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
			Debug.Log("Time Triggered!");
			heldTime = timeToHold;
			gazeIn = false;
			
			if(!toolIn) {
				// STUB: material changing script for the plant plot
				// material = (Material)Resources.Load("Models/Materials/Materials/cardboard_turnip_sprout", typeof(Material));
				// GetComponent<Renderer>().sharedMaterial = material;
				
				// Set the tool as active, move it relative to the parent, and play its animation
				tool_wateringcan.SetActive(true);
				tool_wateringcan.transform.parent = this.transform;
				tool_wateringcan.transform.localPosition = new Vector3(0.0f, 1.0f, 1.0f);
				tool_wateringcan.GetComponent<Animation>().Play("tool_wateringcan");
				
				// Start the keep alive time for the animation
				animateTime = animateHold;
				toolIn = true;
			}
		}
		
		// Check for a started animation
		if(toolIn) {
			animateTime -= Time.deltaTime;
			
			// When 2 seconds have passed, the animation ends, and the tool is removed.
			if(animateTime <= 0.0f) {
				toolIn = false;
				
				tool_wateringcan.SetActive(false);
			}
		}
	}
}
