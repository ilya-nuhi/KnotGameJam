using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;

    void Start() {
    }

    public void PlayAgain(){
        Destroy(FindObjectOfType<GameSession>().gameObject);
        SceneManager.LoadScene(0);
    }

}
