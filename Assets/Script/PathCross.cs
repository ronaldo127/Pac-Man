using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCross : MonoBehaviour {
	public LayerMask layer;
	public PathCross[] adjacents;
	public bool IsPath{ set; get;}

	// Use this for initialization
	void Start () {
		List<PathCross> list = new List<PathCross> ();
		Vector3 pos = transform.position;
		Vector3 scale = transform.lossyScale;

		Vector3[] directions = new Vector3[]{ Vector3.up, Vector3.right, Vector3.left, Vector3.down };
		foreach (Vector3 direction in directions){
			Collider2D collider = Physics2D.Raycast (pos+direction*scale.y, direction, 100.0f, layer).collider;
			PathCross temp;
			if (collider != null) {
				if (collider.gameObject.CompareTag ("Player")) {
					collider = Physics2D.Raycast (
						collider.gameObject.transform.position + direction*5.0f, 
						direction, 100.0f, layer).collider;
				}
				if ((temp = collider.GetComponent<PathCross>())!=null)
					list.Add (temp);
			}
		}

		adjacents = list.ToArray ();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDrawGizmos(){
		if (IsPath)
			Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position,transform.localScale);
	}
}
