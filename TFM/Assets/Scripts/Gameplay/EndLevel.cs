﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Detects if the player has reaches the end level
/// </summary>
public class EndLevel : MonoBehaviour
{
    /// <summary>
    /// Gestor del jugador
    /// </summary>
    //public PlayerManager playerManager;

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            SceneManager.LoadScene("EndGameMenu");
        }
    }
}
