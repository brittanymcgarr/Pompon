////////////////////////////////////////////////////////////////////////////////
// Freezer.cs                                                                 //
// The freezer allows the player to store out of season fruits and vegetables //
// for use in events. The beginning refrigerator costs nothing and can store  //
// one of each item. The repaired version can store up to 100 of each.        //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Freezer : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public float timeToHold = 1.5f;
	public float timeToRead = 120.0f;
	public float heldTime = 0.0f;
	private float readTime;
	public bool gazeIn = false;
	public bool menuActive = false;
	
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
		leftButton = GameObject.Find("StorageLeftCollider").gameObject;
		leftText = GameObject.Find("StorageLeftOption").GetComponent<Text>();
		
		rightButton = GameObject.Find("StorageRightCollider").gameObject;
		rightText = GameObject.Find("StorageRightOption").GetComponent<Text>();
		
		exitButton = GameObject.Find("StorageExitCollider").gameObject;
		exitText = GameObject.Find("StorageExitOption").GetComponent<Text>();
		
		// Load the MenuCanvas
		menuCanvas = GameObject.Find("StorageMenuCanvas").gameObject;
		menuText = GameObject.Find("StorageMenuText").GetComponent<Text>();
		menuCanvas.transform.SetParent(this.transform, false);
		menuCanvas.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		menuCanvas.SetActive(false);
		
		// Get the current repair stage
		stage = GameControl.control.freezerStage;
		
		// Set the appliance's material
		if(stage == 'r') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_freezer", typeof(Material));
			GetComponent<Renderer>().sharedMaterial = material;
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
		
		// Menu Options
		if(menuActive) {			
			MenuControl();
		}
	}
	
	// Handling the menu steps
	public void MenuControl() {
		if(currentMenu == "withdraw") {
			
			if(GameControl.control.storedCrops[produceChoice] > 0 && produceChoice < 16 && produceChoice >= 0) {
				menuText.text = "\nYou have: " + GameControl.control.storedCrops[produceChoice].ToString() + 
								" " + produceType[produceChoice];
								
				leftText.text = "Withdraw";
				rightText.text = "Next";
				exitText.text = "Nevermind";
			} else {
				produceChoice = (produceChoice + 1) % 16;
			}
			
			if(nextMenu == "left") {
				// Withdraw the seeds
				GameControl.control.WithdrawCrops(produceChoice);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "right") {
				produceChoice = (produceChoice + 1) % 16;
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
				
				if(stage == 'b') {
					exitText.text = "Repair";
				} else {
					exitText.text = "Nevermind";
				}
				
				produceChoice = 0;
			} else {
				// No actions
			}
		} else if(currentMenu == "deposit") {
			menuText.text = "\nStore one crop type or all of them?";
			
			leftText.text = "Each Type";
			rightText.text = "All";
			exitText.text = "Go Back";
			
			if(nextMenu == "left") {
				currentMenu = "store";
				nextMenu = "";
				produceChoice = 0;
			} else if(nextMenu == "right") {
				GameControl.control.StoreCrops(-1);
				currentMenu = "success";
				nextMenu = "";
				produceChoice = 0;
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "store") {
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
				GameControl.control.StoreCrops(produceChoice);
				currentMenu = "success";
				nextMenu = "";
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
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				produceChoice = 0;
			}
		} else if(currentMenu == "repair") {
			menuText.text = "\nIt will cost 20,000 gold to repair.\nCurrent Gold: " + GameControl.control.gold.ToString();
			leftText.text = "Repair";
			rightText.text = "No, Thanks!";
			exitText.text = "";
			
			if(nextMenu == "left") {
				if(GameControl.control.gold >= 20000) {
					GameControl.control.freezerStage = 'r';
					GameControl.control.gold -= 20000;
					stage = 'r';
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_freezer", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					currentMenu = "successrepair";
					nextMenu = "";
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
			menuText.text = "\nWithdraw or Deposit Crops?";
			leftText.text = "Withdraw";
			rightText.text = "Deposit";
			
			if(stage == 'b') {
				exitText.text = "Repair";
			} else {
				exitText.text = "Nevermind";
			}
		}
	}
}
