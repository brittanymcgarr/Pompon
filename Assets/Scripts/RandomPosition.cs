using UnityEngine;
using System.Collections;

// Random position mover for the beach_crab
public class RandomPosition : MonoBehaviour {
	public float timer;
	public int newtarget;
	public float speed;
	public int range;
	public NavMeshAgent nav;
	public Vector3 Target;

	// Use this for initialization
	void Start () {
		nav = gameObject.GetComponent<NavMeshAgent>();
		Random.seed = (int)System.DateTime.Now.Ticks;
	}
	
	// Creating a new random position
	void Update() {
		timer += Time.deltaTime;
		
		if(timer >= newtarget) {
			newTarget();
			timer = 0;
		}
	}
	
	void newTarget() {
		float x = gameObject.transform.position.x;
		float z = gameObject.transform.position.z;
		
		float xPos = x + Random.Range(x - range, x + range);
		float zPos = z + Random.Range(z - range, z + range);
		
		Target = new Vector3(xPos, gameObject.transform.position.y, zPos);
		
		nav.SetDestination(Target);
	}
}
