////////////////////////////////////////////////////////////////////////////////
// People_Lily.cs                                                             //
// Lily, the flower cart girl, interacts with the character to deal in seeds  //
// and produce. She is very friendly and helpful.                             //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class People_Lily : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public bool tutorialON = true;
	public bool progressed = false;
	public GameObject speechBubble;
	public GameObject menuCanvas;
	public Text text;
	public Text menuText;
	public Text leftText;
	public Text rightText;
	public bool menuActive = false;
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
	public string currentSeeds;
	public string seasonalSeeds;
	public string currentPrice;
	public string seasonalPrice;
	
	// Private variables
	private float timeToHold = 1.5f;
	private float timeToEmote = 3.0f;
	private float timeToRead = 4.0f;
	private float heldTime;
	private float emoteTime;
	private float readTime;
	private Material material;
	private bool gazeIn = false;
	private bool animateIn = false;
	private string[] tutorialText;
	private int tutorialCount = 0;

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
		
		// Load the happy material for Lily
		material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily_happy", typeof(Material));
		
		// Load the SpeechCanvas
		speechBubble = GameObject.Find("SpeechCanvas").gameObject;
		text = GameObject.Find("SpeechText").GetComponent<Text>();
		speechBubble.transform.SetParent(this.transform, false);
		speechBubble.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		speechBubble.SetActive(false);
		
		// Load the buttons and scripts
		leftButton = GameObject.Find("LeftCollider").gameObject;
		leftText = GameObject.Find("LeftOption").GetComponent<Text>();
		leftScript = leftButton.GetComponent<LeftButton>();
		
		rightButton = GameObject.Find("RightCollider").gameObject;
		rightText = GameObject.Find("RightOption").GetComponent<Text>();
		rightScript = rightButton.GetComponent<RightButton>();
		
		exitButton = GameObject.Find("ExitCollider").gameObject;
		exitScript = exitButton.GetComponent<ExitButton>();
		
		// Check the season's veggie
		GetSeeds();
		
		plainMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard-textures-5", typeof(Material));
		
		// Load the MenuCanvas
		menuCanvas = GameObject.Find("MenuCanvas").gameObject;
		menuText = GameObject.Find("MenuText").GetComponent<Text>();
		menuCanvas.transform.SetParent(this.transform, false);
		menuCanvas.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		menuCanvas.SetActive(false);
		
		// Check for story progressions
		tutorialON = GameControl.control.tutorial;
		
		// If the tutorial is checked, load the speech strings
		if(tutorialON) {
			tutorialText = new string[16];
			
			tutorialText[0] = "Hello! Welcome to the village of Pompon.";
			tutorialText[1] = "My name is Lily. I'm the local merchant.";
			tutorialText[2] = "You must be the new farmer.";
			tutorialText[3] = "I have an order for turnips. Could you help me out?";
			tutorialText[4] = "Take the seeds and head down the road to the field.";
			tutorialText[5] = "The fields were already tilled.";
			tutorialText[6] = "You can just inspect the ground and plant them.";
			tutorialText[7] = "Give the seeds some water by inspecting the ground.";
			tutorialText[8] = "Once the plant has matured, view it to harvest.";
			tutorialText[9] = "Come back to me when you have grown 3 turnips.";
			tutorialText[10] = "Thank you! I really needed those for my order.";
			tutorialText[11] = "Farming isn't so difficult, but I'm so busy.";
			tutorialText[12] = "I would love to start selling some fresh produce.";
			tutorialText[13] = "Go ahead and take these seeds as thanks.";
			tutorialText[14] = "Come see me when you grow more crops or find stuff.";
			tutorialText[15] = "I'll also start carrying some more seed types.";
		} else {
			menuActive = true;
			timeToRead = 60.0f;
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
			
			// Animate Lily's smile. She's a happy person.
			GetComponent<Renderer>().sharedMaterial = material;
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily", typeof(Material));
			
			emoteTime = timeToEmote;
			animateIn = true;
			
			// Check for the intro tutorial
			if(tutorialON) {
				speechBubble.SetActive(true);
				readTime = timeToRead;
				
				if(tutorialCount == 10) {
					if(GameControl.control.crops[0] >= 3) {
						text.text = tutorialText[tutorialCount];
						GameControl.control.BuyCrops(0);
						
						// Give reward seeds of turnip, carrot, broccoli, and tomato
						GameControl.control.seeds[0] += 1;
						GameControl.control.seeds[1] += 1;
						GameControl.control.seeds[3] += 1;
						GameControl.control.seeds[4] += 1;
						
						tutorialCount += 1;
						GameControl.control.tutorial = false;
						GameControl.control.Save();
					}
				} else {
					text.text = tutorialText[tutorialCount];
					tutorialCount += 1;
					if(tutorialCount > 14) {
						tutorialON = false;
						menuActive = true;
						timeToRead = 60.0f;
						speechBubble.SetActive(false);
						GameControl.control.tutorial = false;
						GameControl.control.Save();
					}
				}
			} else {
				menuCanvas.SetActive(true);
				readTime = timeToRead;
				menuActive = true;
			}
		}
		
		// Time out the emotion animation
		if(animateIn) {
			emoteTime -= Time.deltaTime;
			
			if(emoteTime <= 0.0f) {
				GetComponent<Renderer>().sharedMaterial = material;
				animateIn = false;
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_lily_happy", typeof(Material));
			}
		}
		
		// Time out the tutorial text, if active
		if(readTime > 0.0f) {
			readTime -= Time.deltaTime;
			
			if(readTime <= 0.0f) {
				if(menuActive) {
					menuCanvas.SetActive(false);
					menuActive = false;
				} else {
					speechBubble.SetActive(false);
				}
			}
		}
		
		// Menu Options
		if(menuActive) {			
			MenuControl();
		}
	}
	
	// Get the seasonal veggie seeds
	public void GetSeeds() {
		string todayMonth = GameControl.control.todayMonth;

		if(todayMonth == "Dec" || todayMonth == "Jan" || todayMonth == "Feb") {
			// Winter Seasonals
			// Regular Crop: Turnip
			leftMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_turnipseeds", typeof(Material));
			currentSeeds = "Turnip";
			currentPrice = "100";
			
			// Check for monthlies
			if(todayMonth == "Dec") {
				// December Sugarplums
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_sugarplumseeds", typeof(Material));
				seasonalSeeds = "Sugarplum";
				seasonalPrice = "1000";
			} else if(todayMonth == "Jan") {
				// January Leek
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_leekseeds", typeof(Material));
				seasonalSeeds = "Leek";
				seasonalPrice = "300";
			} else {
				// February Pommegranate
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_pommegranateseeds", typeof(Material));
				seasonalSeeds = "Pommegranate";
				seasonalPrice = "400";
			}
		} else if(todayMonth == "Mar" || todayMonth == "Apr" || todayMonth == "May") {
			// Spring Seasonals
			// Regular Crop: Broccoli
			leftMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_broccoliseeds", typeof(Material));
			currentSeeds = "Broccoli";
			currentPrice = "100";
			
			// Check for monthlies
			if(todayMonth == "Mar") {
				// March Lettuce
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_lettuceseeds", typeof(Material));
				seasonalSeeds = "Lettuce";
				seasonalPrice = "100";
			} else if(todayMonth == "Apr") {
				// April Peas
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_peaseeds", typeof(Material));
				seasonalSeeds = "Pea";
				seasonalPrice = "140";
			} else {
				// May Cantaloupe
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_cantaloupeseeds", typeof(Material));
				seasonalSeeds = "Cantaloupe";
				seasonalPrice = "300";
			}
		} else if(todayMonth == "Jun" || todayMonth == "Jul" || todayMonth == "Aug") {
			// Summer Seasonals
			// Regular Crop: Tomato
			leftMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_tomatoseeds", typeof(Material));
			currentSeeds = "Tomato";
			currentPrice = "100";
			
			// Check for monthlies
			if(todayMonth == "Jun") {
				// June Grapes
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_grapeseeds", typeof(Material));
				seasonalSeeds = "Grapes";
				seasonalPrice = "500";
			} else if(todayMonth == "Jul") {
				// July Watermelon
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_watermelonseeds", typeof(Material));
				seasonalSeeds = "Watermelon";
				seasonalPrice = "600";
			} else {
				// August Strawberries
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_strawberryseeds", typeof(Material));
				seasonalSeeds = "Strawberry";
				seasonalPrice = "300";
			}
		} else {
			// Fall Seasonals
			// Regular Crop: Carrot
			leftMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_carrotseeds", typeof(Material));
			currentSeeds = "Carrot";
			currentPrice = "120";
			
			// Check for monthlies
			if(todayMonth == "Sep") {
				// September Sweet Potato
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_sweetpotatoseeds", typeof(Material));
				seasonalSeeds = "Sweet Potato";
				seasonalPrice = "120";
			} else if(todayMonth == "Oct") {
				// October Pumpkins
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_pumpkinseeds", typeof(Material));
				seasonalSeeds = "Pumpkin";
				seasonalPrice = "350";
			} else {
				// November Cranberry
				rightMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_cranberryseeds", typeof(Material));
				seasonalSeeds = "Cranberry";
				seasonalPrice = "200";
			}
		}
	}
	
	// Handling the menu steps
	public void MenuControl() {
		if(currentMenu == "seeds") {
			menuText.text = "\nYou want to buy some seeds? Wonderful!";
			
			// Load the seed menu options
			leftButton.GetComponent<Renderer>().sharedMaterial = leftMaterial;
			rightButton.GetComponent<Renderer>().sharedMaterial = rightMaterial;
			
			// Hide the text options
			leftText.text = "";
			rightText.text = "";
			
			if(nextMenu == "left") {
				// Buy left option
				currentMenu = "leftsell";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "rightsell";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
				leftButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
				rightButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
				leftText.text = "Buy Seeds";
				rightText.text = "Sell Items";
			} else {
				// No actions
			}
		} else if(currentMenu == "sell") {
			menuText.text = "\nDo you want to sell a few crops or everything?";
			
			leftText.text = "Sell Items";
			rightText.text = "Sell All";
			
			if(nextMenu == "left") {
				currentMenu = "sellmenu";
				nextMenu = "";
			} else if(nextMenu == "right") {
				GameControl.control.BuyInventory();
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "sellmenu") {
			menuText.text = "\n\nCrops or items?";
			
			leftText.text = "Crops";
			rightText.text = "Items";
			
			if(nextMenu == "left") {
				currentMenu = "sellcrops";
				nextMenu = "";
			} else if(nextMenu == "right") {
				currentMenu = "sellitems";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "sellcrops") {
			menuText.text = "\nI can buy all of them or keep one of each for your collection.";
			
			leftText.text = "Buy All";
			rightText.text = "Keep One";
			
			if(nextMenu == "left") {
				GameControl.control.BuyCrops(0);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "right") {
				GameControl.control.BuyCrops(1);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "sellitems") {
			menuText.text = "\nI can buy all of them or keep one of each for your collection.";
			
			leftText.text = "Buy All";
			rightText.text = "Keep One";
			
			if(nextMenu == "left") {
				GameControl.control.BuyItems(0);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "right") {
				GameControl.control.BuyItems(1);
				currentMenu = "success";
				nextMenu = "";
			} else if(nextMenu == "exit") {
				currentMenu = "";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "success") {
			// Congratulate player on sale and redirect to the seed menu
			menuText.text = "\n\nHere you go. Have a great day!";
			leftText.text = "";
			rightText.text = "";
			leftButton.SetActive(false);
			rightButton.SetActive(false);

			if(nextMenu == "exit") {
				nextMenu = "";
				currentMenu = "";
				leftButton.SetActive(true);
				rightButton.SetActive(true);
			}
		} else if(currentMenu == "failure") {
			// Inform player of insufficient funds and redirect to seed menu
			menuText.text = "\n\nI'm sorry. You don't have enough money.";
			leftText.text = "";
			rightText.text = "";
			leftButton.SetActive(false);
			rightButton.SetActive(false);
			
			if(nextMenu == "exit") {
				nextMenu = "";
				currentMenu = "";
				leftButton.SetActive(true);
				rightButton.SetActive(true);
			}
		} else if(currentMenu == "leftsell") {
			menuText.text = "\n" + currentSeeds + " are " + currentPrice + " gold. How many do you want?";
			
			// Buy left option
			leftButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
			rightButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
			leftText.text = "Buy One";
			rightText.text = "Buy Three";
			
			if(nextMenu == "left") {
				if(GameControl.control.BuySeeds(1)) {
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				if(GameControl.control.BuySeeds(3)) {
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "exit") {
				currentMenu = "seeds";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "rightsell") {
			menuText.text = "\nThis month's special seeds are " + seasonalSeeds + "! for " + seasonalPrice + " gold";
			
			// Buy right option
			leftButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
			rightButton.GetComponent<Renderer>().sharedMaterial = plainMaterial;
			leftText.text = "Buy One";
			rightText.text = "Buy Three";
			
			if(nextMenu == "left") {
				if(GameControl.control.BuySeasonal(1)) {
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "right") {
				if(GameControl.control.BuySeasonal(3)) {
					currentMenu = "success";
					nextMenu = "";
				} else {
					currentMenu = "failure";
					nextMenu = "";
				}
			} else if(nextMenu == "exit") {
				currentMenu = "seeds";
				nextMenu = "";
			} else {
			
			}
		} else if(currentMenu == "") {
			menuText.text = "\n\nGood Day! You have " + (GameControl.control.gold).ToString() + " gold.";
			leftText.text = "Buy Seeds";
			rightText.text = "Sell Items";
			leftButton.SetActive(true);
			rightButton.SetActive(true);
		} else {
			// No changes
		}
	}
}
