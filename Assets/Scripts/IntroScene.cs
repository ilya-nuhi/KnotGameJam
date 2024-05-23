using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroScene : MonoBehaviour
{
    [SerializeField] GameObject introPanel;
    [SerializeField] AudioSource introAudioSource;
    void Start()
    {
        introPanel.SetActive(true);
    }

    public void PlayButton(){
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        introPanel.GetComponent<Animator>().SetTrigger("Fadein");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // If we are running in the editor
        #if UNITY_EDITOR
        // Stop playing the scene
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void TurnOnVolume(){
        introAudioSource.volume = 1;
    }

    public void TurnOffVolume(){
        introAudioSource.volume = 0;
    }
}
