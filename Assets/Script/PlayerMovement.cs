using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    public float speed = 10f;

    private Rigidbody2D rigidBody;

    private Animator animator;

    private float inputX, inputY;

    private Vector3 offset;

    private bool isTigger;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        offset = Camera.main.transform.position - transform.position;
    }


    private void FixedUpdate()
    {
        if (!isTigger)
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(inputX, inputY).normalized;
            rigidBody.velocity = input * speed;

            Camera.main.transform.position = transform.position + offset;

            if (inputX != 0)
            {
                transform.localScale = new Vector3(inputX, 1, 1);
            }
        }
        else
        {
            rigidBody.velocity = new Vector3(0,0,0);
        }
        
    }

    public void Trigger()
    {
        isTigger = true;
    }

}
