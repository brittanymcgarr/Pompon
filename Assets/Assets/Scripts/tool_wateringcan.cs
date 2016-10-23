////////////////////////////////////////////////////////////////////////////////
// tool_wateringcan.cs                                                        //
// Watering can animation for plant care.                                     //
//                                                                            //
// CPE 481 Fall 2016                                                          //
// Brittany McGarr                                                            //
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System.Collections;

public class tool_wateringcan : MonoBehaviour {
	public Animator animator;
	
	// Use this for initialization
	void Start () {
		animator.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	// Public call to animate
	public void animateTool() {
		animator.enabled = true;
		Destroy(gameObject);
	}
}
