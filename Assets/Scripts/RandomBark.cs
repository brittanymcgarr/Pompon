////////////////////////////////////////////////////////////////////////////////
// RandomBark.cs                                                              //
// Plays an audio source for dog barks at random intervals for Elby.          //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class RandomBark : MonoBehaviour {
	// Public variables
	
	// Private variables
	private float randomTime;
	private AudioSource bark;

	// Use this for initialization
	void Start () {
		randomTime = Random.Range(0.0f, 60.0f);
		bark = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		randomTime -= Time.deltaTime;
		
		if(randomTime <= 0.0f) {
			if(!bark.isPlaying) {
				bark.Play();
			}
			
			randomTime = Random.Range(0.0f, 60.0f);
		}
	}
}
