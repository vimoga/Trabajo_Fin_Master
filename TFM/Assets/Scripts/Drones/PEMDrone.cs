using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    /// PEM effect
    /// </summary>
    public GameObject PEMEffect;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMWave;

    private Transform[] wayPoints;

    private BasicDrone drone;

    private float firingRange;

    private bool isCaptured = false;

    private Collider PEMEnemy;

    private enum colliderStatus {enter,stay,exit};

    private NavMeshAgent agent;

    private int nextWayPoint = 0;

    private float currentAlertTime = 0;

    private Collider enemyInZone;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        firingRange = this.GetComponentInChildren<SphereCollider>().radius;

        drone = GetComponent<BasicDrone>();

        isCaptured = drone.isCaptured;

        wayPoints = drone.wayPoints;

        if (isCaptured)
        {
            drone.currentState = DroneState.CAPTURED;
        }
        else
        {
            drone.currentState = DroneState.PATROL;
        }

        if (wayPoints.Length == 0)
        {
            wayPoints = new Transform[1] { gameObject.transform };
        }

        agent = gameObject.GetComponent<NavMeshAgent>();
    }

    void DroneInterface.OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemyDrone(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
    }

    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemyDrone(other))
            {         
                ColliderBehaviour(colliderStatus.stay, other);
            }
            else
            {
                //bug fix
                if (AuxiliarOperations.IsPlayerDrone(other))
                {
                    CeaseDamage(other.transform.gameObject);
                }
            }
        }
    }

    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemyDrone(other))
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
                if (gameObject.IsDestroyed())
                {
                    CeaseDamage(other.transform.gameObject);
                }
                else {
                    if (AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                    {
                        CeaseDamage(other.transform.gameObject);
                    }
                    else
                    {
                        DealDamage(other.transform.gameObject);                                             
                    }                                      
                }                                              
                break;
            case colliderStatus.exit:
                CeaseDamage(other.transform.gameObject);
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

    /// <summary>
    /// Cease damage to the affected drones
    /// </summary>
    /// <param name="other">enemy of the drone</param>
    private void CeaseDamage(GameObject other)
    {
        if (other != null) {
            other.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);
        }       
        if (PEMEffect.activeSelf && PEMWave.activeSelf)
        {
            PEMEffect.SetActive(false);
            PEMWave.SetActive(false);
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
        return firingRange * transform.localScale.x;
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
        // Switch on the statr enum.
        switch (drone.currentState)
        {
            case DroneState.ATTACK:
                break;
            case DroneState.PATROL:
                //patrol map by waypoints
                if (!gameObject.GetComponent<CommonInterface>().isDestroyed())
                {
                    agent.destination = wayPoints[nextWayPoint].position;
                    Vector3 fixedPosition = gameObject.transform.position;
                    fixedPosition.y -= GameConstants.TERRAIN_HEIGHT_CORRECTION;

                    if (Vector3.Distance(fixedPosition, agent.destination) <= agent.stoppingDistance + GameConstants.WAYPOINT_STOP_AVOID)
                    {
                        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
                    }
                }
                break;
            case DroneState.ALERT:
                break;
            case DroneState.CAPTURED:
                
                
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured && drone.currentState != DroneState.CAPTURED)
        {
            drone.GoToCapturedState();           
        }

        if (drone.currentState == DroneState.ALERT)
        {
            currentAlertTime += Time.deltaTime;
        }
    }
}
