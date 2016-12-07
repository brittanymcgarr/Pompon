////////////////////////////////////////////////////////////////////////////////
// Churn.cs                                                                   //
// Collects and parses the save data to determine repair state, progress to   //
// butter collection, and resulting butter.                                   //
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

public class Churn : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public GameObject menuCanvas;
	public Text menuText;
	public Text leftText;
	public Text rightText;
	public Text exitText;
	public string nextMenu = "";
	public string currentMenu = "";
	public GameObject butter;

	// Private variables
	private float timeToHold = 1.5f;
	private float timeToRead = 60.0f;
	private float heldTime;
	private float readTime;
	private float butterIn = -2.0f;
	private bool gazeIn = false;
	private bool menuActive = false;
	private bool butterReady = false;
	private char milk;
	private char stage;
	private DateTime deposit;
	private Material material;

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
		
		// Get the current state of the butter churn, if it has been repaired
		stage = GameControl.control.butterChurn[0];
		if(stage == 'r') {
			string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
			string[] tokens = GameControl.control.butterChurn.Split(',');
			
			milk = tokens[1][0];
			
			if(milk == 'M' || milk == 'G') {
				deposit = System.DateTime.ParseExact(tokens[2], datetimeFormat, CultureInfo.InvariantCulture);
				
				if(deposit.AddHours(6) <= System.DateTime.Now) {
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn_ready", typeof(Material));
					butterReady = true;
					gameObject.GetComponent<Renderer>().sharedMaterial = material;
				} else {
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn", typeof(Material));
					gameObject.GetComponent<Renderer>().sharedMaterial = material;
				}
			} else {
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn", typeof(Material));
				gameObject.GetComponent<Renderer>().sharedMaterial = material;
			}
		}
		
		// Get the menu canvas
		menuCanvas = GameObject.Find("ChurnMenuCanvas").gameObject;
		
		menuText = GameObject.Find("ChurnMenuText").GetComponent<Text>();
		leftText = GameObject.Find("ChurnLeftOption").GetComponent<Text>();
		rightText = GameObject.Find("ChurnRightOption").GetComponent<Text>();
		exitText = GameObject.Find("ChurnExitOption").GetComponent<Text>();
		butter = GameObject.Find("Butter").gameObject;
		
		// Hide the canvas and butter
		menuCanvas.SetActive(false);
		butter.SetActive(false);
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
		
		// Check the churn's remaining time
		if(stage == 'r' && (milk == 'G' || milk == 'M')) {
			if(deposit.AddHours(6) < System.DateTime.Now) {
				butterReady = true;
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn_ready", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
				
				if(GameControl.control.butterChurn[2] == 'G') {
					Material butterMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_butter_good", typeof(Material));
					butter.GetComponent<Renderer>().sharedMaterial = butterMaterial;
				}
			}
		}
		
		if(butterIn >= 0.0) {
			butterIn -= Time.deltaTime;
			
			if(butterIn <= 0.0) {
				butter.SetActive(false);
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
			if(butterReady && (milk == 'G' || milk == 'M')) {
				if(milk == 'G') {
					menuText.text = "\nThe Blue Ribbon Butter is ready! Withdraw?";
				} else {
					menuText.text = "\nThe Butter is ready! Withdraw?";
				}
				
				leftText.text = "Withdraw";
				rightText.text = "Not Now";
			}
			
			if(nextMenu == "left") {
				// Withdraw the butter
				if(deposit.AddHours(6) <= System.DateTime.Now) {
					GameControl.control.WithdrawChurn();
					deposit = System.DateTime.Now;
					currentMenu = "success";
					nextMenu = "";
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					butterReady = false;
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
			} else {
				// No actions
			}
		} else if(currentMenu == "deposit") {
			if(GameControl.control.items[7] > 0 || GameControl.control.items[8] > 0) {
				string menuString = "\nYou have: Milk ";
								
				if(GameControl.control.items[8] > 0) {
					menuString = menuString + "and Good Milk. Which are you depositing?";
				}
				
				menuText.text = menuString;
				leftText.text = "Milk";
				rightText.text = "GoodMilk";
			} else {
				currentMenu = "failure";
				nextMenu = "";
			}
			
			if(nextMenu == "left") {
				// Store the regular milk
				if(GameControl.control.items[7] > 0 && GameControl.control.butterChurn[2] == 'N') {
					GameControl.control.DepositChurn('M');
					currentMenu = "successDeposit";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				if(GameControl.control.items[8] > 0 && GameControl.control.butterChurn[2] == 'N') {
					GameControl.control.DepositChurn('G');
					currentMenu = "successDeposit";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
			} else {
				// No actions
			}
		} else if(currentMenu == "successDeposit") {
			menuText.text = "\nFilled the churn. Come back in 6 hours.";
			leftText.text = "";
			rightText.text = "";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "success") {
			menuText.text = "\nUpdated your inventory. Come back in a while.";
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			butterIn = 2.0f;
			butter.SetActive(true);
			
			if(butterReady) {
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn_ready", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
				butterReady = false;
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "failure") {
			if(stage == 'b') {
				menuText.text = "\nCannot complete action. Repair the machine, first.";
			} else {
				menuText.text = "\nCannot complete action. Load machine and wait 6 hours.";
			}
			
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "repair" && stage == 'b') {
			menuText.text = "\nIt will cost 30,000 gold to repair the butter churn.\nCurrent Gold: " + GameControl.control.gold.ToString();
			leftText.text = "Repair";
			rightText.text = "No, Thanks!";
			exitText.text = "";
			
			if(nextMenu == "left") {
				if(GameControl.control.gold >= 30000) {
					GameControl.control.gold -= 30000;
					stage = 'r';
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_churn", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					currentMenu = "successrepair";
					nextMenu = "";
					GameControl.control.UpgradeChurn();
					GameControl.control.Save();
				} else {
					currentMenu = "failrepair";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "exit") {
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
			menuText.text = "\nWithdraw Butter or Deposit Milk?";
			leftText.text = "Withdraw";
			rightText.text = "Deposit";
			
			if(stage == 'b') {
				exitText.text = "Repair";
			} else {
				exitText.text = "Nevermind";
			}
			
			if(nextMenu == "left") {
				currentMenu = "withdraw";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "deposit";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				if(stage == 'b') {
					currentMenu = "repair";
					nextMenu = "";
				} else {
					currentMenu = "";
					nextMenu = "";
				}
			} else {
			
			}
		}
	}
}
