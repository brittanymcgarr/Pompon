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
		}
	}
	
	// Update the GUI display
	void OnGUI () {
		// GUI.Label(new Rect(10, 10, 100, 30), "Gold: " + gold);
	}
}

// Private class of player data
[Serializable]
class PlayerData {
	public int gold;
}