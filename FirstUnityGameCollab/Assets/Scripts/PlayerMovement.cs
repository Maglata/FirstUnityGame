using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float MoveSpeed = 5f;
    public float Jump = 2f;
    private bool Canjump;
    private Rigidbody2D rigidbodycomponent;
    private bool IsGrounded;

    // Start is called before the first frame update
    void Start()
    {
        rigidbodycomponent = GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W))
            Canjump = true;

        if (Input.GetKey(KeyCode.D))
            rigidbodycomponent.velocity = new Vector2(MoveSpeed, rigidbodycomponent.velocity.y);

        if (Input.GetKey(KeyCode.A))
            rigidbodycomponent.velocity = new Vector2(-MoveSpeed, rigidbodycomponent.velocity.y);


    }


    // FixedUpdate is called once every physic update
    private void FixedUpdate()
    {
        if (Canjump)
        {
            if (!IsGrounded)
                return;

            rigidbodycomponent.AddForce(Vector2.up * Jump, ForceMode2D.Impulse);
            Canjump = false;
        }
            

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        IsGrounded = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        IsGrounded = false;
    }
}
