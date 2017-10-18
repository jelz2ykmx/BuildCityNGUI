using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PerimeterColliderBomb : MonoBehaviour {		//used to color the grass red when in collision
		
	public int collisionCounter = 0;				//keeps track of how many other grass patches this one is overlapping

	private int 
	ghostZ = -9,
	explosionZ = -6,
	zeroZ = 0;

	public bool inCollision = false, isExploded = false;
	private List<GameObject> victims = new List<GameObject>();

	public GameObject ExplosionPf, Vortex, Grave, Ghost; 

	private SoundFX soundFX;
	private Helios helios;


	void Start () 
	{
		soundFX = GameObject.Find ("SoundFX").GetComponent <SoundFX>();
		helios = GameObject.Find ("Helios").GetComponent<Helios>();
	}

	void OnTriggerEnter(Collider collider)//OnCollisionEnter(Collision collision)
	{
		//print ("enter");
		//print ("collider.gameObject.tag");
		if (collider.gameObject.tag == "Unit"&&!isExploded) {	 
			collisionCounter++;						
			victims.Add (collider.gameObject);
			StartCoroutine ("Explode");
			inCollision = true;	
		}

	}

	void OnTriggerExit(Collider collider)//OnCollisionEnter(Collision collision)
	{
		if (collider.gameObject.tag == "Unit"&&!isExploded) {	 
			collisionCounter--;	
			victims.Remove(collider.gameObject);

			if(collisionCounter == 0)
			{	
				victims.Clear ();
				StopCoroutine ("Explode"); //bomb aborts explosion if targets have moved on
				inCollision = false;
			}
		}
	}

	private IEnumerator Explode()
	{
		yield return new WaitForSeconds (1.0f);
		isExploded = true;
		soundFX.BuildingExplode ();
		ExplosionPf.SetActive (true);	

		foreach (var unit in victims) {
			if (unit != null) 
			{
				unit.GetComponent<FighterController> ().life -= 100;

				if (unit.GetComponent<FighterController> ().life <= 0) 
				{
					Vector3 pos = unit.transform.position;

					Instantiate (Vortex, new Vector3 (pos.x, pos.y, explosionZ), Quaternion.identity);
					Instantiate (Grave, new Vector3 (pos.x, pos.y, zeroZ), Quaternion.identity);
					Instantiate (Ghost, new Vector3 (pos.x, pos.y, ghostZ), Quaternion.identity);

					((Helios)helios).KillUnit (unit.GetComponent<FighterController> ().assignedToGroup,
						unit.GetComponent<Selector> ().index);
					unit.GetComponent<FighterController> ().Hit ();
				}
			}
		}
		victims.Clear ();

		//StartCoroutine ("DestroySelf");
	}

	private IEnumerator DestroySelf()
	{
		yield return new WaitForSeconds (1.0f);
		Destroy (transform.parent.gameObject);		
	}


}
