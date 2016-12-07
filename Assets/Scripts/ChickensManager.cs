////////////////////////////////////////////////////////////////////////////////
// ChickensManager.cs                                                         //
// Manager class that updates the chicken and egg generation by viewing the   //
// current set of animals available.                                          //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class ChickensManager : MonoBehaviour {
	// Public variables
	public bool malePresent = false;
	
	// Private variables
	private DateTime eggTime;
	private const string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
	
	// Awake comes before Start
	void Awake () {
		// Find information on the current, nesting birds
		for(int index = 0; index < 8; index++) {
			string saveData = GameControl.control.chickens[index];
			string[] tokens = saveData.Split(',');
			
			if(saveData != "" && tokens.Length >= 7) {
				// Check values for an adult
				if(tokens[1][0] == 'A') {
					// Flag that the adult is male
					if(tokens[2][0] == 'M') {
						malePresent = true;
					} else {
						// Check if a female laid an egg
						if(tokens[5][0] == 'C' || tokens[5][0] == 'N') {
							eggTime = System.DateTime.ParseExact(tokens[7], datetimeFormat, CultureInfo.InvariantCulture);
							
							if(eggTime.AddDays(1) <= System.DateTime.Now) {
								if(tokens[4][0] - '0' == 3) {
									tokens[5] = "G";
								} else {
									tokens[5] = "W";
								}
							}
						}
					}
				} 
				
				string returnData = String.Join(",", tokens);
				GameControl.control.chickens[index] = returnData;
			}
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
