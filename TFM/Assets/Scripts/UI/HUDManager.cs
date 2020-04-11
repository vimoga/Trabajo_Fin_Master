﻿using System.Collections;
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
    /// Main Camaera of the game
    /// </summary>
    public Camera mainCamera;

    private int currentDroneIndex = 0;
    private float currentCPUIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
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
    public void addPlayerDrone(BasicDrone drone) {
        currentDroneIndex += 1;
        drones[currentDroneIndex].GetComponent<HUDPlayerDrone>().drone = drone;
        drawPlayerDrones(drone, currentDroneIndex);
    }

    /// <summary>
    /// remove drone from the HUD
    /// </summary>
    /// <param name="drone">player drone to remove from the HUD</param>
    public void removePlayerDrone(BasicDrone drone) {
        for (int i = 0; i < drones.Length; i++) {
            if (drones[i].GetComponent<HUDPlayerDrone>().drone.Equals(drone)) {
                drones[i].SetActive(false);
            }
        }
        currentDroneIndex -= 1;
        restructurePlayerDrones();
    }

    /// <summary>
    /// Add CPU power in the HUD
    /// </summary>
    /// <param name="drone">Quantity of CPU power to add in the HUD</param>
    public void addCPUPower(float power) {
        currentCPUIndex += power;
        drawCPUPower();
    }

    /// <summary>
    /// remove CPU power from the HUD
    /// </summary>
    /// <param name="drone">Quantity of CPU power to remove from the HUD</param>
    public void removeCPUPower(float power) {
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
        RawImage rawImage = drones[index].GetComponent<RawImage>();
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
            drones[index].GetComponent<Text>().text = "Ammo: " + drone.ammo;
        }
        else
        {
            drones[index].GetComponent<Text>().text = "";
        }

        drones[index].GetComponent<SimpleHealthBar>().UpdateBar(drone.life, drone.maxHeath);

        drones[index].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
