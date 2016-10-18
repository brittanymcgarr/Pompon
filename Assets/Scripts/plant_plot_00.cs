////////////////////////////////////////////////////////////////////////////////
// plant_plot_00.cs                                                           //
// Handles the fixed-gaze input response when the plant plot is viewed and    //
// held for 2 seconds. Action gets the next state of the plant and updates    //
// with an animation for the player. Stores the plot's current plant info.    //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

// IGvrGazeResponder inheritance handles gaze-driven input
public class plant_plot_00 : MonoBehaviour, IGvrGazeResponder {
	// Public variables
	public Material material;
	public Material seedMaterial;
	public GameObject tool_wateringcan;
	public GameObject tool_seeds;
	public GameObject current_tool;
	public int plotRow = 0;
	public int plotColumn = 0;
	public char growthStage = 'D';
	public string plantType = "";
	public string seedType = "";
	public int seedPos = 0;
	public DateTime plantTime;
	public const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
	public int stage1_seconds = 0;
	public int stage2_seconds = 0;
	public int stage3_seconds = 0;
	
	// Private variables
	private float timeToHold = 1.5f;
	private float animateHold = 2.0f;
	private float heldTime;
	private float animateTime;
	private bool gazeIn = false;
	private bool toolIn = false;
	private bool seedsLoaded = false;
	private bool seedsUsed = false;
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
		
		// Get this plot's numbering
		string parentName = gameObject.transform.name;
		plotRow = parentName[11] - '0';
		plotColumn = parentName[12] - '0';
		
		// Get the values from the GameControl in format:
		// "plant_plot_##,<growth stage character: 'D'irt, 'S'eed, '1' (sprout), '2' (stalk), '3' (harvest)>,
		// <plant type string: "turnip", "carrot", etc.>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
		if(GameControl.control.plots.Length != 0) {
			string saveData = GameControl.control.plots[plotColumn];
			string[] dataTokens = saveData.Split(',');
			saveData = dataTokens[1];
			growthStage = saveData[0];
			plantType = dataTokens[2];
			plantTime = System.DateTime.ParseExact(dataTokens[3], datetimeFormat, CultureInfo.InvariantCulture);
			
			// Set the plant growth values, if matches a type
			if(plantType != "") {
				SetStageTimes();
			}
		} else {
			growthStage = 'D';
			plantType = "";
			plantTime = System.DateTime.Now;
		}
		
		// Create the next action tool set
		tool_wateringcan = Instantiate(tool_wateringcan) as GameObject;
		tool_seeds = Instantiate(Resources.Load("Models/tool_seeds")) as GameObject;
		
		// Handle the current tool set
		int seeds = GameControl.control.seeds[0] + GameControl.control.seeds[1] + GameControl.control.seeds[2];
		if(growthStage == 'D' && seeds > 0) {
			current_tool = tool_seeds;
			seedsLoaded = true;
		} else {
			current_tool = tool_wateringcan;
			seedsLoaded = false;
		}
		
		// Create the material
		if(growthStage == 'S') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_sprout", typeof(Material));
		} else if (growthStage == '1') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_sprout", typeof(Material));
		} else if (growthStage == '2') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_sprout", typeof(Material));
		} else if (growthStage == '3') {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_ready", typeof(Material));
		} else {
			material = (Material)Resources.Load("Models/Materials/Materials/cardboard_dirt", typeof(Material));
		}
		GetComponent<Renderer>().sharedMaterial = material;
		
		// Save the state, just in case
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
			
			if(growthStage == 'D') {
				seedType = "";
				
				for(int index = 0; index < GameControl.control.seeds.Length; index++) {
					if(GameControl.control.seeds[index] > 0) {
						if(index == 0) {
							seedType = "turnip";
						} else if(index == 1) {
							seedType = "carrot";
						} else if(index == 2) {
							seedType = "pumpkin";
						} else if(index == 3) {
							seedType = "broccoli";
						} else if(index == 4) {
							seedType = "tomato";
						} else if(index == 5) {
							seedType = "sugarplum";
						} else if(index == 6) {
							seedType = "leek";
						} else if(index == 7) {
							seedType = "pommegranate";
						} else if(index == 8) {
							seedType = "lettuce";
						} else if(index == 9) {
							seedType = "pea";
						} else if(index == 10) {
							seedType = "cantaloupe";
						} else {
							seedType = "";
						}
						
						seedPos = index;
						index = 1000;
					}
				}
				
				current_tool = tool_seeds;
				seedMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + seedType + "seeds", typeof(Material));
				current_tool.GetComponent<Renderer>().sharedMaterial = seedMaterial;
				seedsLoaded = true;
			}
			
			if(seedType == "") {
				seedsLoaded = false;
				seedsUsed = false;
				current_tool = tool_wateringcan;
			}
			
			if(!toolIn) {
				// STUB: material changing script for the plant plot
				// material = (Material)Resources.Load("Models/Materials/Materials/cardboard_turnip_sprout", typeof(Material));
				// GetComponent<Renderer>().sharedMaterial = material;
				
				// Check for harvest action
				if(harvest) {
					// Set up the animation
					current_tool = Instantiate(Resources.Load("Models/harvest_turnip")) as GameObject;
					seedMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType, typeof(Material));
					current_tool.GetComponent<Renderer>().sharedMaterial = seedMaterial;
					current_tool.SetActive(true);
					current_tool.transform.SetParent(this.transform, false);
					current_tool.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
					Destroy(current_tool, 2.0f);
					Destroy(tool_wateringcan, 1.0f);
					harvest = false;
					
					// Update the crops in the player's inventory
					GameControl.control.crops[seedPos] += 1;
					
					// Replace the state of the plot
					growthStage = 'D';
					seedType = "";
					plantType = "";
					
					// Save and restart the plot
					Save();
					ResetPlot();
				} else {
					// Set the tool as active, move it relative to the parent, and play its animation
					
					current_tool.SetActive(true);
					current_tool.transform.SetParent(this.transform, false);
					current_tool.transform.localPosition = new Vector3(0.0f, 1.0f, 1.0f);
					current_tool.GetComponent<Animation>().Play("tool_wateringcan");
					
					if(seedsLoaded) {
						seedsLoaded = false;
						seedsUsed = true;
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
				toolIn = false;
				
				current_tool.SetActive(false);
				
				// Update the current tool if the state changes
				if(seedsUsed) {
					// Using seeds changes the state to 'S'eed
					seedsUsed = false;
					growthStage = 'S';
					Destroy(current_tool, 1.0f);
					current_tool = tool_wateringcan;
					plantType = seedType;
					SetStageTimes();
					plantTime = System.DateTime.Now;
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_sprout", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					
					// Update the player's inventory
					GameControl.control.seeds[seedPos] -= 1;
					
					// Save the new state
					Save();
				}
			}
		}
		
		// Check for growth stage
		if(growthStage != 'D') {
			if(growthStage == 'S') {
				// Check if seconds to stage 1
				if(plantTime.AddSeconds(stage1_seconds) < System.DateTime.Now) {
					growthStage = '1';
					Save();
				}
			} else if (growthStage == '1') {
				// Check if seconds to stage 2
				if(plantTime.AddSeconds(stage2_seconds) < System.DateTime.Now) {
					growthStage = '2';
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_sprout", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					Save();
				}
			} else if (growthStage == '2') {
				// Check if seconds to stage 3
				if(plantTime.AddSeconds(stage3_seconds) < System.DateTime.Now) {
					growthStage = '3';
					material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_sprout", typeof(Material));
					GetComponent<Renderer>().sharedMaterial = material;
					Save();
				}
			} else if (growthStage == '3') {
				// Harvest item becomes an option
				material = (Material)Resources.Load("Models/Materials/Materials/cardboard_" + plantType + "_ready", typeof(Material));
				GetComponent<Renderer>().sharedMaterial = material;
				
				harvest = true;
			} else {
				// No actions
			}
		}
	}
	
	// Saving on disable (screen shutoff) for auto-saving values
	void OnDisable() {
		Save();
	}
	
	// Saving the data as a string
	void Save() {
		// Save the plot data to GameControl in format:
		// "plant_plot_##,<growth stage character: 'D'irt, 'S'eed, '1' (sprout), '2' (stalk), '3' (harvest)>,
		// <plant type string: "turnip", "carrot", etc.>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
		string saveValues = "plant_plot_" + (char) (plotRow + 48) + (char) (plotColumn + 48) + "," 
							+ growthStage + "," + plantType + ",";
		string dateString = plantTime.ToString(datetimeFormat);
		saveValues = saveValues + dateString;
		
		GameControl.control.plots[plotColumn] = saveValues;
		
		// Save to the main state, too
		GameControl.control.Save();
	}
	
	// Reset data for debugging
	public void ResetPlot() {
		Start();
	}
	
	// Set the staging times for the different plant types
	public void SetStageTimes() {
		if(plantType == "turnip") {
			stage1_seconds = 10;
			stage2_seconds = 20;
			stage3_seconds = 30;
		} else if(plantType == "carrot") {
			stage1_seconds = 60;
			stage2_seconds = 120;
			stage3_seconds = 300;
		} else if(plantType == "pumpkin") {
			stage1_seconds = 10 * 60;
			stage2_seconds = 30 * 60;
			stage3_seconds = 60 * 60;
		} else if(plantType == "broccoli") {
			stage1_seconds = 60;
			stage2_seconds = 180;
			stage3_seconds = 420;
		} else if(plantType == "tomato") {
			stage1_seconds = 5 * 60;
			stage2_seconds = 10 * 60;
			stage3_seconds = 15 * 60;
		} else if(plantType == "sugarplum") {
			stage1_seconds = 3 * 60 * 60;
			stage2_seconds = 12 * 60 * 60;
			stage3_seconds = 24 * 60 * 60;
		} else if(plantType == "leek") {
			stage1_seconds = 15 * 60;
			stage2_seconds = 30 * 60;
			stage3_seconds = 45 * 60;
		} else if(plantType == "pommegranate") {
			stage1_seconds = 60 * 60;
			stage2_seconds = 2 * 60 * 60;
			stage3_seconds = 3 * 60 * 60;
		} else if(plantType == "lettuce") {
			stage1_seconds = 1 * 60;
			stage2_seconds = 3 * 60;
			stage3_seconds = 5 * 60;
		} else if(plantType == "pea") {
			stage1_seconds = 1 * 60;
			stage2_seconds = 3 * 60;
			stage3_seconds = 5 * 60;
		} else if(plantType == "cantaloupe") {
			stage1_seconds = 30 * 60;
			stage2_seconds = 60 * 60;
			stage3_seconds = 90 * 60;
		} else {
			stage1_seconds = 0;
			stage2_seconds = 0;
			stage3_seconds = 0;
		}
	}
}
