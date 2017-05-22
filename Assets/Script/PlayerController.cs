using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Vector2 moveDirection = Vector2.right;
    public float speed = 1.0f;

    // Use this for initialization
    void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (horizontalInput != 0.0f)
        {
            if (horizontalInput > 0.0f)
            {
                Debug.Log("Right");
                moveDirection = Vector2.right;
            }
            else
            {
                Debug.Log("Left");
                moveDirection = Vector2.left;
            }
        }
        float verticalInput = Input.GetAxis("Vertical");
        if (verticalInput != 0.0f)
        {
            if (verticalInput > 0.0f)
            {
                Debug.Log("Up");
                moveDirection = Vector2.up;
            }
            else
            {
                Debug.Log("Down");
                moveDirection = Vector2.down;
            }
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
        Debug.Log("collision  " + collision.contacts[0].normal.ToString());
        if ((collision.contacts[0].normal + moveDirection).magnitude < 0.3f)
        {
            moveDirection = Vector2.zero;
        }
    }
}
