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

    private float currentCPUPower = 0;

    // Start is called before the first frame update
    void Start()
    {
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

        hudManager.addCPUPower(currentCPUPower);
    }

    public void AddPlayerDrone(BasicDrone playerDrone) {
        playerDrones.Add(playerDrone);
        hudManager.addPlayerDrone(playerDrone);
    }

    public void removePlayerDrone(BasicDrone drone) {
        hudManager.removePlayerDrone(drone);
        //revisar
        playerDrones.Remove(drone);
    }

    public void addCPUPower(int power) {
        currentCPUPower += power;
        hudManager.addCPUPower(power);
    }

    public void removeCPUPower(int power) {
        hudManager.removeCPUPower(power);
        currentCPUPower -= power;
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
