﻿using System.Collections;
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

    /// <summary>
    /// HUD used in the game
    /// </summary>
    public HUDManager hudManager;

    private List<BasicDrone> playerDrones = new List<BasicDrone>();


    [HideInInspector]
    public float currentCPUPower = 0;

    // Start is called before the first frame update
    void Start()
    {
        hudManager.Initialize();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) {
            AddPlayerDrone(player.GetComponent<BasicDrone>());
            currentCPUPower += player.GetComponent<BasicDrone>().captureCost;
        }
        
        GameObject[] playerDrones = GameObject.FindGameObjectsWithTag("Player_Drone");

        foreach (GameObject playerDrone in playerDrones)
        {
            AddPlayerDrone(playerDrone.GetComponent<BasicDrone>());
            currentCPUPower += playerDrone.GetComponent<BasicDrone>().captureCost;
        }

        hudManager.RemoveCPUPower(GameConstants.MAX_CPU_POWER);

        hudManager.AddCPUPower(currentCPUPower);
    }

    public void AddPlayerDrone(BasicDrone playerDrone) {
        playerDrones.Add(playerDrone);
        hudManager.AddPlayerDrone(playerDrone);
    }

    public void RemovePlayerDrone(BasicDrone drone) {

        //game over when main drone is destroyed
        if (drone.name.Equals("Main Drone")) {
            Invoke("GameOver", 2.5f);
        }
        hudManager.RemovePlayerDrone(drone);
        playerDrones.Remove(drone);
    }

    public void AddCPUPower(float power) {
        currentCPUPower += power;
        hudManager.AddCPUPower(power);
    }

    public void RemoveCPUPower(float power) {
        hudManager.RemoveCPUPower(power);
        currentCPUPower -= power;
    }

    private void GameOver() {
        SceneManager.LoadScene("EndGameMenu");
    }

    // Update is called once per frame
    void Update()
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
