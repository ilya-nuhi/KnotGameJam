using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Player")]
    public float runSpeed = 10f;
    [SerializeField] float jumpSpeed = 20f;
    [SerializeField] float climbSpeed = 5f;
    [SerializeField] Vector2 deathKick = new Vector2 (10f,10f);

    [Header("Audio")]
    [SerializeField] AudioClip whipSFX;
    [SerializeField] AudioClip bounceSFX;

    [Header("References")]
    [SerializeField] BoxCollider2D whipCollider;
    

    public Vector2 moveInput;
    Rigidbody2D myRigidBody;
    public Animator myAnimator;
    
    float gravityScaleAtStart;

    public int jumpCount;

    CapsuleCollider2D myBodyCollider;
    
    BoxCollider2D myFeetCollider;

    SpriteRenderer mySprite;

    public Action onDash;
    public Action onThrowRope;
    float timer = 0f;
    bool isAlive = true;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
        mySprite = GetComponent<SpriteRenderer>();
    
    }

    void Update()
    {
        if(!isAlive){return;}
        timer-=Time.deltaTime;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
    }

    void ClimbLadder()
    {
        if(!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing"))){
            myRigidBody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing",false);    
            return;
        }
        myRigidBody.gravityScale = 0f;
        Vector2 playerVelocity = new Vector2( myRigidBody.velocity.x ,moveInput.y * climbSpeed);
        myRigidBody.velocity = playerVelocity;
        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("isClimbing",playerHasVerticalSpeed);
    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed){
            transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x),1f);
        }
        
    }

    void OnMove(InputValue value){
        if(!isAlive){return;}
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputValue value){
        if(!isAlive){return;}
        if(myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))){
            jumpCount = 1;
        }
        if(value.isPressed && jumpCount>=1){
            jumpCount--;
            myRigidBody.velocity += new Vector2 (0,jumpSpeed);
            if(Protrusion.isConnectedCheck){
                onThrowRope?.Invoke();
            }
        }
    }

    void OnFire(InputValue value){
        if(!isAlive){return;}
        if(value.isPressed && timer <=0){
            timer=1.5f;
            myAnimator.SetTrigger("Whipping");
            GetComponent<AudioSource>().PlayOneShot(whipSFX);
            DetectAndDestroyEnemies();
        }
    }

    void OnBackDash(InputValue value){
        if(!isAlive){return;}
        if(value.isPressed){
            onDash?.Invoke();
        }
    }

    void OnThrowRope(InputValue value){
        if(!isAlive){return;}
        if(value.isPressed){
            onThrowRope?.Invoke();
        }
    }

    void Run(){
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    void Die()
    {
        if(myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies","Hazard"))){
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidBody.velocity = deathKick;
            //mySprite.color = new Color (1,0.325f,0.325f,1);
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
        }
    }

    void DetectAndDestroyEnemies()
    {
        Vector2 rayDirection = Vector2.right;
        float rayDistance = 5f;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, rayDirection * MathF.Sign(transform.localScale.x),
                                             rayDistance, LayerMask.GetMask("Enemies"));

        if (hit.collider != null)
        {
            hit.collider.GetComponent<EnemyMovement>().EnemyDie();
        }
    }


    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag=="Bouncing"){
            AudioSource.PlayClipAtPoint(bounceSFX, other.gameObject.transform.position);
            jumpCount++;
            Vector2 bounceDirection = other.transform.up;
            Debug.Log(bounceDirection);
            myRigidBody.AddForce(bounceDirection * 100f, ForceMode2D.Impulse);
        }
    }
}
