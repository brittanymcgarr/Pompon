using UnityEngine;
using System.Collections;

public class HeadLookWalk : MonoBehaviour {
	public float velocity = 1.0f;
	public bool isWalking = false;
	
	private AudioSource footsteps;
	private CharacterController controller;
	private Clicker clicker = new Clicker();
	
	// Use this for initialization
	void Start () {
		controller = GetComponent<CharacterController>();
		footsteps = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if(clicker.clicked()) {
			isWalking = !isWalking;
		}
		if(isWalking) {
			controller.SimpleMove(Camera.main.transform.forward * velocity);
			
			if(!footsteps.isPlaying) {
				footsteps.Play();
			}
		} else {
			footsteps.Stop();
		}
	}
}
