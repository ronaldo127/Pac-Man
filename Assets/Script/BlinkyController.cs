using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkyController : MonoBehaviour {
	
	public float speed = 1.0f;

	private Animator animator;

	protected PlayerController target;

	public Vector2 moveDirection = Vector2.right;

	public LayerMask layer;


	// Use this for initialization
	void Start () {
		target = GameObject.FindObjectOfType<PlayerController> ();
		animator = GetComponent<Animator> ();
		ChooseDirection ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{
		HandleCollision2D(collision);
	}

	private void OnCollisionStay2D(Collision2D collision)
	{
		HandleCollision2D(collision);
	}

	private void HandleCollision2D(Collision2D collision) {
		if (collision.gameObject.CompareTag("Wall"))
		{
			animator.SetBool ("IsMoving", false);
		}
	}


	private void FixedUpdate()
	{
		transform.position += new Vector3(moveDirection.x, moveDirection.y) * speed * Time.deltaTime;
	}

	protected void ChooseDirection(){



		Vector3 diff = target.transform.position - transform.position;
		Vector3 verticalComponent = Vector3.Project (diff, Vector3.up);
		Vector3 horizontalComponent = Vector3.Project (diff, Vector3.right);
		if (verticalComponent.magnitude > horizontalComponent.magnitude) {
			moveDirection = verticalComponent.normalized;
		} else {
			moveDirection = horizontalComponent.normalized;
		}
	}
}
