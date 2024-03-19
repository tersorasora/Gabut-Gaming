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
    [SerializeField] TrailRenderer dashTrail;
    [SerializeField] AudioClip jumpSfx, dashSfx, walkSfx;
    float tempGravity;
    Animator animator;
    CapsuleCollider2D playerCollider;
    BoxCollider2D playerFeetCollider;
    public AudioSource audioSource;
    float dashCooldown = 2f;
    float dashTime = 0.2f;
    bool dashAble = true;
    bool isDashing;

    void Start()
    {
        playerRB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<CapsuleCollider2D>();
        playerFeetCollider = GetComponent<BoxCollider2D>();
        tempGravity = playerRB.gravityScale;
    }

    
    void Update()
    {
        if(isDashing){
            return;
        }
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
        if(walkingTrue && playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            if(!audioSource.isPlaying){
                audioSource.clip = walkSfx;
                audioSource.Play();
            }
        }else{
            audioSource.Stop();
        }
    }

    void OnDash(InputValue value){
        if(value.isPressed && dashAble && moveInput != Vector2.zero){
            // Debug.Log("Dash");
            animator.SetBool("isWalking", false);
            StartCoroutine(DashNow());
        }
    }

    IEnumerator DashNow(){
        float originalGravity = playerRB.gravityScale; // supaya kalo dash nya diatas agak ngambang bentar
        playerRB.gravityScale = 0f;
        dashAble = false; // supaya gk bisa dash lagi
        isDashing = true; // lagi ngedash
        Vector2 playerDashVelocity = new Vector2(moveInput.x * dashSpeed, 0f);
        audioSource.clip = dashSfx;
        audioSource.Play();
        playerRB.velocity = playerDashVelocity;
        dashTrail.emitting = true; // trailnya muncul
        animator.SetTrigger("isDashing"); // animasi dash
        yield return new WaitForSeconds(dashTime); // berapa lama waktu ngedashnya
        animator.ResetTrigger("isDashing"); // reset animasi
        playerRB.gravityScale = originalGravity; // balikin gravity ke semula
        isDashing = false; // gk lagi ngedash
        dashTrail.emitting = false; // trailnya ilang
        yield return new WaitForSeconds(dashCooldown); // berapa lama cooldownnya
        dashAble = true;
    }

    void flipSprite(){
        bool horizontalSpeed = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        if(horizontalSpeed == true){
            transform.localScale = new Vector2(Mathf.Sign(playerRB.velocity.x), 1f);
        }
    }

    void OnJump(InputValue value){
        if(!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        else if(value.isPressed){
            jump();
        }
    }

    void jump(){
        audioSource.clip = jumpSfx;
        audioSource.Play();
        playerRB.velocity += new Vector2(0f, jumpForce);
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
