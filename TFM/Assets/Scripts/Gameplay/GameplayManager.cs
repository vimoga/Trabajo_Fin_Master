using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{

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
