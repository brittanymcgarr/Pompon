////////////////////////////////////////////////////////////////////////////////
// SeedHopper.cs                                                              //
// The seed hopper takes one of the player's grown produce and converts it    //
// into seeds for the player to use. Useful for growing out-of-season produce.//
// The repaired seed hopper costs 50,000 gold and takes 6 hours to produce 2  //
// seed packets. The upgraded seed hopper costs 100,000 gold and takes 6 hours//
// to produce 3 packets.                                                      //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class SeedHopper : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public float timeToHold = 1.5f;
	public float timeToRead = 120.0f;
	public float heldTime = 0.0f;
	private float readTime;
	public bool gazeIn = false;
	public bool menuActive = false;
	public bool seedsReady = false;
	
	public GameObject menuCanvas;
	public Text text;
	public Text menuText;
	public Text leftText;
	public Text rightText;
	public Text exitText;
	public string nextMenu = "";
	public string currentMenu = "";
	public GameObject leftButton;
	public GameObject rightButton;
	public GameObject exitButton;
	public int produceChoice = 0;
	
	public string[] produceType;
	public char stage = 'b';
	public Material material;
	public DateTime depositTime;
	public int plantType;
	
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
		
		// Load the seed names
		produceType = new string[16];
		produceType[0] = "Turnip";
		produceType[1] = "Carrot";
		produceType[2] = "Pumpkin";
		produceType[3] = "Broccoli";
		produceType[4] = "Tomato";
		produceType[5] = "Sugarplum";
		produceType[6] = "Leek";
		produceType[7] = "Pommegranate";
		produceType[8] = "Lettuce";
		produceType[9] = "Pea";
		produceType[10] = "Cantaloupe";
		produceType[11] = "Grapes";
		produceType[12] = "Watermelon";
		produceType[13] = "Strawberry";
		produceType[14] = "Sweet Potato";
		produceType[15] = "Cranberry";
		
		// Load the buttons and scripts
		leftButton = GameObject.Find("HopperLeftCollider").gameObject;
		leftText = GameObject.Find("HopperLeftOption").GetComponent<Text>();
		
		rightButton = GameObject.Find("HopperRightCollider").gameObject;
		rightText = GameObject.Find("HopperRightOption").GetComponent<Text>();
		
		exitButton = GameObject.Find("HopperExitCollider").gameObject;
		exitText = GameObject.Find("HopperExitOption").GetComponent<Text>();
		
		// Load the MenuCanvas
		menuCanvas = GameObject.Find("HopperMenuCanvas").gameObject;
		menuText = GameObject.Find("HopperMenuText").GetComponent<Text>();
		menuCanvas.transform.SetParent(this.transform, false);
		menuCanvas.transform.localPosition = new Vector3(-0.47f, 1.0f, 0.012f);
		menuCanvas.SetActive(false);
		
		// Get the current repair stage
		stage = GameControl.control.freezerStage;
		
		// Check if the seed hopper is ready and set the skin
		// Get the values from the GameControl in format:
		// "<Repair Stage char>,<Int of plant type position>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
		if(GameControl.control.seedHopper != "") {
			string saveData = GameControl.control.seedHopper;
			
			if(saveData != "" && saveData != null) {
				string[] dataTokens = saveData.Split(',');
				string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
				stage = (dataTokens[0])[0];
				plantType = Int32.Parse(dataTokens[1]);
				depositTime = System.DateTime.ParseExact(dataTokens[2], datetimeFormat, CultureInfo.InvariantCulture);
			} else {
				string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
				stage = 'b';
				plantType = -1;
				depositTime = System.DateTime.Now;
				DateTime timestamp = System.DateTime.Now;
				string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
				GameControl.control.seedHopper = "b,-1," + datenow;
				GameControl.control.Save();
			}
		} else {
			stage = 'b';
			plantType = -1;
			depositTime = System.DateTime.Now;
		}
		
		// Set the appliance's material
		if(stage == 'r' || stage == 'u') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_seedhopper", typeof(Material));
			GetComponent<Renderer>().sharedMaterial = material;
		}
		
		// If the seed hopper is ready, set the screen
		if(depositTime.AddHours(6) < System.DateTime.Now) {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_seedhopper_ready", typeof(Material));
			GetComponent<Renderer>().sharedMaterial = material;
			seedsReady = true;
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
			heldTime = timeToHold;
			gazeIn = false;
			
			menuCanvas.SetActive(true);
			readTime = timeToRead;
			menuActive = true;
		}
		
		// Time out the tutorial text, if active
		if(menuActive && readTime > 0.0f) {
			readTime -= Time.deltaTime;
			
			if(readTime <= 0.0f) {
				menuCanvas.SetActive(false);
				menuActive = false;
			}
		}
		
		// Check the seed hopper's remaining time
		if(stage != 'b' && plantType != -1) {
			if(depositTime.AddHours(6) < System.DateTime.Now) {
				seedsReady = true;
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_seedhopper_ready", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
			}
		}
		
		// Menu Options
		if(menuActive) {			
			MenuControl();
		}
	}
	
	// Handling the menu steps
	public void MenuControl() {
		if(currentMenu == "withdraw") {
			if(seedsReady && plantType >= 0) {
				menuText.text = "\n" + produceType[plantType] + " Seeds have been hulled! Withdraw them?";
				leftText.text = "Withdraw";
				rightText.text = "Not Now";
				exitText.text = "";
			}
			
			if(nextMenu == "left") {
				// Withdraw the seeds
				if(GameControl.control.WithdrawHopper()) {
					plantType = -1;
					depositTime = System.DateTime.Now;
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
				
				if(stage == 'b') {
					exitText.text = "Repair";
				} else if(stage == 'r') {
					exitText.text = "Upgrade";
				} else {
					exitText.text = "Nevermind";
				}
			} else {
				// No actions
			}
			
			produceChoice = 0;
		} else if(currentMenu == "deposit") {
			if(GameControl.control.crops[produceChoice] > 0 && produceChoice < 16 && produceChoice >= 0) {
				menuText.text = "\nYou have: " + GameControl.control.crops[produceChoice].ToString() + 
								" " + produceType[produceChoice];
								
				leftText.text = "Store";
				rightText.text = "Next";
				exitText.text = "Go Back";
			} else {
				produceChoice = (produceChoice + 1) % 16;
			}
			
			if(nextMenu == "left") {
				// Store the seeds
				if(GameControl.control.StoreHopper(produceChoice)) {
					plantType = produceChoice;
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				produceChoice = (produceChoice + 1) % 16;
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
				produceChoice = 0;
			} else {
				// No actions
			}
		} else if(currentMenu == "success") {
			menuText.text = "\nUpdated your inventory.";
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(seedsReady) {
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_seedhopper", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
				seedsReady = false;
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				produceChoice = 0;
			}
		} else if(currentMenu == "repair" && stage == 'b') {
			menuText.text = "\nIt will cost 50,000 gold to repair to convert a crop to two seed packs.\nCurrent Gold: " + GameControl.control.gold.ToString();
			leftText.text = "Repair";
			rightText.text = "No, Thanks!";
			exitText.text = "";
			
			if(nextMenu == "left") {
				if(GameControl.control.gold >= 50000) {
					GameControl.control.freezerStage = 'r';
					GameControl.control.gold -= 50000;
					stage = 'r';
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_seedhopper", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					currentMenu = "successrepair";
					nextMenu = "";
					GameControl.control.UpgradeHopper();
					GameControl.control.Save();
				} else {
					currentMenu = "failrepair";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "repair" && stage == 'r') {
			menuText.text = "\nIt will cost 100,000 gold to upgrade to 3 seed packs.\nCurrent Gold: " + GameControl.control.gold.ToString();
			leftText.text = "Upgrade";
			rightText.text = "No, Thanks!";
			exitText.text = "";
			
			if(nextMenu == "left") {
				if(GameControl.control.gold >= 100000) {
					GameControl.control.freezerStage = 'r';
					GameControl.control.gold -= 100000;
					stage = 'u';
					currentMenu = "successrepair";
					nextMenu = "";
					GameControl.control.UpgradeHopper();
					GameControl.control.Save();
				} else {
					currentMenu = "failrepair";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "successrepair") {
			menuText.text = "\n Alright, big spender!";
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(nextMenu != "") {
				currentMenu = "";
				nextMenu = "";
			}
		} else if(currentMenu == "failrepair") {
			menuText.text = "\nYou only have " + GameControl.control.gold.ToString() + " gold.";
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(nextMenu != "") {
				currentMenu = "";
				nextMenu = "";
			}
		} else {
			menuText.text = "\nWithdraw Seeds or Deposit Crops?";
			leftText.text = "Withdraw";
			rightText.text = "Deposit";
			
			if(stage == 'b') {
				exitText.text = "Repair";
			} else if(stage == 'r') {
				exitText.text = "Upgrade";
			} else {
				exitText.text = "Nevermind";
			}
		}
	}
}
