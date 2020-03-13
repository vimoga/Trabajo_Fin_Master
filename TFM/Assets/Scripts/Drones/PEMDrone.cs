using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEMDrone : MonoBehaviour, DroneInterface
{
    public float damage = 0.10f;

    // Distance the drone can aim and fire from
    public float firingRange = 25f;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMEffect;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMWave;

    private bool isCaptured = false;

    private enum colliderStatus {enter,stay,exit};

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        if (gameObject.tag == "Player" || gameObject.tag == "Player_Drone")
        {
            isCaptured = true;
        }
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }

       

    }


    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }

    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }

       
    }

    private void ColliderBehaviour(colliderStatus colStatus, Collider other)
    {
        switch (colStatus) {
            case colliderStatus.enter:

                other.transform.gameObject.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
                if (!PEMEffect.activeSelf && !PEMWave.activeSelf)
                {
                    PEMEffect.SetActive(true);
                    PEMWave.SetActive(true);
                }

                break;
            case colliderStatus.stay:

                other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                {
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }

                    break;
            case colliderStatus.exit:

                other.transform.gameObject.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);

                if (PEMEffect.activeSelf && PEMWave.activeSelf)
                {
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }

                break;
        }

    }

    public void SetCaptured(bool isCaptured)
    {
        this.isCaptured = isCaptured;
    }

    public bool GetCaptured()
    {
        return isCaptured;
    }

    public float GetFiringRange()
    {
        return firingRange;
    }

    public void Attack(GameObject enemy)
    {
 
    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
