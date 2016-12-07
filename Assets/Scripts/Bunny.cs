////////////////////////////////////////////////////////////////////////////////
// Bunny.cs                                                                   //
// Gaze selection for generating and capturing a random rabbit if the player  //
// has planted carrots.                                                       //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Bunny : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
	public int alleleA, alleleB;
	public char gender;
	public DateTime age;

	// Private variables
	private float timeToHold = 3.0f;
	private float heldTime;
	private bool gazeIn = false;
	private int bunnyNumber = 0;
	private bool bunny = false;

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
		
		// Check if the carrots were planted in order for the rabbit to appear
		if(GameControl.control.carrotPlanted && GameControl.control.bunnyCaught.AddDays(1) <= System.DateTime.Now) {
			bunnyNumber = 0;
			
			while(bunnyNumber < 6 && GameControl.control.bunnies[bunnyNumber][8] != '0') {
				bunnyNumber++;
			}
			
			if(bunnyNumber < 6) {
				// Generate bunny alleles and gender based on time
				alleleA = System.DateTime.Now.Second % 4 + 1;
				alleleB = System.DateTime.Now.Millisecond % 4 + 1;
				
				int randomGender = System.DateTime.Now.Millisecond % 2 + 1;
				if(randomGender == 1) {
					gender = 'M';
				} else {
					gender = 'F';
				}
				
				age = System.DateTime.Now.AddDays(-3);
				
				// Get the skin of the bunny based on the generated alleles
				int dominantGene;
				if(alleleA > alleleB) {
					dominantGene = alleleA;
				} else {
					dominantGene = alleleB;
				}
				
				// Brown is the most dominant gene
				Material bunnyMaterial;
				if(dominantGene == 4) {
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_brown", typeof(Material));
				} else if(dominantGene == 3) {
					// Followed by black
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_black", typeof(Material));
				} else if(dominantGene == 2) {
					// Gray
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_cutout", typeof(Material));
				} else {
					// White is the rarest
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_white", typeof(Material));
				}
				
				GetComponent<Renderer>().sharedMaterial = bunnyMaterial;
				
				bunny = true;
			} else {
				gameObject.SetActive(false);
			}
		} else {
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(bunny) {
			// Decrement the timer if the gaze hit the object
			if(gazeIn) {
				heldTime -= Time.deltaTime;
			}
			
			if(heldTime <= 0.0f) {
				// Perform the event and reset the timer and boolean
				// Debug.Log("Time Triggered!");
				heldTime = timeToHold;
				gazeIn = false;
				
				bunny = false;
				
				if(bunnyNumber < 6) {
					GameControl.control.bunnyCaught = System.DateTime.Now;
					
					GameControl.control.bunnies[bunnyNumber] = "Bunny0" + (char)(bunnyNumber + '0') + "," + 
											(char)(alleleA + '0') + "," + (char)(alleleB + '0') + "," + 
											gender + "," + age.ToString(datetimeFormat);
					
					GameControl.control.Save();
					
					gameObject.SetActive(false);
				}
			}
		}
	}
}
