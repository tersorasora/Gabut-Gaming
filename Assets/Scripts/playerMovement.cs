using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class playerMovement : MonoBehaviour
{
    Vector2 moveInput;
    int lastDirection = 1;
    Rigidbody2D playerRB;
    [SerializeField] float speed = 2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float climbSpeed = 2f;
    [SerializeField] float dashSpeed = 20f;
    [SerializeField] TrailRenderer dashTrail;
    [SerializeField] AudioClip jumpSfx, dashSfx, walkSfx, hitSfx, throwSfx;
    [SerializeField] GameObject Shuriken;
    [SerializeField] Transform ShurikenSpawnPoint;
    float tempGravity;
    Animator animator;
    CapsuleCollider2D playerCollider;
    BoxCollider2D playerFeetCollider;
    public AudioSource audioSource, walkAudioSource;
    float dashCooldown = 2f;
    float dashTime = 0.2f;
    bool dashAble = true;
    bool isDashing;
    bool isAlive = true;
    bool canShoot = true;

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
        if(!isAlive){
            return;
        }
        Walk();
        flipSprite();
        Climbingladder();
        die();
    }

    void OnMove(InputValue value){
        if(!isAlive){
            return;
        }
        moveInput = value.Get<Vector2>();
        if(moveInput.x != 0){
            lastDirection = (int)moveInput.x;
        }
    }

    void Walk(){
        bool walkingTrue = Mathf.Abs(playerRB.velocity.x) > Mathf.Epsilon;

        Vector2 playerVelocity = new Vector2(moveInput.x * speed, playerRB.velocity.y);
        playerRB.velocity = playerVelocity;
        animator.SetBool("isWalking", walkingTrue);
        if(walkingTrue && playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            if(!walkAudioSource.isPlaying){
                walkAudioSource.clip = walkSfx;
                walkAudioSource.Play();
            }
        }else{
            walkAudioSource.Stop();
        }
    }

    void OnFire(InputValue value){
        if(!isAlive){
            return;
        }
        if(value.isPressed && canShoot && !isDashing && animator.GetBool("isWalking") == false){
            audioSource.Stop();
            canShoot = false;
            animator.SetTrigger("isThrowing");
            audioSource.clip = throwSfx;
            audioSource.Play();
            StartCoroutine(projectilesDelay());
            StartCoroutine(resetThrow());
            StartCoroutine(waitShoot());
        }
    }

    IEnumerator projectilesDelay(){
        yield return new WaitForSeconds(0.2f);
        Instantiate(Shuriken, ShurikenSpawnPoint.position, transform.rotation);
    }

    IEnumerator resetThrow(){
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("stopThrowing");
    }

    IEnumerator waitShoot(){
        yield return new WaitForSeconds(3f);
        canShoot = true;
    }

    void OnDash(InputValue value){
        if(!isAlive){
            return;
        }
        if(value.isPressed && dashAble){
            animator.SetBool("isWalking", false);
            StartCoroutine(DashNow());
        }
    }

    IEnumerator DashNow(){
        float originalGravity = playerRB.gravityScale; // supaya kalo dash nya diatas agak ngambang bentar
        playerRB.gravityScale = 0f;
        dashAble = false; // supaya gk bisa dash lagi
        isDashing = true; // lagi ngedash
        Vector2 playerDashVelocity = new Vector2(lastDirection * dashSpeed, 0f);
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
        if(!isAlive){
            return;
        }
        if(!playerFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            return;
        }
        if(value.isPressed){
            jump();
        }
    }

    void jump(){
        audioSource.PlayOneShot(jumpSfx);
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

    void die(){
        if(playerCollider.IsTouchingLayers(LayerMask.GetMask("enemy", "Spikes"))){
            audioSource.clip = hitSfx;
            audioSource.Play();
            isAlive = false;
            animator.SetBool("isWalking", false);
            animator.SetTrigger("isDying");
        }
    }
}
