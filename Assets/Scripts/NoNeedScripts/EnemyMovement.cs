using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    [SerializeField] float movementSpeed = 1f;
    [SerializeField] AudioClip deathSFX;
    Rigidbody2D myRigidBody;
    BoxCollider2D myBoxCollider;
    CircleCollider2D myCircleCollider;
    Animator enemyAnimator;
    bool isDead = false;
    AudioSource audioSource;
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        myRigidBody = GetComponent<Rigidbody2D>();
        myBoxCollider = GetComponent<BoxCollider2D>();
        myCircleCollider = GetComponent<CircleCollider2D>();
        enemyAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if(isDead){return;}
        myRigidBody.velocity = new Vector2 (Mathf.Sign(myRigidBody.transform.localScale.x)*movementSpeed,0f);
        FlipEnemyFacingHelper();
    }

    void FlipEnemyFacingHelper()
    {
        if(myBoxCollider.IsTouchingLayers(LayerMask.GetMask("Hazard","Ground"))){
            FlipEnemyFacing();
        }
        if(!myCircleCollider.IsTouchingLayers(LayerMask.GetMask("Ground","Climbing"))){
            FlipEnemyFacing();
        }
    }

    void FlipEnemyFacing()
    {
        transform.localScale = new Vector2 (-Mathf.Sign(myRigidBody.velocity.x),1f);
    }

    public void EnemyDie(){
        isDead = true;
        myRigidBody.velocity = Vector2.zero;
        GetComponent<CapsuleCollider2D>().enabled = false;
        myCircleCollider.enabled = false;
        myBoxCollider.enabled = false;
        StartCoroutine(Die());
    }

    IEnumerator Die(){
        enemyAnimator.SetTrigger("Die");
        audioSource.PlayOneShot(deathSFX);
        yield return new WaitForSeconds(5);
        Destroy(gameObject);
    }
}
