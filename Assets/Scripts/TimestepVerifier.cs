using UnityEngine;

public class TimestepVerifier : MonoBehaviour
{
    public float fixedTimestep = 0.02f; // Ensure this matches your editor setting

    void Awake()
    {
        // Set the fixed timestep
        Time.fixedDeltaTime = fixedTimestep;
    }
    void Start()
    {
        Debug.Log("Fixed Timestep: " + Time.fixedDeltaTime);
    }
}
