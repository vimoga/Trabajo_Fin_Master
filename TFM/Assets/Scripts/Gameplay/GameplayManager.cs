using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

/// <summary>
/// Implements common functions used to manage the gameplay
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

    /// <summary>
    /// Instance of the player to use on the checkpoints
    /// </summary>
    public GameObject playerPrebab;

    private List<BasicDrone> playerDrones = new List<BasicDrone>();

    [HideInInspector]
    public float currentCPUGamePower = 0;

    private int currentMaxCPUPower = 1;


    // Start is called before the first frame update
    void Start()
    {
        hudManager.Initialize();

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        NavMeshAgent agent = player.GetComponent<NavMeshAgent>();

        

        if (GameConstants.spawnPoint.x != 0 && GameConstants.spawnPoint.z != 0)
        {
            Destroy(player);
            player = Instantiate(playerPrebab, new Vector3(GameConstants.spawnPoint.x, player.transform.position.y, GameConstants.spawnPoint.z), Quaternion.identity);
            player.transform.parent = GameObject.FindGameObjectWithTag("DroneContainer").transform;
            GameConstants.playerTemp = player;
        
            Debug.Log("player temp creado");
            //fix for build mode
            GameObject.FindObjectOfType<RTS_Camera>().transform.position = new Vector3(GameConstants.spawnPoint.x, GameObject.FindObjectOfType<RTS_Camera>().transform.position.y, GameConstants.spawnPoint.z);
        }
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

        if (currentCPUGamePower == 0)
        {
            currentMaxCPUPower = 1;
            currentCPUGamePower = 1;
        }
        else
        {
            currentMaxCPUPower = (int)currentCPUGamePower;
        }
       
        hudManager.AddCPUPower(currentCPUGamePower);

        hudManager.AddCPUMaxPower(currentMaxCPUPower);

        GameObject.FindGameObjectWithTag("FogOfWar").GetComponent<FogOfWar>().clearRadars();

        GameConstants.currentCPUPower = currentCPUGamePower;

             
    }

    /// <summary>
    /// Adds a player drone in the hud and the game
    /// </summary>
    /// <param name="drone">drone to add</param>
    public void AddPlayerDrone(BasicDrone playerDrone) {
        playerDrones.Add(playerDrone);
        hudManager.AddPlayerDrone(playerDrone);
    }

    /// <summary>
    /// Removes a player drone from the hud and the game
    /// </summary>
    /// <param name="drone">drone to remove</param>
    public void RemovePlayerDrone(BasicDrone drone) {

        //game over when main drone is destroyed
        if (drone.name.Equals("Main Drone")) {
            GameConstants.postCapturedTemp.Clear();
            GameConstants.generatorDestroyedTemp.Clear();
            if (GameConstants.spawnPoint.x != 0 && GameConstants.spawnPoint.z != 0) {
                Invoke("Respawn", 2.5f);
            }
            else
            {
                Invoke("GameOver", 2.5f);
            }           
        }
        hudManager.RemovePlayerDrone(drone);
        playerDrones.Remove(drone);
    }

    /// <summary>
    /// increments the current cpu power
    /// </summary>
    /// <param name="power">cpu to add</param>
    public void AddCPUPower(float power) {
        currentCPUGamePower += power;
        hudManager.AddCPUPower(power);
        GameConstants.currentCPUPower = currentCPUGamePower;
    }

    /// <summary>
    /// decreses the current cpu power
    /// </summary>
    /// <param name="power">cpu to decrese</param>
    public void RemoveCPUPower(float power) {
        hudManager.RemoveCPUPower(power);
        currentCPUGamePower -= power;
        GameConstants.currentCPUPower = currentCPUGamePower;
    }

    /// <summary>
    /// decreses the current maximum cpu to reach
    /// </summary>
    /// <param name="power">max cpu to decrese</param>
    public void AddMaxCPU(int power)
    {
        currentMaxCPUPower += power;
        hudManager.AddCPUMaxPower(power);
    }

    /// <summary>
    /// decreses the current maximum cpu to reach
    /// </summary>
    /// <param name="power">max cpu to decrese</param>
    public void RemoveMaxCPU(int power)
    {
        hudManager.RemoveCPUMaxPower(power);
        currentMaxCPUPower -= power;
    }

    private void Respawn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
