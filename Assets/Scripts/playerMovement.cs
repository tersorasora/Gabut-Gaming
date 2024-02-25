using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D playerRB;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float climbSpeed = 2f;
    float tempGravity;
    Animator animator;
    CapsuleCollider2D playerCollider;

    private bool isJumping = false;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        tempGravity = playerRB.gravityScale;
    }

    
    void Update()
    {
        Walk();
        flipSprite();
        Climbingladder();
    }

    void OnMove(InputValue value){
        moveInput = value.Get<Vector2>();
    }

    void Walk(){
        bool walkingTrue = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        Vector2 playerVelocity = new Vector2(moveInput.x * speed, playerRB.velocity.y);
        playerRB.velocity = playerVelocity;
        
        animator.SetBool("isWalking", walkingTrue);
    }

    void OnJump(InputValue value){
        if(value.isPressed && !isJumping){
            playerRB.velocity += new Vector2(0f, jumpForce);
            isJumping = true;
        }
    }

    void flipSprite(){
        bool horizontalSpeed = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        if(horizontalSpeed == true){
            transform.localScale = new Vector2(Mathf.Sign(playerRB.velocity.x), 1f);
        }
    }

    void Climbingladder(){
        if(!playerCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
            playerRB.gravityScale = tempGravity;
            return;
        }else{
            Vector2 climbVelocity = new Vector2(playerRB.velocity.x, moveInput.y * climbSpeed);
            playerRB.velocity = climbVelocity;
            playerRB.gravityScale = 0f;
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.CompareTag("Ground")){
            isJumping = false;
        }
    }
}
