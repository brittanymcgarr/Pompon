//////////////////////////////////////////////////////////////////////////////////////////////
// GameControl																				//
// Object for storing encoded persistent data (as opposed to PlayerPrefs					//
// which is plaintext). This object can be called from any component 						//
// in its context scene by:																	//
// GameControl.control.gold = value;														//
// It's just that easy!																		//
// Courtesy of Mike Geig																	//
// https://unity3d.com/learn/tutorials/topics/scripting/persistence-saving-and-loading-data //
//////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {
    // Make this a singleton class by storing a reference
	public static GameControl control;

	// Variables
	public static int CROPS = 11;
	public int seedPos = 0;
	public int seedPrice = 100;
	public int seasonalPos = 0;
	public int seasonalPrice = 200;
	
	// Persistent data to store
	public int gold;
	public string[] plots = new string[8];
	public int[] seeds = new int[CROPS];
	public int[] crops = new int[CROPS];
	public int[] items = new int[5];
	public DateTime crabGift;
	public int crabNumber = 0;
	public DateTime acornsDrop;
	public int acornsNumber = 0;
	public DateTime mushroomDrop;
	public int mushroomsNumber = 0;
	public DateTime flowerDrop;
	public int flowersNumber = 0;
	public bool tutorial = true;
	public string todayMonth;
	
	// Awake comes before Start
	void Awake () {
		// Check for an instance of the GameControl already loaded
		// If it has not been loaded, store this instance
		if(control == null) {
			DontDestroyOnLoad(gameObject);
			control = this;
			
			DateTime now = DateTime.Now;
			todayMonth = now.ToString("MMM");
			
			// Initialize the seeds
			SetupSeeds();
		} else {
		// Otherwise, destroy the calling object. It has already been made.
			Destroy(gameObject);
		}
	}
	
	// Loading on enable to automatically load values at runtime
	void OnEnable() {
		Load();
	}
	
	// Saving on disable (screen shutoff) for auto-saving values
	void OnDisable() {
		Save();
	}
	
	// Saving from other scripts
	public void Save() {
		// Create the binary file and path name
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		
		// Create the player data object to serialize and save
		PlayerData data = new PlayerData();
		
		data.gold = gold;
		data.plots = plots;
		data.seeds = seeds;
		data.crops = crops;
		data.items = items;
		data.crabGift = crabGift;
		data.crabNumber = crabNumber;
		data.acornsDrop = acornsDrop;
		data.acornsNumber = acornsNumber;
		data.mushroomDrop = mushroomDrop;
		data.mushroomsNumber = mushroomsNumber;
		data.flowerDrop = flowerDrop;
		data.flowersNumber = flowersNumber;
		data.tutorial = tutorial;
		
		// Serialize it
		bf.Serialize(file, data);
		
		// Close the file
		file.Close();
	}
	
	// Loading from other scripts
	public void Load() {
		if(File.Exists(Application.persistentDataPath + "/playerInfo.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
			
			// Pull out the data from the file and deserialize it as a PlayerData
			PlayerData data = (PlayerData) bf.Deserialize(file);
			
			// Close the file
			file.Close();
			
			// Set that data
			gold = data.gold;
			plots = data.plots;
			seeds = data.seeds;
			crops = data.crops;
			items = data.items;
			crabGift = data.crabGift;
			crabNumber = data.crabNumber;
			acornsDrop = data.acornsDrop;
			acornsNumber = data.acornsNumber;
			mushroomDrop = data.mushroomDrop;
			mushroomsNumber = data.mushroomsNumber;
			flowerDrop = data.flowerDrop;
			flowersNumber = data.flowersNumber;
			tutorial = data.tutorial;
		} else {
			// Save the new game
			Save();
		}
	}
	
	// Erase the game file and load a new set of data
	public void EraseSave() {
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		plots = new string[8];
		seeds = new int[CROPS];
		crops = new int[CROPS];
		items = new int[5];
		
		for(int index = 0; index < 8; index++) {
			plots[index] = "plant_plot_0" + (char)(index + 48) + "," + 'D' + ",dirt," + datenow;
		}
		
		for(int index = 0; index < CROPS; index++) {
			seeds[index] = 0;
		}
		
		for(int index = 0; index < CROPS; index++) {
			crops[index] = 0;
		}
		
		for(int index = 0; index < 5; index++) {
			items[index] = 0;
		}
		
		// Give the player some turnip seeds and 100 gold
		gold = 100;
		seeds[0] = 3;
		tutorial = true;
		
		// Reset their gifting times
		crabGift = System.DateTime.Now;
		crabGift = crabGift.AddDays(-1);
		acornsDrop = System.DateTime.Now;
		acornsDrop = acornsDrop.AddDays(-1);
		mushroomDrop = System.DateTime.Now;
		mushroomDrop = mushroomDrop.AddDays(-1);
		flowerDrop = System.DateTime.Now;
		flowerDrop = flowerDrop.AddDays(-1);
		
		crabNumber = acornsNumber = mushroomsNumber = flowersNumber = 0;
		
		// Delete the old data
		File.Delete(Application.persistentDataPath + "/playerInfo.dat");
		
		// Reset the plant plots
		GameObject plantplot;
		plant_plot_00 plotScript;
		
		for(int plot = 0; plot < 8; plot++) {
			plantplot = GameObject.Find("Terrain/Field/plant_plot_0" + (char)(plot + 48));
			plotScript = plantplot.GetComponent<plant_plot_00>();
			plotScript.ResetPlot();
		}
		
		// Save the game, now
		Save();
	}
	
	// Update the GUI display
	void OnGUI () {
		// GUI.Label(new Rect(10, 10, 100, 30), "Gold: " + gold);
	}
	
	// Collectables
	// Gift from the crab, once a day
	public void getCrabGift() {
		if(crabGift.AddDays(1) <= System.DateTime.Now) {
			crabGift = System.DateTime.Now;
			crabNumber = 1;
			
			// Update the scallop count
			items[2] += 1;
		}
	}
	
	// Get up to 10 acorns from random trees
	public bool getAcorn() {
		if(acornsNumber < 10) {
			if(acornsNumber == 0) {
				acornsDrop = System.DateTime.Now;
			}
			
			System.Random random = new System.Random();
			int randomNumber = random.Next(1,4);
			if(randomNumber % 2 == 0) {
				acornsNumber += 1;
				items[0] += 1;
				
				return true;
			}
		}
		
		if(acornsNumber == 10 || acornsDrop.AddDays(1) <= System.DateTime.Now) {
			if(acornsDrop.AddDays(1) <= System.DateTime.Now) {
				acornsNumber = 0;
			}
		}
		
		return false;
	}
	
	// Get up to two mushrooms a day
	public bool getMushroom() {
		if(mushroomsNumber < 2) {
			if(mushroomsNumber == 0) {
				mushroomDrop = System.DateTime.Now;
			}
			
			mushroomsNumber += 1;
			items[1] += 1;
			
			return true;
		}
		
		if(mushroomsNumber == 2 || mushroomDrop.AddDays(1) <= System.DateTime.Now) {
			if(mushroomDrop.AddDays(1) <= System.DateTime.Now) {
				mushroomsNumber = 0;
			}
		}
		
		return false;
	}
	
	// Setup Monthly and seasonal seeds
	public void SetupSeeds() {
		if(todayMonth == "Dec" || todayMonth == "Jan" || todayMonth == "Feb") {
			// Winter Seasonals
			// Regular Crop: Turnip
			seedPos = 0;
			seedPrice = 100;
			
			// Check for monthlies
			if(todayMonth == "Dec") {
				// December Sugarplums 1000g seed 10000g sell
				seasonalPos = 5;
				seasonalPrice = 1000;
			} else if(todayMonth == "Jan") {
				// January Leeks buy 300g seed 450g sell
				seasonalPos = 6;
				seasonalPrice = 300;
			} else {
				// February Pommegranates 400g seed 1500g sell
				seasonalPos = 7;
				seasonalPrice = 400;
			}
		} else if(todayMonth == "Mar" || todayMonth == "Apr" || todayMonth == "May") {
			// Spring Seasonals
			// Regular Crop: Broccoli
			seedPos = 3;
			seedPrice = 100;
			
			// Check for monthlies
			if(todayMonth == "Mar") {
				// March Lettuce
				seasonalPos = 8;
				seasonalPrice = 100;
			} else if(todayMonth == "Apr") {
				// April Peas
				seasonalPos = 9;
				seasonalPrice = 140;
			} else {
				// May Cantaloupe
				seasonalPos = 10;
				seasonalPrice = 300;
			}
		} else if(todayMonth == "Jun" || todayMonth == "Jul" || todayMonth == "Aug") {
			// Summer Seasonals
			// Regular Crop: Tomato
			seedPos = 4;
			seedPrice = 100;
			
			// Check for monthlies
			if(todayMonth == "Jun") {
				// STUB : Pumpkin
				seasonalPos = 2;
				seasonalPrice = 350;
			} else if(todayMonth == "Jul") {
				// October Pumpkins
				seasonalPos = 2;
				seasonalPrice = 350;
			} else {
				// STUB : Pumpkin
				seasonalPos = 2;
				seasonalPrice = 350;
			}
		} else {
			// Fall Seasonals
			// Regular Crop: Carrot
			seedPos = 1;
			seedPrice = 120;
			
			// Check for monthlies
			if(todayMonth == "Sep") {
				// STUB : Pumpkin
				seasonalPos = 2;
				seasonalPrice = 350;
			} else if(todayMonth == "Oct") {
				// October Pumpkins 350g seed 500g sell
				seasonalPos = 2;
				seasonalPrice = 350;
			} else {
				// STUB : Pumpkin
				seasonalPos = 2;
				seasonalPrice = 350;
			}
		}	
	}
	
	// Buy Seeds
	public bool BuySeeds(int number) {
		if(gold >= seedPrice * number) {
			gold = gold - seedPrice * number;
			seeds[seedPos] = seeds[seedPos] + number;
			
			Save();
			
			return true;
		} else {
			return false;
		}
	}
	
	// Buy Seasonal Seeds
	public bool BuySeasonal(int number) {
		if(gold >= seasonalPrice * number) {
			gold = gold - seasonalPrice * number;
			seeds[seasonalPos] = seeds[seasonalPos] + number;
			
			Save();
			
			return true;
		} else {
			return false;
		}
	}
	
	// Buy whole inventory
	public void BuyInventory() {
		// Buy all crops
		BuyCrops(0);
		
		// Buy all items
		BuyItems(0);
		
		// Save the game 
		Save();
	}
	
	// Buy Crops
	public void BuyCrops(int remaining) {
		if(remaining < 0) {
			remaining = 0;
		}
		
		// Sell turnips
		if(crops[0] > 0) {
			if(crops[0] > remaining) {
				gold = gold + (crops[0] - remaining) * 120;
				crops[0] = remaining;
			}
		}
		
		// Sell carrots
		if(crops[1] > 0) {
			if(crops[1] > remaining) {
				gold = gold + (crops[1] - remaining) * 150;
				crops[1] = remaining;
			}
		}
		
		// Sell pumpkins
		if(crops[2] > 0) {
			if(crops[2] > remaining) {
				gold = gold + (crops[2] - remaining) * 500;
				crops[2] = remaining;
			}
		}
		
		// Sell broccoli
		if(crops[3] > 0) {
			if(crops[3] > remaining) {
				gold = gold + (crops[3] - remaining) * 150;
				crops[3] = remaining;
			}
		}
		
		// Sell tomato
		if(crops[4] > 0) {
			if(crops[4] > remaining) {
				gold = gold + (crops[4] - remaining) * 200;
				crops[4] = remaining;
			}
		}
		
		// Sell sugarplum
		if(crops[5] > 0) {
			if(crops[5] > remaining) {
				gold = gold + (crops[5] - remaining) * 10000;
				crops[5] = remaining;
			}
		}
		
		// Sell leek
		if(crops[6] > 0) {
			if(crops[6] > remaining) {
				gold = gold + (crops[6] - remaining) * 450;
				crops[6] = remaining;
			}
		}
		
		// Sell Pommegranate
		if(crops[7] > 0) {
			if(crops[7] > remaining) {
				gold = gold + (crops[7] - remaining) * 1500;
				crops[7] = remaining;
			}
		}
		
		// Sell Lettuce
		if(crops[8] > 0) {
			if(crops[8] > remaining) {
				gold = gold + (crops[8] - remaining) * 200;
				crops[8] = remaining;
			}
		}
		
		// Sell Peas
		if(crops[9] > 0) {
			if(crops[9] > remaining) {
				gold = gold + (crops[9] - remaining) * 300;
				crops[9] = remaining;
			}
		}
		
		// Sell Cantaloupe
		if(crops[10] > 0) {
			if(crops[10] > remaining) {
				gold = gold + (crops[10] - remaining) * 500;
				crops[10] = remaining;
			}
		}
		
		Save();
	}
	
	// Buy Items
	public void BuyItems(int remaining) {
		if(remaining < 0) {
			remaining = 0;
		}
		
		// Sell acorns
		if(items[0] > 0) {
			if(items[0] > remaining) {
				gold = gold + (items[0] - remaining) * 10;
				items[0] = remaining;
			}
		}
		
		// Sell mushrooms
		if(items[1] > 0) {
			if(items[1] > remaining) {
				gold = gold + (items[1] - remaining) * 10;
				items[1] = remaining;
			}
		}
		
		// Sell scallops
		if(items[2] > 0) {
			if(items[2] > remaining) {
				gold = gold + (items[2] - remaining) * 10;
				items[2] = remaining;
			}
		}

		Save();
	}
}

// Private class of player data
[Serializable]
class PlayerData {
	// Size variables
	public static int CROPS = 11;

	public int gold;
	
	// Store strings of the plant plot states in format
	// "plant_plot_##,<growth stage character: 'D'irt, 'S'eed, '1' (sprout), '2' (stalk), '3' (harvest)>,
	// <plant type string: "turnip", "carrot", etc.>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
	public string[] plots;
	
	// Store inventory values
	
	// Seeds
	// Order of seed values: 0 turnips, 1 carrots, 2 pumpkins, 3 broccoli, 4 tomato, 5 sugarplum, 6 leek, 7 pommegranate,
	// 						 8 lettuce, 9 pea, 10 cantaloupe... 
	public int[] seeds;
	
	// Crops
	// Order of crop values: 0 turnips, 1 carrots, 2 pumpkins, 3 broccoli, 4 tomato, 5 sugarplum, 6 leek, 7 pommegranate,
	// 						 8 lettuce, 9 pea, 10 cantaloupe...
	public int[] crops;
	
	// Items
	// Order of item values: 0 acorns, 1 mushrooms, 2 scallops, 3 tulips, 4 pompons, ...
	public int[] items;
	
	// Gift spawns
	public DateTime crabGift;
	public int crabNumber = 0;
	public DateTime acornsDrop;
	public int acornsNumber = 0;
	public DateTime mushroomDrop;
	public int mushroomsNumber = 0;
	public DateTime flowerDrop;
	public int flowersNumber = 0;
	
	// UI variables
	public bool tutorial = true;
	
	// Class Methods
	// Public constructor
	public PlayerData() {
		plots = new string[8];
		seeds = new int[CROPS];
		crops = new int[CROPS];
		items = new int[5];
		
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		for(int index = 0; index < 8; index++) {
			plots[index] = "plant_plot_0" + (char)(index + 48) + "," + 'D' + ",dirt," + datenow;
		}
		
		for(int index = 0; index < CROPS; index++) {
			seeds[index] = 0;
		}
		
		for(int index = 0; index < CROPS; index++) {
			crops[index] = 0;
		}
		
		for(int index = 0; index < 5; index++) {
			items[index] = 0;
		}
		
		crabGift = System.DateTime.Now;
		acornsDrop = System.DateTime.Now;
		mushroomDrop = System.DateTime.Now;
		flowerDrop = System.DateTime.Now;
	}
}