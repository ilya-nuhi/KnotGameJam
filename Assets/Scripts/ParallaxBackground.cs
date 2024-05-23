using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [SerializeField] private float parallaxEffectMultiplier = 0.5f; // Adjust this value to change the parallax effect strength
    private Transform cameraTransform; // Reference to the main camera's transform
    private Vector3 lastCameraPosition; // The camera's position in the previous frame

    void Start()
    {
        // Initialize references
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
    }

    void Update()
    {
        // Calculate the difference in the camera's position since the last frame
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        // Move the background by a proportion of the camera's movement
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier, deltaMovement.y * parallaxEffectMultiplier, 0);

        // Update the last camera position for the next frame
        lastCameraPosition = cameraTransform.position;
    }
}
