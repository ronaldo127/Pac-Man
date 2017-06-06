using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlinkyController : MonoBehaviour {
	
	public float speed = 1.0f;

//	private Animator animator;

	protected PlayerController target;

	public Vector2 moveDirection = Vector2.right;

	public LayerMask layer;

	private PathCross[] pathCrosses;

	private Hashtable pathNodes;

	private bool isChoosingPath = false;

	private Vector3 targetPosition;

	private PathNode nextNode;

	// Use this for initialization
	void Start () {
		target = GameObject.FindObjectOfType<PlayerController> ();
//		animator = GetComponent<Animator> ();
		pathCrosses = GameObject.FindObjectsOfType<PathCross> ();
		ChooseDirection (this.name);
	}

	protected void ClearPathNodes(){
		if (pathNodes == null) {
			pathNodes = new Hashtable ();
			foreach (PathCross path in pathCrosses) {
				pathNodes.Add (path.name, new PathNode(path.gameObject));
			}
			pathNodes.Add (this.name, new PathNode (this.gameObject));
			pathNodes.Add (target.name, new PathNode (target.gameObject));
		}
		foreach (PathNode node in pathNodes.Values) {
			if (!node.isOpen) {
				node.isOpen = true;
				node.cost = 0.0f;
				node.parent = null;
				node.Clear ();
			}
		}
		targetPosition = target.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	}



	private void OnTriggerStay2D(Collider2D collision)
	{
		HandleTrigger2D(collision);
	}

	private void HandleTrigger2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cross")&&(collider.transform.position-transform.position).magnitude<.5f) {
			if (!isChoosingPath)
				ChooseDirection (collider.name);
		}
	}


	private void FixedUpdate()
	{
		transform.position += new Vector3(moveDirection.x, moveDirection.y) * speed * Time.deltaTime;
	}

	protected void ChooseDirection(string currentTrigger){
		isChoosingPath = true;
		ClearPathNodes ();

		SortedList<PathNode, PathNode> priorityQueue = new SortedList<PathNode, PathNode>();

		Vector2[] directions = new Vector2[]{Vector2.up, Vector2.right, Vector2.down, Vector2.left};

		((PathNode)pathNodes [currentTrigger]).isOpen = false;

		PathNode thisNode = ((PathNode)pathNodes [this.name]);
		thisNode.isOpen = false;
		thisNode.cost = 0;
		thisNode.parent = null;
		priorityQueue.Add (thisNode, thisNode);

		PathNode path = null;
		while(priorityQueue.Count>0){
			PathNode currentNode = priorityQueue.Values [0];
			priorityQueue.RemoveAt (0);
			print ("Current node: "+currentNode.pathCross.name+" "+currentNode.pathCross.transform.position.ToString()+" "+currentNode.cost);
			if (path!=null && path.cost<currentNode.cost)
				continue;
			if (currentNode.pathCross.CompareTag ("Player") && (path == null || currentNode.cost < path.cost)) {
				path = currentNode;
			} else {
				foreach (Vector2 direction in currentNode.Directions) {
					Transform currentNodeTransform = currentNode.pathCross.transform;
					Collider2D collider = Physics2D.Raycast ((Vector2)currentNodeTransform.position+direction, 
						direction, 25.0f, layer).collider;
					Vector3 prevPos = currentNode.pathCross.transform.position;
					if (collider) {
						Vector3 nextPos;
						if (collider.tag.Equals("Cross") || collider.tag.Equals("Player")){
							PathNode node = ((PathNode)pathNodes [collider.name]);
							if (node.isOpen) {
								if (collider.tag.Equals ("Player"))
									nextPos = targetPosition;
								else
									nextPos = collider.transform.position;
								float newCost = currentNode.cost + (collider.transform.position - prevPos).magnitude;
								if (path!=null && path.cost<newCost)
									continue;
								node.isOpen = false;
								node.cost = newCost;
								node.parent = currentNode;
								print ("Adding: "+node.pathCross.name+" "+node.pathCross.transform.position.ToString()+" "+node.cost);
								priorityQueue.Add (node, node);
							}
						}
					}
				}
			}
		}
		if (path == null) {
			path = nextNode;
		} else {
			while (path.parent != thisNode) {
				print ("reverse path: " + path.pathCross.name);
				path.Use ();
				nextNode = path;
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
		print (moveDirection);
		//Vector2 temp = (Vector2)path.pathCross.transform.position;
		//this.transform.position = (Vector3)Physics2D.Raycast (temp - moveDirection, -((Vector2)moveDirection), 10.0f, layer).collider.transform.position;
		isChoosingPath = false;
	}
}
