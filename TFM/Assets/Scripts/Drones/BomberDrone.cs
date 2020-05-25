﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Bomber Drone
/// </summary>
public class BomberDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Damage of the bombs
    /// </summary>
    public float damage = 200f;

    /// <summary>
    /// Distance the drone can fire from
    /// </summary> 
    public float firerate = 5f;

    /// <summary>
    /// Bomb to drop
    /// </summary>
    public GameObject bomb;

    /// <summary>
    /// Gameobject from the bombs are drop
    /// </summary>
    public GameObject bombLauncher;

    private BasicDrone drone;

    private float firingRange;

    private Transform[] wayPoints;

    private GameObject bomberDroneEnemy;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    private NavMeshAgent agent;

    private int nextWayPoint = 0;

    private float currentAlertTime = 0;

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
            OnTriggerBehaviour(other);
        }
    }

    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }

    }

    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                if (bomberDroneEnemy !=null )
                {
                    if (bomberDroneEnemy.Equals(other.gameObject) && !AuxiliarOperations.IsDestroyed(other.gameObject))
                    {
                        drone.GoToAlertState();
                    }
                    else
                    {
                        bomberDroneEnemy = null;
                        drone.GoToPatrolState();
                    }
                }               
            }
        }
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if (AuxiliarOperations.IsPlayerDrone(other) && !AuxiliarOperations.EnemyIsAerial(other.gameObject))
        {
            if (bomberDroneEnemy == null)
            {
                bomberDroneEnemy = other.gameObject;
                drone.GoToAttackState();
            }
            else
            {
                if ((Vector3.Distance(bomberDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position)) 
                    && !AuxiliarOperations.EnemyIsAerial(other.transform.gameObject))
                {
                    bomberDroneEnemy = other.gameObject;
                }
                drone.GoToAttackState();
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the bomber drone
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    public void Attack(GameObject enemy)
    {
        if (!isCaptured)
        {
            MakeAttack(enemy);
        }
        else
        {
            //only attack if there is ammo remaining
            if (gameObject.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO && !AuxiliarOperations.EnemyIsAerial(enemy))
            {
                MakeAttack(enemy);
            }
            else
            {
                if (gameObject.GetComponent<BasicDrone>().ammo > 0 && !AuxiliarOperations.EnemyIsAerial(enemy))
                {
                    MakeAttack(enemy);
                }
            }
        }
    }

    /// <summary>
    /// Generate the effects, animation and behaviour of the drone attack
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    private void MakeAttack(GameObject enemy)
    {
        //gameObject.transform.LookAt(enemy.transform);

        if ((currentFireRate > firerate))
        {
            currentFireRate = 0;

            //generate new bomb instance
            Bomb bombScript = bomb.GetComponentInChildren<Bomb>();
            bombScript.enemy = enemy;
            bombScript.damage = damage;

            GameObject.Instantiate(bomb, bombLauncher.transform.position, bombLauncher.transform.rotation);

            if (isCaptured)
            {
                gameObject.GetComponent<BasicDrone>().AmmoOut();
            }
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
        return gameObject.GetComponent<NavMeshAgent>().radius+4;
    }

    // Update is called once per frame
    void Update()
    {       
        // Switch on the statr enum.
        switch (drone.currentState)
        {
            case DroneState.ATTACK:
                //attack player drones when is not captured
                if (!isCaptured && bomberDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(bomberDroneEnemy))
                    {
                        if (!AuxiliarOperations.EnemyIsAerial(bomberDroneEnemy))
                        {
                            if (Vector3.Distance(bomberDroneEnemy.transform.position, gameObject.transform.position) > (gameObject.GetComponent<NavMeshAgent>().radius + 3))
                            {
                                gameObject.GetComponent<NavMeshAgent>().destination = bomberDroneEnemy.transform.position;
                            }
                            else
                            {
                                Attack(bomberDroneEnemy);
                                gameObject.GetComponent<NavMeshAgent>().destination = gameObject.transform.position;
                            }
                        }
                        else
                        {
                            drone.GoToPatrolState();
                        }
                    }
                    else
                    {
                        bomberDroneEnemy = null;
                        drone.GoToPatrolState();
                    }
                }
                else if (!isCaptured && AuxiliarOperations.IsDestroyed(bomberDroneEnemy))
                {
                    drone.GoToPatrolState();
                }
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
                //follow player drones when is not captured
                if (!isCaptured && bomberDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(bomberDroneEnemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                agent.destination = bomberDroneEnemy.transform.position;
                            }                           
                        }
                        else
                        {
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                bomberDroneEnemy = null;
                                drone.GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                        {
                            bomberDroneEnemy = null;
                            drone.GoToPatrolState();
                        }
                    }
                }
                break;
            case DroneState.CAPTURED:
                break;
        }

        currentFireRate += Time.deltaTime;

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

