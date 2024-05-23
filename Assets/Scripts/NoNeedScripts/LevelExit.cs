using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    GameSession gameSession;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Player"){
            gameSession.ReturnMenu();
        }
    }

    private void Awake() {
        gameSession = FindObjectOfType<GameSession>();
    }
}
