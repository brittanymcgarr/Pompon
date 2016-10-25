////////////////////////////////////////////////////////////////////////////////
// SeedCrate.cs                                                               //
// The Seed Crate allows the player to deposit, store, and withdraw the seeds //
// they wish to use on the fields.                                            //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SeedCrate : MonoBehaviour, IGvrGazeResponder {
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
	public string nextMenu = "";
	public string currentMenu = "";
	public GameObject leftButton;
	public LeftButton leftScript;
	public Material leftMaterial;
	public GameObject rightButton;
	public RightButton rightScript;
	public Material rightMaterial;
	public GameObject exitButton;
	public ExitButton exitScript;
	public Material plainMaterial;
	public int seedChoice = 0;
	
	public string[] seedNames;
	

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
		seedNames = new string[16];
		seedNames[0] = "Turnip";
		seedNames[1] = "Carrot";
		seedNames[2] = "Pumpkin";
		seedNames[3] = "Broccoli";
		seedNames[4] = "Tomato";
		seedNames[5] = "Sugarplum";
		seedNames[6] = "Leek";
		seedNames[7] = "Pommegranate";
		seedNames[8] = "Lettuce";
		seedNames[9] = "Pea";
		seedNames[10] = "Cantaloupe";
		seedNames[11] = "Grapes";
		seedNames[12] = "Watermelon";
		seedNames[13] = "Strawberry";
		seedNames[14] = "Sweet Potato";
		seedNames[15] = "Cranberry";
		
		// Load the buttons and scripts
		leftButton = GameObject.Find("SeedLeftCollider").gameObject;
		leftText = GameObject.Find("SeedLeftOption").GetComponent<Text>();
		leftScript = leftButton.GetComponent<LeftButton>();
		
		rightButton = GameObject.Find("SeedRightCollider").gameObject;
		rightText = GameObject.Find("SeedRightOption").GetComponent<Text>();
		rightScript = rightButton.GetComponent<RightButton>();
		
		exitButton = GameObject.Find("SeedExitCollider").gameObject;
		exitScript = exitButton.GetComponent<ExitButton>();
		
		plainMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard-textures-5", typeof(Material));
		
		// Load the MenuCanvas
		menuCanvas = GameObject.Find("SeedMenuCanvas").gameObject;
		menuText = GameObject.Find("SeedMenuText").GetComponent<Text>();
		menuCanvas.transform.SetParent(this.transform, false);
		menuCanvas.transform.localPosition = new Vector3(0.0f, 1.69f, 0.0f);
		menuCanvas.SetActive(false);
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
			
			if(GameControl.control.storedSeeds[seedChoice] > 0 && seedChoice < 16 && seedChoice >= 0) {
				menuText.text = "\nYou have: " + GameControl.control.storedSeeds[seedChoice].ToString() + 
								" " + seedNames[seedChoice] + " seeds";
								
				leftText.text = "Withdraw";
				rightText.text = "Next";
			} else {
				seedChoice = (seedChoice + 1) % 16;
			}
			
			if(nextMenu == "left") {
				// Withdraw the seeds
				GameControl.control.WithdrawSeeds(seedChoice);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "right") {
				seedChoice = (seedChoice + 1) % 16;
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
				seedChoice = 0;
			} else {
				// No actions
			}
		} else if(currentMenu == "deposit") {
			menuText.text = "\nStore one seed type or all of them?";
			
			leftText.text = "Each Type";
			rightText.text = "All";
			
			if(nextMenu == "left") {
				currentMenu = "store";
				nextMenu = "";
				seedChoice = 0;
			} else if(nextMenu == "right") {
				GameControl.control.StoreSeeds(-1);
				currentMenu = "success";
				nextMenu = "";
				seedChoice = 0;
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "store") {
			if(GameControl.control.seeds[seedChoice] > 0 && seedChoice < 16 && seedChoice >= 0) {
				menuText.text = "\nYou have: " + GameControl.control.seeds[seedChoice].ToString() + 
								" " + seedNames[seedChoice] + " seeds";
								
				leftText.text = "Store";
				rightText.text = "Next";
			} else {
				seedChoice = (seedChoice + 1) % 16;
			}
			
			if(nextMenu == "left") {
				// Store the seeds
				GameControl.control.StoreSeeds(seedChoice);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "right") {
				seedChoice = (seedChoice + 1) % 16;
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftText.text = "Withdraw";
				rightText.text = "Deposit";
				seedChoice = 0;
			} else {
				// No actions
			}
		} else if(currentMenu == "success") {
			menuText.text = "\nUpdated your inventory.";
			leftText.text = "";
			rightText.text = "";
			
			if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				seedChoice = 0;
			}
		} else {
			menuText.text = "\nWithdraw or Deposit Seeds?";
			leftText.text = "Withdraw";
			rightText.text = "Deposit";
		}
	}
}
