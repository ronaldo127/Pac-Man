﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlinkyController : MonoBehaviour {
	
	public float speed = 1.0f;

	public Vector2 moveDirection = Vector2.right;

	public LayerMask layer;

	public bool isReverse;

	public PathCross respawn;

	protected PlayerController target;

	private PathCross[] pathCrosses;

	private Hashtable pathNodes;

	private bool isChoosingPath = false;

	private Vector3 targetPosition;

	private Stack<PathNode> pathStack;

	private string lastMarkerName;

	private PathNode targetNode;

	private Animator animator;

	private bool isDead;

	// Use this for initialization
	void Start () {
		target = GameObject.FindObjectOfType<PlayerController> ();
		animator = GetComponent<Animator> ();
		pathCrosses = GameObject.FindObjectsOfType<PathCross> ();
	}

	protected void ClearPathNodes(){
		if (pathNodes == null) {
			pathNodes = new Hashtable ();
			foreach (PathCross path in pathCrosses) {
				pathNodes.Add (path.name, new PathNode(path.gameObject));
			}
			pathNodes.Add (this.name, new PathNode (this.gameObject));
			targetNode = new PathNode (target.gameObject);
			pathNodes.Add (target.name, targetNode);
			pathStack = new Stack<PathNode> ();
		}
		foreach (PathNode node in pathNodes.Values) {
			if (!node.isOpen) {
				node.isOpen = true;
				node.cost = 0.0f;
				node.parent = null;
				node.Clear ();
			}
		}
		targetPosition = targetNode.pathCross.transform.position;
		targetPosition = new Vector3 (Mathf.Round(targetPosition.x), Mathf.Round(targetPosition.y));
	}
	
	// Update is called once per frame
	void Update () {
	}



	void OnTriggerStay2D(Collider2D collision)
	{
		HandleTrigger2D(collision);
	}


	private void HandleTrigger2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cross")&&(collider.transform.position-this.transform.position).magnitude<.5f) {
			if (!isChoosingPath && collider.name != lastMarkerName) {
				ChooseDirection (this.name, collider.name);
				lastMarkerName = collider.name;
			}
			lastMarkerName = collider.name;
			if (collider.name == targetNode.pathCross.name) {
				Respawn();
			}
		}
		if (collider.gameObject.CompareTag("Player")) {
			if (!isDead && isReverse) {
				Die ();
			}
		}
	}

	private void Die(){
		isDead = true;
		targetNode = (PathNode)pathNodes [respawn.name];
		isReverse = false;
		animator.SetBool ("Dead", isDead);
	}

	private void Respawn(){
		isDead = false;
		Follow ();
		targetNode = (PathNode)pathNodes [target.name];
		animator.SetBool ("Dead", isDead);
	}

	private void GoToNextDirection(){
		if (pathStack == null || pathStack.Count < 1)
			return;
		PathNode path = pathStack.Pop ();
		Vector3 diff2 = path.pathCross.transform.position - transform.position;
		Vector3 verticalComponent = Vector3.Project (diff2, Vector3.up);
		Vector3 horizontalComponent = Vector3.Project (diff2, Vector3.right);
		if (verticalComponent.magnitude > horizontalComponent.magnitude) {
			moveDirection = verticalComponent.normalized;
		} else {
			moveDirection = horizontalComponent.normalized;
		}
	}


	private void FixedUpdate()
	{
		transform.position += new Vector3(moveDirection.x, moveDirection.y) * speed * Time.deltaTime;
	}

	protected void ChooseDirection(string currentTrigger, string markerName){
		isChoosingPath = true;
		bool isReversed = this.isReverse;
		ClearPathNodes ();

		SortedList<PathNode, PathNode> priorityQueue = new SortedList<PathNode, PathNode>();


		((PathNode)pathNodes [currentTrigger]).isOpen = false;

		PathNode thisNode = ((PathNode)pathNodes [this.name]);
		thisNode.isOpen = false;
		thisNode.cost = 0;
		thisNode.parent = null;
		priorityQueue.Add (thisNode, thisNode);

		PathNode path = null;
		while(priorityQueue.Count>0){
			PathNode currentNode;
			if (isReversed) {
				currentNode = priorityQueue.Values [priorityQueue.Values.Count-1];
				priorityQueue.RemoveAt (priorityQueue.Values.Count-1);
			} else {
				currentNode = priorityQueue.Values [0];
				priorityQueue.RemoveAt (0);
			}
			print ("Current node: "+currentNode.pathCross.name+" "+currentNode.Position.ToString()+" "+currentNode.cost);
			if (currentNode==targetNode && (path == null || currentNode.value < path.value)) {
				path = currentNode;
				break;
			} else {
				foreach (Vector2 direction in currentNode.Directions) {
					Transform currentNodeTransform = currentNode.pathCross.transform;
					Collider2D collider = Physics2D.Raycast ((Vector2)currentNode.Position+direction, 
						direction, 25.0f, layer).collider;
					Vector3 prevPos = currentNode.Position;
					if (collider) {
						Vector3 nextPos;
						if (collider.tag.Equals("Cross") || collider.tag.Equals("Player")){
							PathNode node = ((PathNode)pathNodes [collider.name]);
							if (node.isOpen) {
								if (collider.tag.Equals ("Player"))
									nextPos = targetPosition;
								else
									nextPos = node.Position;
								float newCost = currentNode.cost + (node.Position - prevPos).magnitude;
								node.cost = newCost;
								Vector2 diff = targetPosition - node.Position;
								node.heuristic = Mathf.Abs(diff.x) + Mathf.Abs(diff.y);
								node.parent = currentNode;
								node.Close();
								print ("Adding: "+node.pathCross.name+" "+node.Position.ToString()+" "+node.cost);
								priorityQueue.Add (node, node);
							}
						}
					}
				}
			}
		}
		if (path == null) {
			if (pathStack.Count > 0)
				path = pathStack.Pop ();
			else {
				path = (PathNode)pathNodes[markerName];
				List<Vector2> possibleDirections = new List<Vector2> ();
				foreach (Vector2 direction in path.Directions) {
					possibleDirections.Add(direction);
				}
				moveDirection = possibleDirections[UnityEngine.Random.Range(0, possibleDirections.Count)];
				return;
			}
		} else {
			pathStack.Clear ();
			while (path.parent != thisNode) {
				print ("reverse path: " + path.pathCross.name);
				path.Use ();
				pathStack.Push (path);
				path = path.parent;
			}
		}
		print ("reverse path: " + path.pathCross.name);
		path.Use ();
		
		Vector3 diff2 = path.pathCross.transform.position - transform.position;
		Vector3 verticalComponent = Vector3.Project (diff2, Vector3.up);
		Vector3 horizontalComponent = Vector3.Project (diff2, Vector3.right);
		if (verticalComponent.magnitude > horizontalComponent.magnitude) {
			moveDirection = verticalComponent.normalized;
		} else {
			moveDirection = horizontalComponent.normalized;
		}
		this.transform.position = ((PathNode)pathNodes [markerName]).pathCross.transform.position;
		print (moveDirection);
		isChoosingPath = false;
	}

	public void Reverse(){
		if (isReverse){
			CancelInvoke ("Follow");
		}
		isReverse = true;
		animator.SetBool ("Reverse", isReverse);
		//ChooseDirection (this.name, lastMarkerName);
		Invoke ("Follow", 5.0f);
	}

	private void Follow(){
		isReverse = false;
		animator.SetBool ("Reverse", isReverse);
		//ChooseDirection (this.name, lastMarkerName);
	}
}
