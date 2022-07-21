using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAI : MonoBehaviour
{
    private Rigidbody2D _body;
    private Animator _animator;
    public bool isPatrolling;
    private bool shouldFlip;
    public Transform groundCheck;
    public Collider2D bodyCollider;
    public LayerMask heroLayer;
    public LayerMask groundLayer;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
       isPatrolling = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPatrolling)
        {
            Patrol();
        }
    }

    private void FixedUpdate()
    {
        if (isPatrolling)
        {
            shouldFlip = !Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);
        }
    }

    private void Patrol()
    {
        if (shouldFlip || bodyCollider.IsTouchingLayers(groundLayer))
        {
            Flip();
        }

        if (bodyCollider.IsTouchingLayers(heroLayer))
        {
            _animator.SetInteger("AnimState", 0);
            return;
        }

        _body.velocity = new Vector2(-speed * Time.fixedDeltaTime, _body.velocity.y);
        _animator.SetInteger("AnimState", 2);
    }

    private void Flip()
    {
        isPatrolling = false;
        transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
        speed *= -1;
        isPatrolling = true;
    }
}
