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
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class People_Kit : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public GameObject menuCanvas;
	public Text menuText;
	public Text leftText;
	public Text rightText;
	public Text exitText;
	public string nextMenu = "";
	public string currentMenu = "";
	public int animalChoice = 0;
	public string[] animals = new string[8];

	// Private variables
	private float timeToHold = 1.5f;
	private float timeToRead = 60.0f;
	private float heldTime;
	private float readTime;
	private bool gazeIn = false;
	private bool menuActive = false;

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
		
		// Load the buttons and scripts
		leftText = GameObject.Find("KitLeftOption").GetComponent<Text>();
		rightText = GameObject.Find("KitRightOption").GetComponent<Text>();
		exitText = GameObject.Find("KitExitOption").GetComponent<Text>();
		
		// Load the SpeechCanvas
		menuCanvas = GameObject.Find("KitMenuCanvas").gameObject;
		menuText = GameObject.Find("KitMenuText").GetComponent<Text>();
		menuCanvas.transform.SetParent(this.transform, false);
		menuCanvas.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		menuCanvas.SetActive(false);
		
		animalChoice = 0;
		animals[0] = "first";
		animals[1] = "second";
		animals[2] = "third";
		animals[3] = "fourth";
		animals[4] = "fifth";
		animals[5] = "sixth";
		animals[6] = "seventh";
		animals[7] = "eigth";
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
		if(readTime > 0.0f) {
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
		if(currentMenu == "animals") {
			menuText.text = "\nAre you buying or selling some animals today?";
			leftText.text = "Buy";
			rightText.text = "Sell";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left") {
				currentMenu = "buyAnimal";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "sellAnimal";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "buyAnimal") {
			menuText.text = "\nDid you want to buy a Chicken or a Cow?";
			leftText.text = "Chicken";
			rightText.text = "Cow";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left") {
				currentMenu = "buyChicken";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "buyCow";
				nextMenu = "";
			} else {
				
			}
		} else if(currentMenu == "sellAnimal") {
			menuText.text = "\nDid you want to sell a Chicken or a Cow?";
			leftText.text = "Chicken";
			rightText.text = "Cow";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left") {
				currentMenu = "sellChicken";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "sellCow";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "buyChicken") {
			while(animalChoice < 8 && GameControl.control.chickens[animalChoice][7] != 'E') {
				animalChoice += 1;
			}
			
			if(animalChoice < 8) {
				menuText.text = "\nI can sell you a rooster or a hen for 4000g.";
				leftText.text = "Rooster";
				rightText.text = "Hen";
			} else {
				currentMenu = "buyFail";
				nextMenu = "";
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left" && animalChoice < 8) {
				if(GameControl.control.gold >= 4000) {
					currentMenu = "success";
					nextMenu = "";
					
					string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
					DateTime nowTime = System.DateTime.Now;
					string chickTime = nowTime.ToString(datetimeFormat);
					
					GameControl.control.gold -= 4000;
					GameControl.control.chickens[animalChoice] = "Nest0" + (char)(animalChoice + 48) + ",A,M," + chickTime + ",0,N,0," + chickTime;
					GameControl.control.Save();
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right" && animalChoice < 8) {
				if(GameControl.control.gold >= 4000) {
					currentMenu = "success";
					nextMenu = "";
					
					string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
					DateTime nowTime = System.DateTime.Now;
					string chickTime = nowTime.ToString(datetimeFormat);
					
					GameControl.control.gold -= 4000;
					GameControl.control.chickens[animalChoice] = "Nest0" + (char)(animalChoice + 48) + ",A,F," + chickTime + ",0,N,0," + chickTime;
					GameControl.control.Save();
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else {
			
			}
		} else if(currentMenu == "buyCow") {
			while(animalChoice < 5 && GameControl.control.cows[animalChoice][5] != 'E') {
				animalChoice += 1;
			}
			
			if(animalChoice < 5) {
				menuText.text = "\nI can sell you a cow for 10,000g.";
				leftText.text = "Sure";
				rightText.text = "No, Thanks";
			} else {
				currentMenu = "buyFail";
				nextMenu = "";
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left" && animalChoice < 5) {
				if(GameControl.control.gold >= 10000) {
					currentMenu = "success";
					nextMenu = "";
					
					string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
					DateTime nowTime = System.DateTime.Now;
					string buyTime = nowTime.ToString(datetimeFormat);
					string fodderTime = nowTime.AddDays(-1).ToString(datetimeFormat);
					
					GameControl.control.gold -= 10000;
					GameControl.control.cows[animalChoice] = "Cow" + (char)(animalChoice + 48) + ",A," + buyTime + ",0,N,0," + buyTime + "," + fodderTime;
					GameControl.control.Save();
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
				animalChoice = 0;
			} else {
			
			}
		} else if(currentMenu == "sellChicken") {
			while(animalChoice < 8 && GameControl.control.chickens[animalChoice][7] != 'A') {
				animalChoice += 1;
			}
			
			if(animalChoice < 8) {
				menuText.text = "\nI can buy the " + animals[animalChoice] + " adult chicken for 3000g";
				leftText.text = "Sure!";
				rightText.text = "Next One";
			} else {
				currentMenu = "sellFail";
				nextMenu = "";
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left" && animalChoice < 8) {
				currentMenu = "success";
				nextMenu = "";
				
				// Update the chickens
				GameControl.control.gold += 3000;
				
				string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
				DateTime nowTime = System.DateTime.Now;
				string chickTime = nowTime.ToString(datetimeFormat);
				
				GameControl.control.chickens[animalChoice] = "Nest0" + (char)(animalChoice + 48) + ",E,N," + chickTime + ",0,N,0," + chickTime;
				GameControl.control.Save();
			} else if(nextMenu == "right") {
				animalChoice += 1 % 8;
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "sellCow") {
			while(animalChoice < 5 && GameControl.control.cows[animalChoice][5] != 'A') {
				animalChoice += 1;
			}
			
			if(animalChoice < 5) {
				menuText.text = "\nI can buy the " + animals[animalChoice] + " cow for 9000g";
				leftText.text = "Sure!";
				rightText.text = "Next One";
			} else {
				currentMenu = "sellFail";
				nextMenu = "";
			}
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "left" && animalChoice < 5) {
				currentMenu = "success";
				nextMenu = "";
				
				// Update the cows
				GameControl.control.gold += 9000;
				
				string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
				DateTime nowTime = System.DateTime.Now;
				string buyTime = nowTime.ToString(datetimeFormat);
				string fodderTime = nowTime.AddDays(-1).ToString(datetimeFormat);
				
				GameControl.control.cows[animalChoice] = "Cow" + (char)(animalChoice + 48) + ",E," + buyTime + ",0,N,0," + buyTime + "," + fodderTime;
				GameControl.control.Save();
			} else if(nextMenu == "right") {
				animalChoice += 1 % 5;
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "sellFail") {
			menuText.text = "\nI can\'t seem to see any adult animals to buy from you. Sorry.";
			leftText.text = "";
			rightText.text = "";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				animalChoice = 0;
			}
		} else if(currentMenu == "buyFail") {
			menuText.text = "I don\'t see a free space for your purchase. Sorry.";
			leftText.text = "";
			rightText.text = "";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			}
		} else if(currentMenu == "plots") {
			if(GameControl.control.plotUpgrade1 && GameControl.control.plotUpgrade2 && GameControl.control.plotUpgrade3) {
				menuText.text = "\nYour field is fully upgraded. Congrats!";
				leftText.text = "";
				rightText.text = "";
				exitText.text = "All Right!";
			} else {
				menuText.text = "I can plow another row for planting for 10,000g. You have: " + GameControl.control.gold.ToString();
				leftText.text = "Yes!";
				rightText.text = "No, thanks";
				exitText.text = "Nevermind";
			}
			
			if(nextMenu == "left") {
				if(GameControl.control.plotUpgrade1 && GameControl.control.plotUpgrade2 && GameControl.control.plotUpgrade3) {
					nextMenu = "";
					currentMenu = "";
				} else {
					if(GameControl.control.gold >= 10000 && !GameControl.control.plotUpgrade1) {
						GameControl.control.gold -= 10000;
						GameControl.control.plotUpgrade1 = true;
						currentMenu = "success";
						nextMenu = "";
						GameControl.control.Save();
					} else if(GameControl.control.gold >= 10000 && !GameControl.control.plotUpgrade2) {
						GameControl.control.gold -= 10000;
						GameControl.control.plotUpgrade2 = true;
						currentMenu = "success";
						nextMenu = "";
						GameControl.control.Save();
					} else if(GameControl.control.gold >= 10000 && !GameControl.control.plotUpgrade3) {
						GameControl.control.gold -= 10000;
						GameControl.control.plotUpgrade3 = true;
						currentMenu = "success";
						nextMenu = "";
						GameControl.control.Save();
					} else {
						currentMenu = "failure";
						nextMenu = "";
					}
				}
			} else if(nextMenu == "right") {
				currentMenu = "";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
				// No action
			}
		} else if(currentMenu == "success") {
			menuText.text = "\nI'll get right to it!";
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				animalChoice = 0;
			}
		} else if(currentMenu == "failure") {
			menuText.text = "\nSorry. Check if you have enough gold. You have: " + GameControl.control.gold.ToString();
			leftText.text = "";
			rightText.text = "";
			exitText.text = "Thanks";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				animalChoice = 0;
			}
		} else {
			menuText.text = "\nHello! What can I help you with?";
			leftText.text = "Animals";
			rightText.text = "Land";
			exitText.text = "Nevermind";
			
			if(nextMenu == "left") {
				currentMenu = "animals";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "plots";
				nextMenu = "";
			} else {
				currentMenu = "";
				nextMenu = "";
			}
		}
	}
}
