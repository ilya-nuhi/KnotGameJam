using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameSession : MonoBehaviour
{
    [SerializeField] int playerLives = 3;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI lengthText;
    [SerializeField] TextMeshProUGUI maxLengthText;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip deathSFX;
    [SerializeField] AudioClip gameOverSFX;


    public int ropeLength = 0;
    void Awake() {
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if(numGameSessions>1){
            Destroy(gameObject);
        }
        else{
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start() {
        UpdateRopeLength();
        UpdateMaxRopeLength(RopeMechanics.maxChainCount);
    }

    public void UpdateMaxRopeLength(int newRopeLength)
    {
        StartCoroutine(SmoothUpdateRopeLength(newRopeLength));
    }

    private IEnumerator SmoothUpdateRopeLength(int newRopeLength)
    {
        float duration = 2f; // Duration in seconds
        float elapsed = 0f;
        
        int currentRopeLength = RopeMechanics.maxChainCount;
        RopeMechanics.maxChainCount = newRopeLength; // Update the max chain count immediately

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration); // Ensure t is between 0 and 1

            // Smoothly interpolate the value
            int interpolatedLength = Mathf.RoundToInt(Mathf.Lerp(currentRopeLength, newRopeLength, t));
            maxLengthText.text = interpolatedLength.ToString();

            yield return null; // Wait for the next frame
        }

        // Ensure the final value is set
        maxLengthText.text = newRopeLength.ToString();
    }


    private void Update() {
        UpdateRopeLength();
    }

    private void UpdateRopeLength()
    {
        lengthText.text = RopeMechanics.chainCount.ToString();
    }


    public void ProcessPlayerDeath(){
        if(playerLives>1){
            GetComponent<AudioSource>().PlayOneShot(deathSFX);
            StartCoroutine(TakeLife());
        }
        else{
            GetComponent<AudioSource>().PlayOneShot(gameOverSFX);
            StartCoroutine(ResetGameSession());
        }
    }

    IEnumerator TakeLife()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        playerLives--;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
        livesText.text = playerLives.ToString();
    }

    public void ReturnMenu(){
        StartCoroutine(ResetGameSession());
    }
    IEnumerator ResetGameSession()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
        RopeMechanics.maxChainCount = 100;
        FindObjectOfType<ScenePersist>().SceneDestroy();
        Destroy(gameObject);
        SceneManager.LoadScene(1);

    }
}
