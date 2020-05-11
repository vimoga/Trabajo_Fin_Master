using System.Collections;
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
            if (SceneManager.GetActiveScene().name.Equals("MainLevel"))
            {
                SceneManager.LoadScene("MainLevel2");
                GameConstants.currentLevel = "MainLevel2";
            }
            else if (SceneManager.GetActiveScene().name.Equals("MainLevel2"))
            {
                SceneManager.LoadScene("EndGameMenu");
                GameConstants.currentLevel = "MainLevel";
            }

            //clear game variables
            GameConstants.currentCPUPower = 0;
            GameConstants.radarCaptured.Clear();
            GameConstants.postCaptured.Clear();
            GameConstants.spawnPoint = new Vector3(0, 0, 0);
        }
    }
}
