using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Implements comom functions used to manage the gameplay
/// </summary>
public class GameplayManager : MonoBehaviour
{

    /// <summary>
    /// pause menu used in the game
    /// </summary>
    public GameObject pauseMenu;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (SceneManager.GetActiveScene().name.Equals("Demo"))
        {
            //Enables and disables the pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (pauseMenu)
                {
                    if (pauseMenu.activeSelf)
                    {
                        pauseMenu.SetActive(false);
                    }
                    else
                    {
                        pauseMenu.SetActive(true);
                    }
                }
            }
        }
    }
}
