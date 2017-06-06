using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCross : MonoBehaviour {
	public bool isPath;

	void OnDrawGizmos(){
		if (isPath)
			Gizmos.color = Color.red;
		Gizmos.DrawWireCube (transform.position,transform.localScale);
	}
}
