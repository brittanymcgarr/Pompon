////////////////////////////////////////////////////////////////////////////////
// Nest.cs                                                                    //
// Gathers the game control values for the chicken's nest and updates the     //
// nest and animal information for interaction.                               //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class Nest : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public Material nestMaterial;
	public Material chickenMaterial;
	public Material heartMaterial;
	public GameObject tool_hand;
	public GameObject chicken;
	public GameObject egg;
	public GameObject NestHeart0;
	public GameObject NestHeart1;
	public GameObject NestHeart2;
	public int nestNumber;
	public char stage;
	public char gender;
	public DateTime birthTime;
	public int heartStage;
	public char eggStatus;
	public int pats;
	public DateTime eggTime;
	public const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
	public GameObject parentManager;
	public ChickensManager manager;
	
	// Private variables
	private float timeToHold = 1.5f;
	private float animateHold = 2.0f;
	private float heldTime;
	private float animateTime;
	private bool gazeIn = false;
	private bool toolIn = false;
	private bool harvest = false;
	
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
		
		// Get the parent nest's numbering
		string parentName = gameObject.transform.name;
		nestNumber = parentName[5] - '0';
		
		// Get the save state information and parse it for values
		if(nestNumber < 8) {
			// Parsing the saved data
			string saveData = GameControl.control.chickens[nestNumber];
			string[] tokens = saveData.Split(',');
			
			parentManager = GameObject.Find("Chickens").gameObject;
			manager = parentManager.GetComponent<ChickensManager>();
			
			// Nest values in string data format
			// "Nest##,<Growth Stage character: 'E'mpty, E'G'g, 'C'hick, 'A'dult>,<Gender character: 'N'one, 'F'emale, 'M'ale>,
			// <DateTime string of purchase/laid in "yyyy-MM-dd HH:mm:ss.fffffff">,<Hearts # character: '0', '1', '2', '3'>,
			// <Egg status: 'N'one, 'C'ollected, 'W'aiting, 'G'old>,<Pats # character: '0' ... '9'>,
			// <DateTime string of collection in "yyyy-MM-dd HH:mm:ss.fffffff">"
			if(saveData != "" && tokens.Length >= 7) {
				stage = tokens[1][0];
				gender = tokens[2][0];
				birthTime = System.DateTime.ParseExact(tokens[3], datetimeFormat, CultureInfo.InvariantCulture);
				heartStage = tokens[4][0] - '0';
				eggStatus = tokens[5][0];
				pats = tokens[6][0] - '0';
				eggTime = System.DateTime.ParseExact(tokens[7], datetimeFormat, CultureInfo.InvariantCulture);
				
				// Check for an active egg
				if(eggStatus == 'W' || eggStatus == 'G') {
					harvest = true;
				}
			} else {
				stage = 'E';
				gender = 'N';
				birthTime = System.DateTime.Now;
				heartStage = 0;
				eggStatus = 'N';
				pats = 0;
				eggTime = System.DateTime.Now;
			}
			
			// Get the child tool, egg, chicken, and heart sets
			tool_hand = GameObject.Find("tool_hand0" + (char)(nestNumber + '0'));
			chicken = GameObject.Find("Chicken0" + (char)(nestNumber + '0'));
			egg = GameObject.Find("Egg" + (char)(nestNumber + '0'));
			NestHeart0 = GameObject.Find("NestHeart_" + (char)(nestNumber + '0') + "0");
			NestHeart1 = GameObject.Find("NestHeart_" + (char)(nestNumber + '0') + "1");
			NestHeart2 = GameObject.Find("NestHeart_" + (char)(nestNumber + '0') + "2");
			
			// Set the egg and tool inactive, for now
			tool_hand.SetActive(false);
			egg.SetActive(false);
			
			// Age up the chicken, if necessary
			AgeUp();
			
			// Check the hearts of the chicken
			UpdateHearts();
			
			// Interpret the status of the nest and set the materials for the chicken and the hearts
			if(stage == 'G') {
				// If the chicken's stage is egg, check for gold or normal
				if(eggStatus == 'G') {
					chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_egg_good", typeof(Material));
				} else {
					chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_egg", typeof(Material));
				}
			} else if(stage == 'C') {
				// Chicks are neutral
				chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_chick_cutout", typeof(Material));
				chicken.transform.SetParent(this.transform, false);
				chicken.transform.localScale = new Vector3(0.01f, 0.5f, 0.3f);
				chicken.transform.localPosition = new Vector3(0.0f, 0.75f, 0.02f);
			} else if(stage == 'A') {
				// Adults can be male (rooster) or female
				if(gender == 'M') {
					chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_rooster_cutout", typeof(Material));
				} else {
					chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_chicken_cutout", typeof(Material));
				}
			} else {
				// No interprated data, make empty
				chicken.SetActive(false);
				NestHeart0.SetActive(false);
				NestHeart1.SetActive(false);
				NestHeart2.SetActive(false);
			}
			
			// Set the material
			chicken.GetComponent<Renderer>().sharedMaterial = chickenMaterial;
			
			// Handle the hearts
			heartMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_heart", typeof(Material));
			
			if(heartStage > 0) {
				if(heartStage >= 1) {
					NestHeart0.GetComponent<Renderer>().sharedMaterial = heartMaterial;
				}
				
				if(heartStage >= 2) {
					NestHeart1.GetComponent<Renderer>().sharedMaterial = heartMaterial;
				}
				
				if(heartStage >= 3) {
					NestHeart2.GetComponent<Renderer>().sharedMaterial = heartMaterial;
				}
			}
			
			// Get the nest material, if an egg was laid
			if(eggStatus == 'W' || eggStatus == 'G') {
				nestMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_nest_ready", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = nestMaterial;
			}
		}
		
		Save();
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
					// Harvest the egg
					egg.SetActive(true);
					egg.transform.SetParent(this.transform, false);
					egg.transform.localPosition = new Vector3(0.0f, 3.0f, 0.0f);
					
					// Update the inventory
					if(eggStatus == 'G') {
						GameControl.control.items[6] += 1;
					} else if(eggStatus == 'W') {
						GameControl.control.items[5] += 1;
					} else {
					
					}
					
					eggStatus = 'C';
					eggTime = System.DateTime.Now;
					nestMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_nest_empty", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = nestMaterial;
					
					Save();
				} else if(stage == 'E' && manager.malePresent && GameControl.control.items[5] > 0) {
					GameControl.control.items[5] -= 1;
					chickenMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_egg", typeof(Material));
					chicken.GetComponent<Renderer>().sharedMaterial = chickenMaterial;
					chicken.SetActive(true);
					stage = 'G';
					gender = 'N';
					birthTime = System.DateTime.Now;
					heartStage = 0;
					eggStatus = 'N';
					pats = 0;
					eggTime = System.DateTime.Now;
					
					Save();
				} else {
					// Set the hand as active, move it relative to the parent, and play its animation
					tool_hand.SetActive(true);
					tool_hand.transform.SetParent(this.transform, false);
					tool_hand.transform.localPosition = new Vector3(-0.011f, 1.204f, -0.054f);
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
					egg.SetActive(false);
					toolIn = false;
				} else {
					toolIn = false;
					tool_hand.SetActive(false);
				}
			}
		}
	}
	
	// Save game stage information
	void Save() {
		if(nestNumber < 8) {
			// Nest values in string data format
			// "Nest##,<Growth Stage character: 'E'mpty, E'G'g, 'C'hick, 'A'dult>,<Gender character: 'N'one, 'F'emale, 'M'ale>,
			// <DateTime string of purchase/laid in "yyyy-MM-dd HH:mm:ss.fffffff">,<Hearts # character: '0', '1', '2', '3'>,
			// <Egg status: 'N'one, 'C'ollected, 'W'aiting, 'G'old>,<Pats # character: '0' ... '9'>,
			// <DateTime string of collection in "yyyy-MM-dd HH:mm:ss.fffffff">"
			string dateString = birthTime.ToString(datetimeFormat);
			string eggString = eggTime.ToString(datetimeFormat);
			string saveData = "Nest0" + (char)(nestNumber + '0') + "," + stage + "," + gender + "," + dateString + "," +
								(char)(heartStage + '0') + "," + eggStatus + "," + (char)(pats + '0') + "," + eggString;
								
			GameControl.control.chickens[nestNumber] = saveData;
			GameControl.control.Save();
		} 
	}
	
	// Reset the nest
	public void Reset() {
		Start();
	}
	
	// Handles aging of animal
	void AgeUp() {
		// Check if the nest is occupied, and if so, age up the animal
		// Chicks age up to adulthood after five days
		if(stage == 'C') {
			if(birthTime.AddDays(5) <= System.DateTime.Now) {
				stage = 'A';
			}
		}
		
		// Eggs age into chicks at 2 days
		if(stage == 'G') {
			if(birthTime.AddDays(2) <= System.DateTime.Now) {
				stage = 'C';
				
				// Gender is random, but 1/2 times will be female
				System.Random random = new System.Random();
				int randomNumber = random.Next(1,6);
				
				if(randomNumber % 2 == 1) {
					gender = 'F';
				} else {
					gender = 'M';
				}
				
				eggStatus = 'N';
			}
		}
	}
	
	// Updates the heart stages for the chicken
	void UpdateHearts() {
		if(heartStage < 1 && pats == 9 && (birthTime.AddDays(3) <= System.DateTime.Now)) {
			heartStage = 1;
			pats = 0;
		}
		
		if(heartStage < 2 && pats == 9 && (birthTime.AddDays(7) <= System.DateTime.Now)) {
			heartStage = 2;
			pats = 0;
		}
		
		if(heartStage < 3 && pats == 9 && (birthTime.AddDays(14) <= System.DateTime.Now)) {
			heartStage = 3;
			pats = 0;
		}
	}
}
