using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class playerMovement : MonoBehaviour
{
    Vector2 moveInput;
    Rigidbody2D playerRB;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] float dashSpeed = 20f;
    float tempGravity;
    Animator animator;
    CapsuleCollider2D playerCollider;
    BoxCollider2D playerFeetCollider;
    AudioSource jumpSound;
    float dashCooldown = 2f;
    bool dashAble = true;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        jumpSound = GetComponent<AudioSource>();
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
        // if(playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
        //     if(moveInput.x < 0 && transform.localScale.x < 0){
        //         animator.SetBool("isWalking", false);
        //         return ;
        //     }else if(moveInput.x > 0 && transform.localScale.x > 0){
        //         animator.SetBool("isWalking", false);
        //         return;
        //     }
        // }

        bool walkingTrue = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        Vector2 playerVelocity = new Vector2(moveInput.x * speed, playerRB.velocity.y);
        playerRB.velocity = playerVelocity;
        
        animator.SetBool("isWalking", walkingTrue);
        
    }

    void OnDash(InputValue value){
        if(value.isPressed && dashAble){
            Vector2 dashDirection = new Vector2(moveInput.x, 0f).normalized;
            playerRB.velocity = dashDirection * dashSpeed;
            dashAble = false;
            StartCoroutine(DashCooldown());
        }
    }

    IEnumerator DashCooldown(){
        yield return new WaitForSeconds(dashCooldown);
        dashAble = true;
    }

    void flipSprite(){
        // if(playerCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
        //     return;
        // }

        bool horizontalSpeed = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        if(horizontalSpeed == true){
            transform.localScale = new Vector2(Mathf.Sign(playerRB.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value){
        if(!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        if(value.isPressed){
            jumpSound.Play();
            playerRB.velocity += new Vector2(0f, jumpForce);
        }
    }

    void Climbingladder(){
        if(!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
            playerRB.gravityScale = tempGravity;
            return;
        }else{
            Vector2 climbVelocity = new Vector2(playerRB.velocity.x, moveInput.y * climbSpeed);
            playerRB.velocity = climbVelocity;
            playerRB.gravityScale = 0f;
        }
    }
}
