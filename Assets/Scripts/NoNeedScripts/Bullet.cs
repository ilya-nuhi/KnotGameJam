using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float bulletSpeed = 20f;

    [SerializeField] AudioClip slimeDeathSFX;
    [SerializeField] float volumeSFX=75;
    Rigidbody2D myRigidBody;
    float xSpeed;
    PlayerMovement player;
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<PlayerMovement>();
        xSpeed = player.transform.localScale.x * bulletSpeed;
    }

    void Update()
    {
        myRigidBody.velocity = new Vector2 (xSpeed,0f);

    }

    void OnTriggerEnter2D(Collider2D other) {
        if(other.tag == "Enemy" ){
            AudioSource.PlayClipAtPoint(slimeDeathSFX,gameObject.transform.position, volumeSFX);
            Destroy(other.gameObject);
        }
        Destroy(gameObject);
    }
}
