using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Teleporter : MonoBehaviour {
	
	public GameObject arrival;

	void OnTriggerEnter2D(Collider2D collider){
		collider.transform.position = arrival.transform.position;
	}
}
