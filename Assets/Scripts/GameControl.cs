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

	// Persistent data to store
	public int gold;
	public string[] plots = new string[8];
	public int[] seeds = new int[1];
	public int[] crops = new int[1];
	public int[] items = new int[5];
	public DateTime crabGift;
	public int crabNumber = 0;
	public DateTime acornsDrop;
	public int acornsNumber = 0;
	public DateTime mushroomDrop;
	public int mushroomsNumber = 0;
	public DateTime flowerDrop;
	public int flowersNumber = 0;
	
	// Awake comes before Start
	void Awake () {
		// Check for an instance of the GameControl already loaded
		// If it has not been loaded, store this instance
		if(control == null) {
			DontDestroyOnLoad(gameObject);
			control = this;
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
		seeds = new int[1];
		crops = new int[1];
		items = new int[5];
		
		for(int index = 0; index < 8; index++) {
			plots[index] = "plant_plot_0" + (char)(index + 48) + "," + 'D' + ",dirt," + datenow;
		}
		
		for(int index = 0; index < 1; index++) {
			seeds[index] = 0;
		}
		
		for(int index = 0; index < 1; index++) {
			crops[index] = 0;
		}
		
		for(int index = 0; index < 5; index++) {
			items[index] = 0;
		}
		
		// Give the player some turnip seeds and 100 gold
		gold = 100;
		seeds[0] = 3;
		
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
}

// Private class of player data
[Serializable]
class PlayerData {
	public int gold;
	
	// Store strings of the plant plot states in format
	// "plant_plot_##,<growth stage character: 'D'irt, 'S'eed, '1' (sprout), '2' (stalk), '3' (harvest)>,
	// <plant type string: "turnip", "carrot", etc.>,<DateTime string of planting in "yyyy-MM-dd HH:mm:ss.fffffff">"
	public string[] plots;
	
	// Store inventory values
	
	// Seeds
	// Order of seed values: 0 turnips, ... 
	public int[] seeds;
	
	// Crops
	// Order of crop values: 0 turnips, ...
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
	
	// Class Methods
	// Public constructor
	public PlayerData() {
		plots = new string[8];
		seeds = new int[1];
		crops = new int[1];
		items = new int[5];
		
		DateTime timestamp = System.DateTime.Now;
		string datenow = timestamp.ToString("yyyy-MM-dd HH:mm:ss.fffffff");
		
		for(int index = 0; index < 8; index++) {
			plots[index] = "plant_plot_0" + (char)(index + 48) + "," + 'D' + ",dirt," + datenow;
		}
		
		for(int index = 0; index < 1; index++) {
			seeds[index] = 0;
		}
		
		for(int index = 0; index < 1; index++) {
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