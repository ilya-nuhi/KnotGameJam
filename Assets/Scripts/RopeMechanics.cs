using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeMechanics : MonoBehaviour
{
    [Header("References")]
    public GameObject currentBaseChain;
    [SerializeField] GameObject chainPrefab;
    [SerializeField] Transform rope;
    [SerializeField] PlayerMovement player;
    [SerializeField] AudioClip ropeStretchSFX;
    
    [Header("Values")]
    public float tensionThreshhold = 20;
    [SerializeField] float pullingForce = 20f;
    SpringJoint2D baseSpringJoint;
    
    public static int chainCount = 0;
    public static int maxChainCount = 100;
    GameObject newChain;
    float waitForTension = 0;
    bool isTension = false;

    Transform firstChain;
    float playerBaseSpeed;
    bool canPull = true;
    AudioSource playerAudioSource;
    
    private void Awake() {

        baseSpringJoint = GetComponent<SpringJoint2D>();

    }

    private void OnEnable() {
        player.onDash += DoBackDash;
    }
    void Start()
    {
        chainCount = rope.childCount-1;
        firstChain = rope.GetChild(1);
        playerBaseSpeed = player.runSpeed;
        playerAudioSource = player.GetComponent<AudioSource>();
        if (playerAudioSource == null)
        {
            // Add an AudioSource if it doesn't exist
            playerAudioSource = player.gameObject.AddComponent<AudioSource>();
        }
    }

    private void DoBackDash()
    {
        if(canPull){
            player.myAnimator.SetTrigger("BackDash");
            StartCoroutine(SmoothPull(pullingForce*1.5f, 0.3f));
        }
    }

    private IEnumerator SmoothPull(float pullPower, float duration)
{
    canPull = false;
    Vector2 pullVector = new Vector2(rope.GetChild(8).transform.position.x - player.transform.position.x,
                                     rope.GetChild(8).transform.position.y - player.transform.position.y).normalized;
    float elapsed = 0f;
    Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    pullVector = (rb.velocity.normalized + pullVector*5).normalized;
    while (elapsed < duration)
    {
        float t = elapsed / duration;
        float currentSpeed = Mathf.Lerp(pullPower, 0, t);
        rb.velocity = Vector2.ClampMagnitude(pullVector * currentSpeed, pullPower);
        elapsed += Time.deltaTime;
        yield return null;
    }

    rb.velocity = Vector2.zero; // Ensure final velocity is zero

    yield return new WaitForSeconds(0.3f);

    canPull = true;
}

    void Update()
    {
        waitForTension -= Time.deltaTime;
        if(waitForTension<=0){
            waitForTension = 0.1f;
            isTension=false;
        }

        Vector2 reactionForce = baseSpringJoint.GetReactionForce(Time.deltaTime);
        if(reactionForce.magnitude > tensionThreshhold){
            isTension = true;
            if(chainCount < maxChainCount){
                newChain = Instantiate(chainPrefab, transform.position, Quaternion.identity);
                newChain.transform.parent = rope;
                newChain.GetComponent<SpringJoint2D>().connectedBody = currentBaseChain.GetComponent<Rigidbody2D>();
                newChain.GetComponent<HingeJoint2D>().connectedBody = currentBaseChain.GetComponent<Rigidbody2D>();
                baseSpringJoint.connectedBody = newChain.GetComponent<Rigidbody2D>();
                currentBaseChain = newChain;
                chainCount++;
            }
        }

        if((chainCount >= maxChainCount) && isTension && canPull){
            StartCoroutine(SmoothPull(pullingForce/1.75f, 0.5f));
            if(!playerAudioSource.isPlaying){
                playerAudioSource.PlayOneShot(ropeStretchSFX, 1);
            }
        }
        else{
            player.runSpeed = playerBaseSpeed;
        }
    }

    private void OnDisable() {
        player.onDash-=DoBackDash;
    }
}
