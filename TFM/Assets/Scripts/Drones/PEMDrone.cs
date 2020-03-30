using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the PEM Drone
/// </summary>
public class PEMDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Damage of the pem damage
    /// </summary>
    public float damage = 0.10f;

    /// <summary>
    /// Distance the drone can deal damage
    /// </summary>
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

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="colStatus">type of collider</param>
    /// <param name="other">object collided</param>
    private void ColliderBehaviour(colliderStatus colStatus, Collider other)
    {
        switch (colStatus) {
            case colliderStatus.enter:
                DealDamage(other.transform.gameObject);
                break;
            case colliderStatus.stay:
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

    /// <summary>
    /// Deal damage to the affected drones
    /// </summary>
    /// <param name="other">enemy of the drone</param>
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

    /// <summary>
    /// Custom Attack function of the pem drone
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    public void Attack(GameObject enemy)
    {
        //because the drone deals attacks from a close area, the drone closes to their objective
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemy.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    
}
