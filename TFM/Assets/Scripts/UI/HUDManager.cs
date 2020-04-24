using RTS_Cam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage to represent visual inforamtion of the 
/// </summary>
public class HUDManager : MonoBehaviour
{
    /// <summary>
    /// Visual representation of player drones
    /// </summary>
    public GameObject[] drones;

    /// <summary>
    /// Visual representation of the CPU Power
    /// </summary>
    public GameObject[] cpuPower;

    /// <summary>
    /// player movement manager
    /// </summary>
    private PlayerMovement playerMovement;

    private int currentDroneIndex = 0;
    private float currentCPUIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerMovement>();
    }

    /// <summary>
    /// Clear every element form the HUD to start drawing
    /// </summary>
    public void Initialize() {
        foreach (GameObject playerDrone in drones)
        {
            playerDrone.SetActive(false);
        }

        foreach (GameObject power in cpuPower)
        {
            power.SetActive(false);
        }

    }

    /// <summary>
    /// add new drone in the HUD
    /// </summary>
    /// <param name="drone">player drone to add in the HUD</param>
    public void AddPlayerDrone(BasicDrone drone) {      
        drones[currentDroneIndex].GetComponent<HUDPlayerDrone>().drone = drone;
        drawPlayerDrones(drone, currentDroneIndex);
        currentDroneIndex += 1;
    }

    /// <summary>
    /// remove drone from the HUD
    /// </summary>
    /// <param name="drone">player drone to remove from the HUD</param>
    public void RemovePlayerDrone(BasicDrone drone) {
        for (int i = 0; i < drones.Length; i++) {
            if (drones[i].GetComponent<HUDPlayerDrone>().drone) {
                if (drones[i].GetComponent<HUDPlayerDrone>().drone.Equals(drone))
                {
                    drones[i].SetActive(false);
                }
            }
        }
        currentDroneIndex -= 1;
        restructurePlayerDrones();
    }

    /// <summary>
    /// Add CPU power in the HUD
    /// </summary>
    /// <param name="drone">Quantity of CPU power to add in the HUD</param>
    public void AddCPUPower(float power) {
        currentCPUIndex += power;
        drawCPUPower();
    }

    /// <summary>
    /// remove CPU power from the HUD
    /// </summary>
    /// <param name="drone">Quantity of CPU power to remove from the HUD</param>
    public void RemoveCPUPower(float power) {
        currentCPUIndex -= power;
        drawCPUPower();
    }

    private void drawCPUPower()
    {
        int counter = 0;
        foreach (GameObject cpu in cpuPower)
        {
            if (counter <= currentCPUIndex)
            {
                cpu.SetActive(true);
            }
            else
            {
                cpu.SetActive(false);
            }
            counter++;
        }
    }

    private void restructurePlayerDrones()
    {
        for (int i = 0; i < drones.Length; i++)
        {
            if (!drones[i].activeSelf)
            {
                for (int x = i; x < drones.Length; x++)
                {
                    if (drones[x].activeSelf)
                    {
                        drones[i].GetComponent<HUDPlayerDrone>().drone = drones[x].GetComponent<HUDPlayerDrone>().drone;
                        drones[x].SetActive(false);
                        drawPlayerDrones(drones[i].GetComponent<HUDPlayerDrone>().drone, i);
                    }
                }
            }
        }      
    }

    private void drawPlayerDrones(BasicDrone drone, int index)
    {
        RawImage rawImage = drones[index].GetComponentInChildren<RawImage>();
        switch (drone.name)
        {
            case "Bomber Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/bomber");
                break;
            case "Main Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/main");
                break;
            case "Healer Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/healer");
                break;
            case "PEM Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/PEM");
                break;
            case "Aerial Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/plane");
                break;
            case "Scout Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/scout");
                break;
            case "Tank Drone":
                rawImage.texture = (Texture)Resources.Load("Textures/tank");
                break;
        }

        if (drone.maxAmmo != -1)
        {
            drones[index].GetComponentInChildren<Text>().text = "Ammo: " + drone.ammo;
        }
        else
        {
            drones[index].GetComponentInChildren<Text>().text = "";
        }

        drones[index].GetComponentInChildren<SimpleHealthBar>().UpdateBar(drone.life, drone.maxHeath);

        drones[index].SetActive(true);
    }

    /// <summary>
    /// Select drone from HUD
    /// </summary>
    /// <param name="index">index of the list</param>
    public void SelectDrone(int index) {

        bool isSelected = false;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player)
        {
            if (player.GetComponent<BasicDrone>().Equals(drones[index].GetComponent<HUDPlayerDrone>().drone))
            {
                playerMovement.ExternalSelect(player);
                isSelected = true;
            }
        }

        if (!isSelected) { 
            GameObject[] playerDrones = GameObject.FindGameObjectsWithTag("Player_Drone");

            foreach (GameObject playerDrone in playerDrones)
            {
                if (playerDrone.GetComponent<BasicDrone>().Equals(drones[index].GetComponent<HUDPlayerDrone>().drone))
                {
                    playerMovement.ExternalSelect(playerDrone);
                    isSelected = true;
                    break;
                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
