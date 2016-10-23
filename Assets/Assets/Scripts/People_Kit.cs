////////////////////////////////////////////////////////////////////////////////
// People_Kit.cs                                                              //
// Kit, the farmhand. He is generally quiet when in town but enjoys the       //
// company of the animals in the barn. He helps the player with adding rows   //
// to the field and tending the animals.                                      //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class People_Kit : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public GameObject speechBubble;
	public Text text;

	// Private variables
	private float timeToHold = 1.5f;
	private float timeToEmote = 3.0f;
	private float timeToRead = 4.0f;
	private float heldTime;
	private float emoteTime;
	private float readTime;
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
		
		// Load the SpeechCanvas
		speechBubble = GameObject.Find("SpeechCanvas").gameObject;
		text = GameObject.Find("SpeechText").GetComponent<Text>();
		speechBubble.transform.SetParent(this.transform, false);
		speechBubble.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		speechBubble.SetActive(false);
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
			
			emoteTime = timeToEmote;
			animateIn = true;
			
			speechBubble.SetActive(true);
			readTime = timeToRead;
		}
		
		// Time out the tutorial text, if active
		if(readTime > 0.0f) {
			readTime -= Time.deltaTime;
			
			if(readTime <= 0.0f) {
				speechBubble.SetActive(false);
			}
		}
	}
}
