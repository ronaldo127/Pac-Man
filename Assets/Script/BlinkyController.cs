using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : MonoBehaviour {
	
	public float speed = 1.0f;

//	private Animator animator;

	protected PlayerController target;

	public Vector2 moveDirection = Vector2.right;

	public LayerMask layer;

	private PathCross[] pathCrosses;

	private Dictionary<string, PathNode> pathNodes;

	private bool isChoosingPath = false;



	// Use this for initialization
	void Start () {
		target = GameObject.FindObjectOfType<PlayerController> ();
//		animator = GetComponent<Animator> ();
		pathCrosses = GameObject.FindObjectsOfType<PathCross> ();
		ChooseDirection (this.name);
	}

	protected void ClearPathNodes(){
		if (pathNodes == null) {
			pathNodes = new Dictionary<string, PathNode> ();
			foreach (PathCross path in pathCrosses) {
				pathNodes.Add (path.name, new PathNode(path.gameObject));
			}
			pathNodes.Add (this.name, new PathNode (this.gameObject));
			pathNodes.Add (target.name, new PathNode (target.gameObject));
		}
		foreach (PathNode node in pathNodes.Values) {
			node.isOpen = true;
			node.cost = 0.0f;
			node.parent = null;
			node.Clear ();
		}
	}
	
	// Update is called once per frame
	void Update () {
	}



	private void OnTriggerStay2D(Collider2D collision)
	{
		HandleTrigger2D(collision);
	}

	private void HandleTrigger2D(Collider2D collider) {
		if (collider.gameObject.CompareTag ("Cross")&&(collider.transform.position-transform.position).magnitude<.5f&&!isChoosingPath) {
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

		pathNodes [currentTrigger].isOpen = false;

		PathNode thisNode = pathNodes [this.name];
		thisNode.isOpen = false;
		thisNode.cost = 0;
		thisNode.parent = null;
		priorityQueue.Add (thisNode, thisNode);

		PathNode path = null;
		while(priorityQueue.Count>0){
			PathNode currentNode = priorityQueue.Values [0];
			priorityQueue.RemoveAt (0);
			print ("Current node: "+currentNode.pathCross.name+" "+currentNode.pathCross.transform.position.ToString()+" "+currentNode.cost);
			if (currentNode.pathCross.CompareTag ("Player") && (path==null || currentNode.cost<path.cost)) {
				path = currentNode;
			}
			foreach (Vector2 direction in directions) {
				Transform currentNodeTransform = currentNode.pathCross.transform;
				Collider2D collider = Physics2D.Raycast ((Vector2)currentNodeTransform.position+direction, 
															direction, 30.0f, layer).collider;
				Vector3 prevPos = currentNode.pathCross.transform.position;
				if (collider) {
					if (pathNodes.ContainsKey(collider.name)){
						PathNode node = pathNodes [collider.name];
						if (node.isOpen) {
							node.isOpen = false;
							node.cost = currentNode.cost + (collider.transform.position - prevPos).magnitude;
							node.parent = currentNode;
							print ("Adding: "+node.pathCross.name+" "+node.pathCross.transform.position.ToString()+" "+node.cost);
							priorityQueue.Add (node, node);
						}
					}
				}
			}
		}

		while (path.parent != thisNode) {
			print ("reverse path: " + path.pathCross.name);
			path.Use ();
			path = path.parent;
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
		isChoosingPath = false;
	}
}
