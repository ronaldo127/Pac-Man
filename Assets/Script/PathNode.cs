using System;
using UnityEngine;

public class PathNode
{
	public PathCross pathCross;

	public bool isOpen;

	public PathNode parent;

	public float cost;

	public PathNode (PathCross pathCross)
	{
		this.pathCross = pathCross;
	}

	public PathNode (PathCross pathCross, PathNode parent):this(pathCross)
	{
		this.parent = parent;
	}
}

