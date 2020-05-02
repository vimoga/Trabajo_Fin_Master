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

    /// <summary>
    /// HUD used in the game
    /// </summary>
    public HUDManager hudManager;

    private List<BasicDrone> playerDrones = new List<BasicDrone>();


    [HideInInspector]
    public float currentCPUGamePower = 0;

    private int currentMaxCPUPower = 1;

    // Start is called before the first frame update
    void Start()
    {
        hudManager.Initialize();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player) {
            AddPlayerDrone(player.GetComponent<BasicDrone>());
            currentCPUGamePower += player.GetComponent<BasicDrone>().captureCost;
        }
        
        GameObject[] playerDrones = GameObject.FindGameObjectsWithTag("Player_Drone");

        foreach (GameObject playerDrone in playerDrones)
        {
            AddPlayerDrone(playerDrone.GetComponent<BasicDrone>());
            currentCPUGamePower += playerDrone.GetComponent<BasicDrone>().captureCost;
        }

        if (currentCPUGamePower == 0) {
            currentMaxCPUPower = 1;
            currentCPUGamePower = 1;
        }
        else
        {
            currentMaxCPUPower = (int)currentCPUGamePower; 
        }

        hudManager.AddCPUPower(currentCPUGamePower);

        hudManager.AddCPUMaxPower(currentMaxCPUPower);
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
        currentCPUGamePower += power;
        hudManager.AddCPUPower(power);
    }

    public void RemoveCPUPower(float power) {
        hudManager.RemoveCPUPower(power);
        currentCPUGamePower -= power;
    }

    public void AddMaxCPU(int power)
    {
        currentMaxCPUPower += power;
        hudManager.AddCPUMaxPower(power);
    }

    public void RemoveMaxCPU(int power)
    {
        hudManager.RemoveCPUMaxPower(power);
        currentMaxCPUPower -= power;
    }

    public bool IsCapturePosible(float cost) {
        return currentCPUGamePower >= cost;
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
