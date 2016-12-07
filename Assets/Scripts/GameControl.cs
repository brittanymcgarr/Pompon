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
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameControl : MonoBehaviour {
    // Make this a singleton class by storing a reference
	public static GameControl control;

	// Variables
	public static int CROPS = 16;
	public static int PLOTS = 32;
	public static int CHICKENS = 8;
	public static int COWS = 5;
	public static int BUNNIES = 6;
	public static int ITEMS = 11;
	public int seedPos = 0;
	public int seedPrice = 100;
	public int seasonalPos = 0;
	public int seasonalPrice = 200;
	
	// Persistent data to store
	public int gold;
	public string[] plots = new string[PLOTS];
	public bool plotUpgrade1 = false;
	public bool plotUpgrade2 = false;
	public bool plotUpgrade3 = false;
	public string[] chickens = new string[CHICKENS];
	public string[] cows = new string[COWS];
	public string[] bunnies = new string[BUNNIES];
	public int[] seeds = new int[CROPS];
	public int[] crops = new int[CROPS];
	public int[] items = new int[ITEMS];
	public int[] storedSeeds = new int[CROPS];
	public int[] storedCrops = new int[CROPS];
	public char freezerStage = 'b';
	public char hopperStage = 'b';
	public string seedHopper = "";
	public string butterChurn = "";
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
	public DateTime bunnyBreed;
	public bool carrotPlanted = false;
	public DateTime bunnyCaught;
	
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
		} else if(control != this) {
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
		// Get time information
		string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
		string dateString = DateTime.Now.ToString(datetimeFormat);
			
		// Create the binary file and path name
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
		
		// Create the player data object to serialize and save
		PlayerData data = new PlayerData();
		
		// Player wealth
		data.gold = gold;
		
		// Check for valid plant plot data
		if(plots.Length == PLOTS) {
			data.plots = plots;
		} else {			
			data.plots = new string[PLOTS];
		
			for(int index = 0; index < plots.Length; index++) {
				data.plots[index] = plots[index];
			}
			
			for(int count = plots.Length; count < PLOTS; count++) {
				data.plots[count] = "plant_plot_" + (char) (count / 8 + 48) + (char) (count % 8 + 48) + ",D,," + dateString;
			}
		}
		
		// Check for plot upgrades
		data.plotUpgrade1 = plotUpgrade1;
		data.plotUpgrade2 = plotUpgrade2;
		data.plotUpgrade3 = plotUpgrade3;
		
		// Check for chickens
		if(chickens.Length == CHICKENS && chickens[0] != "") {
			data.chickens = chickens;
		} else {
			data.chickens = new string[CHICKENS];
			
			for(int index = 0; index < CHICKENS; index++) {
				data.chickens[index] = "Nest0" + (char)(index + 48) + ",E,N," + dateString + ",0,N,0," + dateString;
			}
		}
		
		// Check for cows
		if(cows.Length == COWS && cows[0] != "") {
			data.cows = cows;
		} else {
			data.cows = new string[COWS];
			
			for(int index = 0; index < COWS; index++) {
				cows[index] = "Cow" + (char)(index + 48) + ",E," + dateString + ",0,N,0," + dateString + "," + dateString;
			}
		}
		
		// Check for bunnies
		if(bunnies.Length == BUNNIES && bunnies[0] != "") {
			data.bunnies = bunnies;
		} else {
			data.bunnies = new string[BUNNIES];
			
			for(int index = 0; index < BUNNIES; index++) {
				bunnies[index] = "Bunny0" + (char)(index + 48) + ",0,0,N," + dateString;
			}
		}
			
		// Check for valid seeds length
		if(seeds.Length == CROPS) {
			data.seeds = seeds;
		} else {
			seeds = new int[CROPS];
			
			// Not the correct length of data, so reinitialize
			for(int index = 0; index < CROPS; index++) {
				seeds[index] = 0;
			}
			
			data.seeds = seeds;
		}
		
		// Check for valid crops length
		if(crops.Length == CROPS) {
			data.crops = crops;
		} else {
			crops = new int[CROPS];
			
			for(int index = 0; index < CROPS; index++) {
				crops[index] = 0;
			}
			
			data.crops = crops;
		}
		
		// Check for valid items
		if(items.Length == ITEMS) {
			data.items = items;
		} else {
			int[] hold = new int[items.Length];
			hold = items;
			items = new int[ITEMS];
			
			for(int index = 0; index < ITEMS; index++) {
				if(index < hold.Length) {
					items[index] = hold[index];
				} else {
					items[index] = 0;
				}
			}
			
			data.items = items;
		}
		
		// Check for valid stored seeds data
		if(storedSeeds.Length != CROPS) {
			storedSeeds = new int[CROPS];
			
			for(int index = 0; index < CROPS; index++) {
				storedSeeds[index] = 0;
			}
		}
		data.storedSeeds = storedSeeds;
		
		// Check for valid stored crops data
		if(storedCrops.Length != CROPS) {
			storedCrops = new int[CROPS];
			
			for(int index = 0; index < CROPS; index++) {
				storedCrops[index] = 0;
			}
		}
		data.storedCrops = storedCrops;
		
		// Freezer and hopper repair states
		data.freezerStage = freezerStage;
		data.hopperStage = hopperStage;
		
		// Check for valid seed hopper data
		if(seedHopper == "") {
			seedHopper = "b,-1," + dateString;
		}
		data.seedHopper = seedHopper;
		
		// Check for valid butter churn data
		if(butterChurn == "") {
			butterChurn = "b,N," + dateString;
		}
		data.butterChurn = butterChurn;
		
		// Crab data
		data.crabGift = crabGift;
		data.crabNumber = crabNumber;
		
		// Acorn drop data
		data.acornsDrop = acornsDrop;
		data.acornsNumber = acornsNumber;
		
		// Mushroom data
		data.mushroomDrop = mushroomDrop;
		data.mushroomsNumber = mushroomsNumber;
		
		// Flower data
		data.flowerDrop = flowerDrop;
		data.flowersNumber = flowersNumber;
		
		// Bunny breeding date
		data.bunnyBreed = bunnyBreed;
		
		// Carrots?
		data.carrotPlanted = carrotPlanted;
		
		// Bunny caught today?
		data.bunnyCaught = bunnyCaught;
		
		// Tutorial states
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
			PlayerData data;
			data = (PlayerData) bf.Deserialize(file);
			
			// Close the file
			file.Close();
			
			// Set that data
			gold = data.gold;
			
			string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
			string dateString = DateTime.Now.ToString(datetimeFormat);
			
			// Set the plots
			if(data.plots.Length < PLOTS) {
				for(int index = 0; index < data.plots.Length; index++) {
					plots[index] = data.plots[index];
				}
				
				for(int count = data.plots.Length; count < PLOTS; count++) {
					plots[count] = "plant_plot_" + (char) (count / 8 + 48) + (char) (count % 8 + 48) + ",D,," + dateString;
				}
			} else {
				plots = data.plots;
			}
			
			// Plot upgrades
			plotUpgrade1 = data.plotUpgrade1;
			plotUpgrade2 = data.plotUpgrade2;
			plotUpgrade3 = data.plotUpgrade3;
			
			// Set up the chickens
			if(data.chickens.Length < CHICKENS || data.chickens[0] == "") {
				data.chickens = new string[CHICKENS];
				
				for(int index = 0; index < CHICKENS; index++) {
					data.chickens[index] = "Nest0" + (char)(index + 48) + ",E,N," + dateString + ",0,N,0," + dateString;
				}
			}
			chickens = data.chickens;
			
			if(data.cows.Length < COWS || data.cows[0] == "") {
				data.cows = new string[COWS];
				
				for(int index = 0; index < COWS; index++) {
					cows[index] = "Cow" + (char)(index + 48) + ",E," + dateString + ",0,N,0," + dateString + "," + dateString;
				}
			}
			cows = data.cows;
			
			if(data.bunnies.Length < BUNNIES || data.bunnies[0] == "") {
				data.bunnies = new string[BUNNIES];
				
				for(int index = 0; index < BUNNIES; index++) {
					bunnies[index] = "Bunny0" + (char)(index + 48) + ",0,0,N," + dateString;
				}
			}
			bunnies = data.bunnies;
			
			// Set the seeds
			if(data.seeds.Length < CROPS) {
				for(int index = 0; index < data.seeds.Length; index++) {
					seeds[index] = data.seeds[index];
				}
			} else {
				seeds = data.seeds;
			}
				
			// Set the crops
			if(data.crops.Length < CROPS) {
				for(int index = 0; index < data.crops.Length; index++) {
					crops[index] = data.crops[index];
				}
			} else {
				crops = data.crops;
			}
			
			// Set the items
			if(data.items.Length < ITEMS) {
				int[] hold = new int[data.items.Length];
				hold = data.items;
				data.items = new int[ITEMS];
				
				for(int index = 0; index < ITEMS; index++) {
					if(index < hold.Length) {
						data.items[index] = hold[index];
					} else {
						data.items[index] = 0;
					}
				}
			}
			items = data.items;
			
			// Set the stored seeds
			if(data.storedSeeds.Length < CROPS) {
				for(int index = 0; index < data.storedSeeds.Length; index++) {
					storedSeeds[index] = data.storedSeeds[index];
				}
			} else {
				storedSeeds = data.storedSeeds;
			}	
			
			// Set the stored crops
			if(data.storedCrops.Length < CROPS) {
				for(int index = 0; index < data.storedCrops.Length; index++) {
					storedCrops[index] = data.storedCrops[index];
				}
			} else {
				storedCrops = data.storedCrops;
			}
				
			freezerStage = data.freezerStage;
			hopperStage = data.hopperStage;
			seedHopper = data.seedHopper;
			butterChurn = data.butterChurn;
			crabGift = data.crabGift;
			crabNumber = data.crabNumber;
			acornsDrop = data.acornsDrop;
			acornsNumber = data.acornsNumber;
			mushroomDrop = data.mushroomDrop;
			mushroomsNumber = data.mushroomsNumber;
			flowerDrop = data.flowerDrop;
			flowersNumber = data.flowersNumber;
			bunnyBreed = data.bunnyBreed;
			carrotPlanted = data.carrotPlanted;
			bunnyCaught = data.bunnyCaught;
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
		string dateprior = timestamp.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		plots = new string[PLOTS];
		seeds = new int[CROPS];
		crops = new int[CROPS];
		chickens = new string[CHICKENS];
		cows = new string[COWS];
		bunnies = new string[BUNNIES];
		items = new int[ITEMS];
		storedSeeds = new int[CROPS];
		storedCrops = new int[CROPS];
		
		for(int index = 0; index < 4; index++) {
			for(int count = 0; count < 8; count++) {
				plots[index * 8 + count] = "plant_plot_" + (char) (index + 48) + (char)(count + 48) + "," + 'D' 
											+ ",dirt," + datenow;
			}
		}
		
		for(int index = 0; index < CHICKENS; index++) {
			chickens[index] = "Nest0" + (char)(index + 48) + ",E,N," + datenow + ",0,N,0," + datenow;
		}
		
		for(int index = 0; index < COWS; index++) {
			cows[index] = "Cow" + (char)(index + 48) + ",E," + datenow + ",0,N,0," + datenow + "," + dateprior;
		}
		
		for(int index = 0; index < BUNNIES; index++) {
			bunnies[index] = "Bunny0" + (char)(index + 48) + ",0,0,N," + datenow;
		}
		
		for(int index = 0; index < CROPS; index++) {
			seeds[index] = 0;
			crops[index] = 0;
			storedSeeds[index] = 0;
			storedCrops[index] = 0;
		}
		
		for(int index = 0; index < ITEMS; index++) {
			items[index] = 0;
		}
		
		// Give the player some turnip seeds and 100 gold
		gold = 100;
		seeds[0] = 3;
		tutorial = true;
		freezerStage = 'b';
		hopperStage = 'b';
		
		// Reset the seed hopper information
		seedHopper = "b,-1," + datenow;
		
		// Reset the butter churn information
		butterChurn = "b,N," + datenow;
		
		// Reset their gifting times
		crabGift = System.DateTime.Now;
		crabGift = crabGift.AddDays(-1);
		acornsDrop = System.DateTime.Now;
		acornsDrop = acornsDrop.AddDays(-1);
		mushroomDrop = System.DateTime.Now;
		mushroomDrop = mushroomDrop.AddDays(-1);
		flowerDrop = System.DateTime.Now;
		flowerDrop = flowerDrop.AddDays(-1);
		bunnyBreed = System.DateTime.Now;
		carrotPlanted = false;
		bunnyCaught = System.DateTime.Now.AddDays(-1);
		
		crabNumber = acornsNumber = mushroomsNumber = flowersNumber = 0;
		
		// Delete the old data
		File.Delete(Application.persistentDataPath + "/playerInfo.dat");
		
		// Reset the plant plots
		plotUpgrade1 = false;
		plotUpgrade2 = false;
		plotUpgrade3 = false;
		
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
				// June Grapes
				seasonalPos = 11;
				seasonalPrice = 500;
			} else if(todayMonth == "Jul") {
				// July Watermelon
				seasonalPos = 12;
				seasonalPrice = 600;
			} else {
				// August Strawberries
				seasonalPos = 13;
				seasonalPrice = 300;
			}
		} else {
			// Fall Seasonals
			// Regular Crop: Carrot
			seedPos = 1;
			seedPrice = 120;
			
			// Check for monthlies
			if(todayMonth == "Sep") {
				// September Sweet Potato
				seasonalPos = 14;
				seasonalPrice = 120;
			} else if(todayMonth == "Oct") {
				// October Pumpkins 350g seed 500g sell
				seasonalPos = 2;
				seasonalPrice = 350;
			} else {
				// November Cranberry
				seasonalPos = 15;
				seasonalPrice = 200;
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
		if(crops[0] > remaining) {
			gold = gold + ((crops[0] - remaining) * 120);
			crops[0] = remaining;
		}
		
		// Sell carrots
		if(crops[1] > remaining) {
			gold = gold + ((crops[1] - remaining) * 150);
			crops[1] = remaining;
		}

		
		// Sell pumpkins
		if(crops[2] > remaining) {
			gold = gold + ((crops[2] - remaining) * 500);
			crops[2] = remaining;
		}
		
		// Sell broccoli
		if(crops[3] > remaining) {
			gold = gold + ((crops[3] - remaining) * 150);
			crops[3] = remaining;
		}
		
		// Sell tomato
		if(crops[4] > remaining) {
			gold = gold + ((crops[4] - remaining) * 200);
			crops[4] = remaining;
		}
		
		// Sell sugarplum
		if(crops[5] > remaining) {
			gold = gold + ((crops[5] - remaining) * 10000);
			crops[5] = remaining;
		}
		
		// Sell leek
		if(crops[6] > remaining) {
			gold = gold + ((crops[6] - remaining) * 450);
			crops[6] = remaining;
		}
		
		// Sell Pommegranate
		if(crops[7] > remaining) {
			gold = gold + ((crops[7] - remaining) * 1500);
			crops[7] = remaining;
		}
		
		// Sell Lettuce
		if(crops[8] > remaining) {
			gold = gold + ((crops[8] - remaining) * 200);
			crops[8] = remaining;
		}
		
		// Sell Peas
		if(crops[9] > remaining) {
			gold = gold + ((crops[9] - remaining) * 300);
			crops[9] = remaining;
		}
		
		// Sell Cantaloupe
		if(crops[10] > remaining) {
			gold = gold + ((crops[10] - remaining) * 500);
			crops[10] = remaining;
		}
		
		// Sell Grapes
		if(crops[11] > remaining) {
			gold = gold + ((crops[11] - remaining) * 1500);
			crops[11] = remaining;
		}
		
		// Sell Watermelon
		if(crops[12] > remaining) {
			gold = gold + ((crops[12] - remaining) * 1000);
			crops[12] = remaining;
		}
		
		// Sell Strawberries
		if(crops[13] > remaining) {
			gold = gold + ((crops[13] - remaining) * 450);
			crops[13] = remaining;
		}
		
		// Sell Sweet Potato
		if(crops[14] > remaining) {
			gold = gold + ((crops[14] - remaining) * 150);
			crops[14] = remaining;
		}
		
		// Sell Cranberries
		if(crops[15] > remaining) {
			gold = gold + ((crops[15] - remaining) * 300);
			crops[15] = remaining;
		}
		
		Save();
	}
	
	// Buy Items
	public void BuyItems(int remaining) {
		if(remaining < 0) {
			remaining = 0;
		}
		
		// Sell acorns
		if(items[0] > remaining) {
			gold += (items[0] - remaining) * 10;
			items[0] = remaining;
		}
		
		// Sell mushrooms
		if(items[1] > remaining) {
			gold += (items[1] - remaining) * 50;
			items[1] = remaining;
		}
		
		// Sell scallops
		if(items[2] > remaining) {
			gold += (items[2] - remaining) * 140;
			items[2] = remaining;
		}
		
		// Sell eggs
		if(items[5] > remaining) {
			gold += (items[5] - remaining) * 350;
			items[5] = remaining;
		}
		
		// Sell gold eggs
		if(items[6] > 0) {
			gold += (items[6] - remaining) * 700;
			items[6] = remaining;
		}
		
		// Sell milk
		if(items[7] > 0) {
			gold += (items[7] - remaining) * 500;
			items[7] = remaining;
		}
		
		// Sell good milk
		if(items[8] > 0) {
			gold += (items[8] - remaining) * 1000;
			items[8] = remaining;
		}
		
		// Sell butter
		if(items[9] > 0) {
			gold += (items[9] - remaining) * 1000;
			items[9] = remaining;
		}
		
		// Sell Blue Ribbon Butter
		if(items[10] > 0) {
			gold += (items[10] - remaining) * 2000;
			items[10] = remaining;
		}

		Save();
	}
	
	// Store Seeds
	public void StoreSeeds(int position) {
		// Store all the seeds if the position is a negative number
		if(position < 0) {
			for(int index = 0; index < CROPS; index++) {
				storedSeeds[index] += seeds[index];
				seeds[index] = 0;
			}
		} else {
			if(seeds[position] > 0) {
				storedSeeds[position] += seeds[position];
				seeds[position] = 0;
			}
		}
		
		Save();
	}
	
	// Withdraw Seeds
	public void WithdrawSeeds(int position) {
		// Withdraw all Seeds if the position is negative
		if(position < 0) {
			for(int index = 0; index < CROPS; index++) {
				seeds[index] += storedSeeds[index];
				storedSeeds[index] = 0;
			}
		} else {
			seeds[position] += storedSeeds[position];
			storedSeeds[position] = 0;
		}
		
		Save();
	}
	
	// Store Crops
	public void StoreCrops(int position) {
		int remaining = 0;
		
		// Check stage of freezer
		if(freezerStage == 'r') {
			// If the freezer was repaired, can store as many as the player has up to 100
			if(position < 0) {
				// A negative means store all produce
				for(int index = 0; index < CROPS; index++) {
					if(crops[index] >= 100) {
						remaining = 100 - storedCrops[index];
						storedCrops[index] += remaining;
						crops[index] -= remaining;
					} else {
						if(storedCrops[index] + crops[index] > 100) {
							remaining = storedCrops[index] - 100;
							crops[index] += remaining;
							storedCrops[index] -= remaining;
						} else {
							storedCrops[index] += crops[index];
							crops[index] = 0;
						}
					}
				}
			} else {
				if(crops[position] >= 100) {
					remaining = 100 - storedCrops[position];
					storedCrops[position] += remaining;
					crops[position] -= remaining;
				} else {
					if(storedCrops[position] + crops[position] > 100) {
						remaining = storedCrops[position] - 100;
						crops[position] += remaining;
						storedCrops[position] -= remaining;
					} else {
						storedCrops[position] += crops[position];
						crops[position] = 0;
					}
				}
			}
		} else {
			// Unrepaired fridge can only store 1 of any type of item.
			if(position < 0) {
				// Store one of each
				for(int index = 0; index < CROPS; index++) {
					if(storedCrops[index] == 0 && crops[index] > 0) {
						storedCrops[index] += 1;
						crops[index] -= 1;
					}
				}
			} else {
				if(storedCrops[position] == 0 && crops[position] > 0) {
					storedCrops[position] += 1;
					crops[position] -= 1;
				}
			}
		}
		
		Save();
	}
	
	// Withdraw Crops
	public void WithdrawCrops(int position) {
		if(position < 0) {
			for(int index = 0; index < CROPS; index++) {
				crops[index] += storedCrops[index];
				storedCrops[index] = 0;
			}
		} else {
			crops[position] += storedCrops[position];
			storedCrops[position] = 0;
		}
		
		Save();
	}
	
	// Store a crop for the seed hopper
	public bool StoreHopper(int position) {
		if(position >= 0) {
			string saveData = seedHopper;
			string[] dataTokens = saveData.Split(',');
			int plantType = Int32.Parse(dataTokens[1]);
			
			if(plantType == -1) {
				if(crops[position] > 0) {
					crops[position] -= 1;
					
					DateTime timestamp = System.DateTime.Now;
					string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
					seedHopper = dataTokens[0] + "," + position.ToString() + "," + datenow;
					
					Save();
					
					return true;
				} else {
					return false;
				}
			} else {
				return false;
			}
		} else {
			return false;
		}
	}
	
	// Withdraw a ready seed packet from the seed hopper
	public bool WithdrawHopper() {
		string saveData = seedHopper;
		string[] dataTokens = saveData.Split(',');
		int plantType = Int32.Parse(dataTokens[1]);
		string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
		DateTime depositTime = System.DateTime.ParseExact(dataTokens[2], datetimeFormat, CultureInfo.InvariantCulture);
		
		if((dataTokens[0])[0] != 'b' && plantType >= 0 && depositTime.AddHours(6) < System.DateTime.Now) {
			DateTime timestamp = System.DateTime.Now;
			string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
			seedHopper = dataTokens[0] + ",-1," + datenow;
			
			if((dataTokens[0])[0] == 'u') {
				seeds[plantType] += 3;
			} else {
				seeds[plantType] += 2;
			}
			
			Save();
			
			return true;
		} else {
			return false;
		}
	}
	
	// Upgrade the seed hopper
	public void UpgradeHopper() {
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		if(seedHopper != "" && seedHopper != null) {
			string[] dataTokens = seedHopper.Split(',');
			
			if((dataTokens[0])[0] != 'u') {
				if((dataTokens[0])[0] == 'b') {
					seedHopper = "r,-1," + datenow;
				} else {
					seedHopper = "u,-1," + datenow;
				}
				
				Save();
			}
		} else {			
			seedHopper = "r,-1," + datenow;
			
			Save();
		}
	}
	
	// Upgrade the butter churn
	public void UpgradeChurn() {
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		if(butterChurn != "" && butterChurn != null) {
			string[] dataTokens = butterChurn.Split(',');
			
			if(dataTokens[0][0] == 'b') {
				butterChurn = "r,N," + datenow;
			}
		} else {
			butterChurn = "b,N," + datenow;
		}
		
		Save();
	}
	
	// Withdraw butter from the churn
	public void WithdrawChurn() {
		string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
		
		if(butterChurn != "" && butterChurn != null) {
			string[] dataTokens = butterChurn.Split(',');
			DateTime deposit = System.DateTime.ParseExact(dataTokens[2], datetimeFormat, CultureInfo.InvariantCulture);
			char milk = dataTokens[1][0];
			
			if(deposit.AddHours(6) <= System.DateTime.Now && (milk == 'G' || milk == 'M')) {
				if(milk == 'G') {
					items[10] += 1;
				} else {
					items[9] += 1;
				}
				
				butterChurn = "r,N," + System.DateTime.Now.ToString(datetimeFormat);
				
				Save();
			}
		}
	}
	
	// Deposit butter to the churn
	public void DepositChurn(char milkType) {
		if(butterChurn[0] != 'b' && butterChurn[2] == 'N') {
			if(milkType == 'G' && items[8] > 0) {
				items[8] -= 1;
				
				butterChurn = "r,G," + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
				
				Save();
			} else if(milkType == 'M' && items[7] > 0) {
				items[7] -= 1;
				
				butterChurn = "r,M," + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
				
				Save();
			} else {
			
			}
		}
	}
}

// Private class of player data
[Serializable]
class PlayerData {
	// Size variables
	public static int CROPS = 16;
	public static int PLOTS = 32;
	public static int CHICKENS = 8;
	public static int BUNNIES = 6;
	public static int COWS = 5;
	public static int ITEMS = 11;

	public int gold;
	
	// Store strings of the plant plot states in format
	// "plant_plot_##,<growth stage character: 'D'irt, 'S'eed, '1' (sprout), '2' (stalk), '3' (harvest)>,
	// <plant type string: "turnip", "carrot", etc.>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
	public string[] plots;
	
	// Plot upgrade states
	public bool plotUpgrade1 = false;
	public bool plotUpgrade2 = false;
	public bool plotUpgrade3 = false;
	
	// Chickens
	// Nest values in string data format
	// "Nest##,<Growth Stage character: 'E'mpty, E'G'g, 'C'hick, 'A'dult>,<Gender character: 'N'one, 'F'emale, 'M'ale>,
	// <DateTime string of purchase/laid in "yyyy-MM-dd HH:mm:ss.fffffff">,<Hearts # character: '0', '1', '2', '3'>,
	// <Egg status: 'N'one, 'C'ollected, 'W'aiting, 'G'old>,<Pats # character: '0' ... '9'>,
	// <DateTime string of collection in "yyyy-MM-dd HH:mm:ss.fffffff">"
	public string[] chickens;
	
	// Cows
	// Cow values in string data format
	// "Cow#,<Cow Present character: 'E'mpty, 'A'vailable>,<DateTime string of purchase date in "yyyy-MM-dd HH:mm:ss.fffffff">,
	// <Hearts # character: '0', '1', '2', '3'>,<Milk status: 'N'one, 'C'ollected, 'W'aiting, 'G'ood>,<Pats # character: '0' ... '9'>,
	// <DateTime string of collection time in "yyyy-MM-dd HH:mm:ss.fffffff">,<DateTime string of fodder refill in "yyyy-MM-dd HH:mm:ss.fffffff">"
	public string[] cows;
	
	// Bunnies
	// Bunny save values in string data format
	// "Bunny##,<Bunny Allele 1 int: 0 = No rabbit, 1 = white, 2 = gray, 3 = black, 4 = brown>,<Bunny Allele 2 int: same as 1>,
	// <Gender character: 'N'one, 'M'ale, 'F'emale>,<DateTime age in "yyyy-MM-dd HH:mm:ss.fffffff" format>"
	public string[] bunnies;
	
	// Store inventory values
	
	// Seeds
	// Order of seed values: 0 turnips, 1 carrots, 2 pumpkins, 3 broccoli, 4 tomato, 5 sugarplum, 6 leek, 7 pommegranate,
	// 						 8 lettuce, 9 pea, 10 cantaloupe, 11 grapes, 12 watermelon, 13 strawberry, 14 sweetpotato,
	//						 15 cranberry... 
	public int[] seeds;
	
	// Crops
	// Order of crop values: 0 turnips, 1 carrots, 2 pumpkins, 3 broccoli, 4 tomato, 5 sugarplum, 6 leek, 7 pommegranate,
	// 						 8 lettuce, 9 pea, 10 cantaloupe, 11 grapes, 12 watermelon, 13 strawberry, 14 sweetpotato,
	//						 15 cranberry...
	public int[] crops;
	
	// Items
	// Order of item values: 0 acorns, 1 mushrooms, 2 scallops, 3 tulips, 4 pompons, 5 eggs, 6 gold eggs, 7 milk, 8 good milk,
	// 9 butter, 10 blue ribbon butter ...
	public int[] items;
	
	// Stored Seeds
	public int[] storedSeeds;
	
	// Stored Crops
	public int[] storedCrops;
	
	// Appliance stages
	public char freezerStage = 'b';
	public char hopperStage = 'b';
	
	// Seed Hopper data
	public string seedHopper = "";
	
	// Butter Churn data
	public string butterChurn = "";
	
	// Gift spawns
	public DateTime crabGift;
	public int crabNumber = 0;
	public DateTime acornsDrop;
	public int acornsNumber = 0;
	public DateTime mushroomDrop;
	public int mushroomsNumber = 0;
	public DateTime flowerDrop;
	public int flowersNumber = 0;
	public DateTime bunnyBreed;
	public bool carrotPlanted;
	public DateTime bunnyCaught;
	
	// UI variables
	public bool tutorial = true;
	
	// Class Methods
	// Public constructor
	public PlayerData() {
		plots = new string[PLOTS];
		seeds = new int[CROPS];
		crops = new int[CROPS];
		chickens = new string[CHICKENS];
		cows = new string[COWS];
		bunnies = new string[BUNNIES];
		items = new int[ITEMS];
		storedSeeds = new int[CROPS];
		storedCrops = new int[CROPS];
		
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		for(int index = 0; index < 4; index++) {
			for(int count = 0; count < 8; count++) {
				plots[index * 8 + count] = "plant_plot_" + (char)(index + 48) + (char)(count + 48) + "," + 'D' + ",dirt," + datenow;
			}
		}
		
		plotUpgrade1 = false;
		plotUpgrade2 = false;
		plotUpgrade3 = false;
		
		for(int index = 0; index < CHICKENS; index++) {
			chickens[index] = "Nest0" + (char)(index + 48) + ",E,N," + datenow + ",0,N,0," + datenow;
		}
		
		string dateprior = timestamp.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		for(int index = 0; index < COWS; index++) {
			cows[index] = "Cow" + (char)(index + 48) + ",E," + datenow + ",0,N,0," + datenow + "," + dateprior;
		}
		
		for(int index = 0; index < BUNNIES; index++) {
			bunnies[index] = "Bunny0" + (char)(index + 48) + ",0,0,N," + datenow;
		}
		
		for(int index = 0; index < CROPS; index++) {
			seeds[index] = 0;
			crops[index] = 0;
			storedSeeds[index] = 0;
			storedCrops[index] = 0;
		}
		
		for(int index = 0; index < ITEMS; index++) {
			items[index] = 0;
		}
		
		freezerStage = 'b';
		hopperStage = 'b';
		
		seedHopper = "b," + (-1).ToString() + "," + datenow;
		butterChurn = "b,N," + datenow;
		
		crabGift = System.DateTime.Now;
		acornsDrop = System.DateTime.Now;
		mushroomDrop = System.DateTime.Now;
		flowerDrop = System.DateTime.Now;
		bunnyBreed = System.DateTime.Now;
		carrotPlanted = false;
		bunnyCaught = System.DateTime.Now;
	}
}