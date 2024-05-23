using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Protrusion : MonoBehaviour
{
    PlayerMovement player;
    float distanceWithPlayer;
    [Header("Values")]
    [SerializeField] float throwableDistance = 10f;

    DistanceJoint2D distanceJoint2D;
    LineRenderer lineRenderer;

    public static bool isConnectedCheck;
    bool isConnected = false;
    private void OnEnable() {
        player.onThrowRope += ThrowRope;
    }

    private void ThrowRope()
    {
        if(!isConnected){
            if(distanceWithPlayer < throwableDistance){
                distanceJoint2D.connectedBody = player.GetComponent<Rigidbody2D>();
                lineRenderer.enabled = true;
                isConnected = true;
                isConnectedCheck = true;
                player.jumpCount++;
            }
        }else{
            distanceJoint2D.connectedBody = null;
            lineRenderer.enabled = false;
            isConnected = false;
            isConnectedCheck = false;
        }

    }

    private void Awake()
    {
        distanceJoint2D = GetComponent<DistanceJoint2D>();
        lineRenderer = GetComponent<LineRenderer>();
        player = FindObjectOfType<PlayerMovement>();
        if(lineRenderer==null){
            gameObject.AddComponent<LineRenderer>();
        } 
    }
    void Start()
    {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.75f,0.45f,0.26f);
        lineRenderer.endColor = new Color(0.75f,0.45f,0.26f);
        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false; // Disable the line renderer initially
    }

    // Update is called once per frame
    void Update()
    {
        distanceWithPlayer = (transform.position - player.transform.position).magnitude;
        if (isConnected)
        {
            // Update the line renderer positions
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.transform.position);
        }
    }

    private void OnDisable() {
        player.onThrowRope -= ThrowRope;
    }
}
