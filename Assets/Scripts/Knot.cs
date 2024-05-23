using System.Collections;
using UnityEngine;

public class Knot : MonoBehaviour
{
    [SerializeField] AudioClip untieSFX;
    [SerializeField] AudioClip puleyPullingRopeSFX;
    [SerializeField] int extraRopeLength = 100; // Additional rope length to add when the knot is untied
    RopeMechanics ropeMechanics;
    private GameSession gameSession;
    private AudioSource audioSource;
    private bool triggered = false;
    LineRenderer lineRenderer;
    private void Awake()
    {
        ropeMechanics = FindObjectOfType<RopeMechanics>();
        lineRenderer = GetComponent<LineRenderer>();
        if(lineRenderer==null){
            gameObject.AddComponent<LineRenderer>();
        }
        // Initialize audio source and game session
        audioSource = GetComponent<AudioSource>();
        gameSession = FindObjectOfType<GameSession>();

        // Ensure components are found
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing from Knot GameObject.");
        }
        if (gameSession == null)
        {
            Debug.LogError("GameSession component not found in the scene.");
        }
    }

    private void Start() {
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = new Color(0.75f,0.45f,0.26f);
        lineRenderer.endColor = new Color(0.75f,0.45f,0.26f);
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, ropeMechanics.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player enters the trigger and if the knot has not been triggered before
        if (other.gameObject.CompareTag("Player") && !triggered)
        {
            triggered = true;
            StartCoroutine(Untie());
        }
    }

    private IEnumerator Untie()
    {
        // Play untie sound effect
        if (audioSource != null && untieSFX != null)
        {
            audioSource.PlayOneShot(untieSFX);
        }

        yield return new WaitForSeconds(1);
        if(puleyPullingRopeSFX!=null){
            audioSource.PlayOneShot(puleyPullingRopeSFX);
        }

        // Update the rope length in the game session
        if (gameSession != null)
        {
            gameSession.UpdateMaxRopeLength(RopeMechanics.maxChainCount + extraRopeLength);
        }

        // Wait for 2 seconds before destroying the knot GameObject
        yield return new WaitForSeconds(1);
        Destroy(gameObject);
    }
}
