using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDPlayerDrone : MonoBehaviour
{
    /// <summary>
    /// Drone data
    /// </summary>
    [HideInInspector]
    public BasicDrone drone;

    /// <summary>
    /// Text that show the current ammo quantity
    /// </summary>
    public Text textAmmo;

    /// <summary>
    /// Bar that show the current heath quantity
    /// </summary>
    public SimpleHealthBar health;

    public Button deleteDrone;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void destroyDrone() {
        drone.DestroyDrone();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (drone) {
            if (drone.maxAmmo != -1)
            {
                textAmmo.text = "Ammo: " + drone.ammo;
            }

            health.UpdateBar(drone.life, drone.maxHeath);
        }      
    }
}
