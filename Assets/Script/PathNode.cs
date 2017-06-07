using System;
using UnityEngine;
using System.Collections.Generic;

public class PathNode: IComparable<PathNode>
{
	public GameObject pathCross;

	public bool isOpen;

	public PathNode parent;

	public float cost;

	public Vector2[] Directions;

	public PathNode(){
		Directions = new Vector2[]{Vector2.up, Vector2.right, Vector2.down, Vector2.left};
	}

	public PathNode (GameObject path):this()
	{
		this.pathCross = path;
		PathCross temp = pathCross.GetComponent<PathCross> ();
		if (temp){
			List<Vector2> tempList = new List<Vector2> ();
			foreach(PathCross adjacent in temp.adjacents){
				Vector3 diff = adjacent.transform.position - path.transform.position;
				Vector3 verticalComponent = Vector3.Project (diff, Vector3.up);
				Vector3 horizontalComponent = Vector3.Project (diff, Vector3.right);
				if (verticalComponent.magnitude > horizontalComponent.magnitude) {
					tempList.Add(verticalComponent.normalized);
				} else {
					tempList.Add(horizontalComponent.normalized);
				}
			}
			Directions = tempList.ToArray ();
		}
	}

	public PathNode (GameObject path, PathNode parent):this(path)
	{
		this.parent = parent;
	}

	public int CompareTo(PathNode other){
		if (other.cost > this.cost)
			return -1;
		else if (other.cost < this.cost)
			return 1;
		else {
			if (other == this)
				return 0;
			return 1;
		}
	}

	public void Clear(){
		if (pathCross.CompareTag ("Cross")) {
			PathCross temp = pathCross.GetComponent<PathCross> ();
			if (temp) {
				temp.Color = Color.white;
			}
		}
	}
	public void Use(){
		if (pathCross.CompareTag ("Cross")) {
			PathCross temp = pathCross.GetComponent<PathCross> ();
			if (temp) {
				temp.Color = Color.green;
			}
		}
	}
	public void Close(){
		isOpen = false;
		if (pathCross.CompareTag ("Cross")) {
			PathCross temp = pathCross.GetComponent<PathCross> ();
			if (temp) {
				temp.Color = Color.red;
			}
		}
	}
}

