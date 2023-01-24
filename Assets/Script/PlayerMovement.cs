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

    private bool isTrigger;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        offset = Camera.main.transform.position - transform.position;
    }


    private void FixedUpdate()
    {
        if (!isTrigger)
        {
            inputX = Input.GetAxisRaw("Horizontal");
            inputY = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(inputX, inputY).normalized;
            rigidBody.velocity = input * speed;

            rigidBody.position = new Vector2(
                Mathf.Clamp(rigidBody.position.x, -11, 15),
                Mathf.Clamp(rigidBody.position.y, -21, 5));

            Camera.main.transform.position = transform.position + offset;

            if (inputX != 0)
            {
                transform.localScale = new Vector3(inputX, 1, 1);
                animator.Play("run");
                //animator.SetBool("Run", true);
            }

            if (inputY != 0)
            {
                animator.Play("run");
                //animator.SetBool("Run", true);
            }

            if (inputX == 0 && inputY == 0)
            {
                animator.Play("idle");
                //animator.SetBool("Run", false);
            }


        }
        else
        {
            rigidBody.velocity = new Vector3(0,0,0);
            animator.SetBool("Run", false);
        }


        
    }

    public void Trigger()
    {
        isTrigger = true;
    }

}
