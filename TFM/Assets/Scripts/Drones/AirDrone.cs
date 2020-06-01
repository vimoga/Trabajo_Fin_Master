using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Air Drone
/// </summary>
public class AirDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Damage of the missiles
    /// </summary>
    public float damage = 25f;

    /// <summary>
    /// Time between missiles are shoot
    /// </summary>
    public float firerate = 1.0f;

    /// <summary>
    /// Missile to shoot
    /// </summary>
    public GameObject missile;

    /// <summary>
    /// Speed of the missiles
    /// </summary>
    public float missileSpeed = 10f;

    /// <summary>
    /// Gameobject from the missile is shoot
    /// </summary>
    public GameObject missileLauncher;

    private BasicDrone drone;

    private float firingRange;

    private Transform[] wayPoints;

    private GameObject airDroneEnemy;

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
            wayPoints = new Transform[1]{ gameObject.transform };
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
                if (airDroneEnemy != null)
                {
                    if (airDroneEnemy.Equals(other.gameObject) && !AuxiliarOperations.IsDestroyed(other.gameObject))
                    {
                        drone.GoToAlertState();
                    }
                    else
                    {
                        airDroneEnemy = null;
                        drone.GoToPatrolState();
                    }
                }
                else {
                    drone.GoToPatrolState();
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
        if (AuxiliarOperations.IsPlayerDrone(other))
        {
            if (airDroneEnemy == null)
            {
                airDroneEnemy = other.gameObject;
                drone.GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    airDroneEnemy = other.gameObject;
                }
                drone.GoToAttackState();
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the air drone
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
            if (gameObject.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO)
            {
                MakeAttack(enemy);
            }
            else {
                if (gameObject.GetComponent<BasicDrone>().ammo >0) {
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
        if ((currentFireRate > firerate))
        {
            gameObject.transform.LookAt(enemy.transform);

            currentFireRate = 0;

            //generate new missile instance
            Missile shootMissile = missile.GetComponent<Missile>();
            shootMissile.enemy = enemy;
            shootMissile.speed = missileSpeed;
            shootMissile.damage = damage;

            GameObject.Instantiate(missile, missileLauncher.transform.position, missileLauncher.transform.rotation);

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
        return firingRange * transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {      
        // Switch on the statr enum.
        switch (drone.currentState)
        {
            case DroneState.ATTACK:
                //attack player drones when is not captured
                if (!isCaptured && airDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
                    {
                        agent.destination = gameObject.transform.position;
                        Attack(airDroneEnemy);
                    }
                    else
                    {
                        airDroneEnemy = null;
                        drone.GoToPatrolState();
                    }
                }
                else if (!isCaptured && AuxiliarOperations.IsDestroyed(airDroneEnemy)) {
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
                if (!isCaptured && airDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                agent.destination = airDroneEnemy.transform.position;
                            }
                        }                        
                    }
                    else
                    {
                        if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                        {
                            airDroneEnemy = null;
                            drone.GoToPatrolState();
                        }
                    }
                }
                break;
            case DroneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        currentFireRate += Time.deltaTime;

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
