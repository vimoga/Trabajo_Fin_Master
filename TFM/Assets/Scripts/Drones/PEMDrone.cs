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
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }


    // Detect an Enemy, aim and fire
    void DroneInterface.OnTriggerEnter(Collider other)
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
    void DroneInterface.OnTriggerStay(Collider other)
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
    void DroneInterface.OnTriggerExit(Collider other)
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

                /*other.transform.gameObject.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
                if (!PEMEffect.activeSelf && !PEMWave.activeSelf)
                {
                    PEMEffect.SetActive(true);
                    PEMWave.SetActive(true);
                }*/
                DealDamage(other.transform.gameObject);
                break;
            case colliderStatus.stay:

                //other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                {
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }
                else {
                    DealDamage(other.transform.gameObject);
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

    private void DealDamage(GameObject other)
    {       
        other.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
        other.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
        if (!PEMEffect.activeSelf && !PEMWave.activeSelf)
        {
            PEMEffect.SetActive(true);
            PEMWave.SetActive(true);
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
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    
}
