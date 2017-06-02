using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Vector2 moveDirection = Vector2.right;
    public float speed = 1.0f;

	private Animator animator;

	private DirectionEnum _direction;

	private DirectionEnum Direction{ 
		set{
			if (Direction != value && animator!=null)
				this.animator.SetTrigger ("DirectionChanged");
			_direction = value;
		} 
		get{
			return _direction;
		}
	}

    // Use this for initialization
    void Start () {
		animator = GetComponent<Animator> ();
		Direction = DirectionEnum.EAST;
	}

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0.0f)
        {
            if (horizontalInput > 0.0f)
            {
                moveDirection = Vector2.right;
				animator.SetInteger ("Direction", (int)(Direction = DirectionEnum.EAST));
            }
            else
            {
				moveDirection = Vector2.left;
				animator.SetInteger ("Direction", (int)(Direction = DirectionEnum.WEST));
            }
			animator.SetBool ("IsMoving", true);
        }
        float verticalInput = Input.GetAxis("Vertical");
        if (verticalInput != 0.0f)
        {
            if (verticalInput > 0.0f)
            {
				moveDirection = Vector2.up;
				animator.SetInteger ("Direction", (int)(Direction = DirectionEnum.NORTH));
            }
            else
            {
				moveDirection = Vector2.down;
				animator.SetInteger ("Direction", (int)(Direction = DirectionEnum.SOUTH));
			}
			animator.SetBool ("IsMoving", true);
        }

    }

    private void FixedUpdate()
    {
        transform.position += new Vector3(moveDirection.x, moveDirection.y) * speed * Time.deltaTime;
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
		if (collision.gameObject.CompareTag("Wall") && ((collision.contacts[0].normal + moveDirection).magnitude < 0.3f))
        {
			moveDirection = Vector2.zero;
			animator.SetBool ("IsMoving", false);
        }
    }
}
