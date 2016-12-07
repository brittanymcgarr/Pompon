////////////////////////////////////////////////////////////////////////////////
// Cow.cs                                                                     //
// Gaze selection for petting and milking the cow.                            //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Cow : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
	public char active;
	public DateTime purchased;
	public int hearts;
	public char milk;
	public int pats;
	public DateTime collected;
	public DateTime fodder;
	public GameObject tool_hand;
	public GameObject milkCan;
	public GameObject heart0;
	public GameObject heart1;
	public GameObject heart2;

	// Private variables
	private float timeToHold = 1.5f;
	private float animateHold = 2.0f;
	private float heldTime;
	private float animateTime;
	private bool gazeIn = false;
	private bool toolIn = false;
	private bool harvest = false;
	private int cowNumber = 0;

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
		cowNumber = parentName[3] - '0';
		
		// Get the saved state of the animal
		if(cowNumber < 5) {
			string cowData = GameControl.control.cows[cowNumber];
			string[] tokens = cowData.Split(',');
			
			if(tokens.Length >= 8) {
				active = tokens[1][0];
				purchased = System.DateTime.ParseExact(tokens[2], datetimeFormat, CultureInfo.InvariantCulture);
				hearts = tokens[3][0] - '0';
				milk = tokens[4][0];
				pats = tokens[5][0] - '0';
				collected = System.DateTime.ParseExact(tokens[6], datetimeFormat, CultureInfo.InvariantCulture);
				fodder = System.DateTime.ParseExact(tokens[7], datetimeFormat, CultureInfo.InvariantCulture);
			} else {
				active = 'E';
				purchased = System.DateTime.Now;
				hearts = 0;
				milk = 'N';
				pats = 0;
				collected = System.DateTime.Now;
				fodder = System.DateTime.Now.AddDays(-1);
			}
		} else {
			gameObject.SetActive(false);
		}
		
		// Get the Hearts
		heart0 = GameObject.Find("StallHeart" + (char)(cowNumber + '0') + "0");
		heart1 = GameObject.Find("StallHeart" + (char)(cowNumber + '0') + "1");
		heart2 = GameObject.Find("StallHeart" + (char)(cowNumber + '0') + "2");
		
		// Determine if the cow is present
		if(active != 'A') {
			gameObject.SetActive(false);
			
			heart0.SetActive(false);
			heart1.SetActive(false);
			heart2.SetActive(false);
		} else {
			// Determine if the cow has milk
			Material milkMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_milkcan", typeof(Material));
			if(collected.AddDays(1) <= System.DateTime.Now) {
				if(hearts >= 3) {
					milk = 'G';
					milkMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_milkcan_good", typeof(Material));
				} else {
					milk = 'W';
				}
				
				harvest = true;
			} else {
				milk = 'N';
			}
			
			// Get the hand tool and hide it
			tool_hand = GameObject.Find("tool_glove" + (char)(cowNumber + '0'));
			tool_hand.SetActive(false);
			
			// Get the milk can and hide it
			milkCan = GameObject.Find("Milk" + (char)(cowNumber + '0'));
			milkCan.GetComponent<Renderer>().sharedMaterial = milkMaterial;
			milkCan.SetActive(false);
			
			// Determine if the cow's affection has raised
			UpdateHearts();
			
			Save();
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
			
			if(!toolIn) {
				// Check for harvest action
				if(harvest) {
					// Harvest the milk
					milkCan.SetActive(true);
					
					// Update the inventory
					if(milk == 'G') {
						GameControl.control.items[8] += 1;
					} else if(milk == 'W') {
						GameControl.control.items[7] += 1;
					} else {
					
					}
					
					milk = 'C';
					collected = System.DateTime.Now;
					
					Save();
				} else {
					// Set the hand as active, move it relative to the parent, and play its animation
					tool_hand.SetActive(true);
					tool_hand.GetComponent<Animation>().Play("tool_wateringcan");
					
					if(pats < 9) {
						pats += 1;
					} else {
						UpdateHearts();
						Save();
					}
				}
				
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
				if(harvest) {
					harvest = false;
					milkCan.SetActive(false);
					toolIn = false;
				} else {
					toolIn = false;
					tool_hand.SetActive(false);
				}
			}
		}
	}
	
	// Save the current state of the animal
	void Save() {
		string cowData = "Cow" + (char)(cowNumber + '0') + "," + active + "," + purchased.ToString(datetimeFormat) + "," +
							(char)(hearts + '0') + "," + milk + "," + (char)(pats + '0') + "," + collected.ToString(datetimeFormat) +
							"," + fodder.ToString(datetimeFormat);
		
		GameControl.control.cows[cowNumber] = cowData;
		GameControl.control.Save();
	}
	
	// Reset the cow state
	public void Reset() {
		Start();
	}
	
	// Update the cow's hearts
	void UpdateHearts() {
		if(pats >= 9 && (purchased.AddDays(3) <= System.DateTime.Now)) {
			hearts = 1;
			pats = 0;
		}
		
		if(pats >= 9 && (purchased.AddDays(10) <= System.DateTime.Now)) {
			hearts = 2;
			pats = 0;
		}
		
		if(pats >= 9 && (purchased.AddDays(21) <= System.DateTime.Now)) {
			hearts = 3;
			pats = 0;
		}
		
		// Update the heart skins
		Material material = (Material)Resources.Load("Models/Materials/Materials/cardboard_heart", typeof(Material));
		
		if(hearts >= 1) {
			heart0.GetComponent<Renderer>().sharedMaterial = material;
		}
		
		if(hearts >= 2) {
			heart1.GetComponent<Renderer>().sharedMaterial = material;
		}
		
		if(hearts >= 3) {
			heart2.GetComponent<Renderer>().sharedMaterial = material;
		}
	}
}
