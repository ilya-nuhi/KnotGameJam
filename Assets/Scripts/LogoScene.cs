using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoScene : MonoBehaviour
{
    [SerializeField] GameObject panel;
    
    AudioSource audioSource;
    Animator panelAnimator;
    bool skippingScene = false;
    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        panelAnimator = panel.GetComponent<Animator>();
    }

    private void Start() {
        panelAnimator.enabled=false;
    }
    
    void Update()
    {
        if(!audioSource.isPlaying && !skippingScene){
            StartCoroutine(SkipScene());
        }

    }

    IEnumerator SkipScene()
    {
        skippingScene = true;
        panelAnimator.enabled=true;
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
