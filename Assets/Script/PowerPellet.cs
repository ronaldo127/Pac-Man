using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPellet : MonoBehaviour {

	private BlinkyController blinkController;

	// Use this for initialization
	void Start () {
		blinkController = GameObject.FindObjectOfType<BlinkyController> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter2D(Collision2D collision){
		if (collision.gameObject.CompareTag("Player")){
			blinkController.Reverse();
			Destroy (this.gameObject);
		}
	}
}
