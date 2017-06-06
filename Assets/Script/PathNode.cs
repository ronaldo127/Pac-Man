using System;
using UnityEngine;

public class PathNode: IComparable<PathNode>
{
	public GameObject pathCross;

	public bool isOpen;

	public PathNode parent;

	public float cost;

	public PathNode(){
	
	}

	public PathNode (GameObject path)
	{
		this.pathCross = path;
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
			pathCross.GetComponent<PathCross> ().isPath = false;
		}
	}
	public void Use(){
		if (pathCross.CompareTag ("Cross")) {
			pathCross.GetComponent<PathCross> ().isPath = true;
		}
	}
}

