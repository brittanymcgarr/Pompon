////////////////////////////////////////////////////////////////////////////////
// BunnyHutch.cs                                                              //
// Reads the data for the available rabbits and displays their correct color  //
// variation based on the active allele.                                      //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

public class BunnyHutch : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// Get the parent's number
		string parentName = gameObject.transform.name;
		int bunnyNumber = parentName[6] - '0';
		
		if(bunnyNumber < 6) {
			string bunnyData = GameControl.control.bunnies[bunnyNumber];
			string[] tokens = bunnyData.Split(',');
			
			int alleleA = tokens[1][0] - '0';
			int alleleB = tokens[2][0] - '0';
			int dominantGene = 0;
			
			// If the alleles are greater than 0, the bunny is there
			if(alleleA > 0 && alleleB > 0) {
				// Find the dominant gene
				if(alleleA > alleleB) {
					dominantGene = alleleA;
				} else {
					dominantGene = alleleB;
				}
				
				// Determine the material based on the dominant gene
				Material bunnyMaterial;
				if(dominantGene == 4) {
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_brown", typeof(Material));
				} else if(dominantGene == 3) {
					// Followed by black
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_black", typeof(Material));
				} else if(dominantGene == 2) {
					// Gray
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_cutout", typeof(Material));
				} else {
					// White is the rarest
					bunnyMaterial = (Material)Resources.Load("Models/Materials/Materials/cardboard_bunny_white", typeof(Material));
				}
					
				GetComponent<Renderer>().sharedMaterial = bunnyMaterial;
				
				// Adjust the size if the bunny is a baby
				string datetimeFormat = "yyyy-MM-dd HH:mm:ss.fffffff";
				DateTime birthdate = System.DateTime.ParseExact(tokens[4], datetimeFormat, CultureInfo.InvariantCulture);
				
				if(birthdate.AddDays(3) > System.DateTime.Now) {
					gameObject.transform.localScale = new Vector3(0.001f, 0.5f, 0.5f);
				} 
			} else {
				gameObject.SetActive(false);
			}
		} else {
			gameObject.SetActive(false);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
