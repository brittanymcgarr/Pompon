////////////////////////////////////////////////////////////////////////////////
// BunnyManager.cs                                                            //
// Manages the breeding and introduction of bunnies to the hutch before the   //
// bunnies are displayed with BunnyHutch.cs.                                  //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class BunnyManager : MonoBehaviour {
	// Use this for initialization
	void Start () {
		DateTime bunnyBreeding = GameControl.control.bunnyBreed;
		
		// Only check for breeding pairs once a day
		if(bunnyBreeding.AddDays(1) <= System.DateTime.Now) {
			char[] genders = new char[6];
			DateTime[] birthdays = new DateTime[6];
			int femalePos = -1;
			int malePos = -1;
			int firstOpen = -1;
			
			string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
			DateTime nowTime = System.DateTime.Now;
			
			// Search the save data for viable breeding pairs and store the starting available male and female
			// older than 3 days old.
			for(int index = 0; index < 6; index++) {
				string bunnyData = GameControl.control.bunnies[index];
				string[] tokens = bunnyData.Split(',');

				birthdays[index] = System.DateTime.ParseExact(tokens[4], datetimeFormat, CultureInfo.InvariantCulture);
				
				// Check age and whether the gender is populated
				if(birthdays[index].AddDays(3) <= nowTime && tokens[3][0] != 'N') {
					genders[index] = tokens[3][0];
					
					if(genders[index] == 'F' && femalePos < 0) {
						femalePos = index;
					} else {
						if(malePos < 0) {
							malePos = index;
						}
					}
				}
				
				// Get the first available space for a baby bunny
				if(tokens[3][0] == 'N' && firstOpen < 0) {
					firstOpen = index;
				}
			}
			
			// Check that we found breeding pairs
			if(femalePos >= 0 && malePos >= 0 && firstOpen >= 0) {
				for(int index = firstOpen; index < 6 && malePos < 6 && femalePos < 6; index++) {
					if(GameControl.control.bunnies[index][12] == 'N') {
						int parentA = nowTime.Millisecond % 2;
						int parentB = nowTime.Second % 2;
						int baby = nowTime.Hour % 2;
						
						char babyGender;
						char alleleA;
						char alleleB;
					
						// Determine the baby gender
						if(baby == 0) {
							babyGender = 'F';
						} else {
							babyGender = 'M';
						}
						
						// Get the mother's contributing allele
						if(parentA == 0) {
							alleleA = GameControl.control.bunnies[femalePos][8];
						} else {
							alleleA = GameControl.control.bunnies[femalePos][10];
						}
						
						// Get the father's allele
						if(parentB == 0) {
							alleleB = GameControl.control.bunnies[malePos][8];
						} else {
							alleleB = GameControl.control.bunnies[malePos][10];
						}
						
						// Store the baby's information
						GameControl.control.bunnies[index] = "Bunny0" + (char)(index + '0') + "," + alleleA + "," + alleleB + "," + 
																babyGender + "," + nowTime.ToString(datetimeFormat);
																
						// Find the next set of parents
						for(int mother = femalePos + 1; mother < 6; mother++) {
							if(genders[mother] == 'F') {
								femalePos = mother;
								mother = 7;
							}
							
							if(mother != 7) {
								femalePos = 7;
							}
						}
						
						for(int father = malePos + 1; father < 6; father++) {
							if(genders[father] == 'M') {
								malePos = father;
								father = 7;
							}
							
							if(father != 7) {
								malePos = 7;
							}
						}
					}
				}
				
				// Update the new breeding date
				GameControl.control.bunnyBreed = System.DateTime.Now;
				
				// Save the new babies
				GameControl.control.Save();
			}
		}	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
