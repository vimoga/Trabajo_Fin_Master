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
    /// Distance the drone can fire from
    /// </summary> 
    public float firingRange = 1.5f;

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

    /// <summary>
    /// waypoints for the patrol route
    /// </summary>
    public Transform[] wayPoints;

    private GameObject airDroneEnemy;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    private enum droneState { ATTACK, PATROL, ALERT, CAPTURED };

    private droneState currentState = droneState.PATROL;

    private NavMeshAgent agent;

    private int nextWayPoint = 0;

    private float currentAlertTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured)
        {
            currentState = droneState.CAPTURED;
        }
        else
        {
            currentState = droneState.PATROL;
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
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (airDroneEnemy.Equals(other.gameObject))
                {
                    GoToAlertState();
                }
                else
                {
                    airDroneEnemy = null;
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
        if (AuxiliarOperations.IsPlayer(other))
        {
            if (airDroneEnemy == null)
            {
                airDroneEnemy = other.gameObject;
                GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    airDroneEnemy = other.gameObject;
                }
                GoToAttackState();
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
        return firingRange*10f;
    }

    // Update is called once per frame
    void Update()
    {
        

        // Switch on the statr enum.
        switch (currentState)
        {
            case droneState.ATTACK:
                //attack player drones when is not captured
                if (!isCaptured && airDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
                    {
                        if (!airDroneEnemy.GetComponent<CommonInterface>().isDestroyed())
                        {
                            agent.destination = gameObject.transform.position;
                            Attack(airDroneEnemy);
                        }
                    }
                    else
                    {
                        airDroneEnemy = null;
                        GoToPatrolState();
                    }
                }
                break;
            case droneState.PATROL:
                //patrol map by waypoints
                if (!gameObject.GetComponent<CommonInterface>().isDestroyed())
                {
                    agent.destination = wayPoints[nextWayPoint].position;

                    if (agent.remainingDistance <= agent.stoppingDistance + GameConstants.WAYPOINT_STOP_AVOID)
                    {
                        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
                    }
                }
                break;
            case droneState.ALERT:
                //follow player drones when is not captured
                if (!isCaptured && airDroneEnemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (!airDroneEnemy.GetComponent<CommonInterface>().isDestroyed())
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    agent.destination = airDroneEnemy.transform.position;
                                }
                            }
                            else
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    airDroneEnemy = null;
                                    GoToPatrolState();
                                }
                            }
                        }
                        else
                        {
                            if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                            {
                                airDroneEnemy = null;
                                GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                        {
                            airDroneEnemy = null;
                            GoToPatrolState();
                        }
                    }
                }
                break;
            case droneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        currentFireRate += Time.deltaTime;

        if (isCaptured && currentState != droneState.CAPTURED)
        {
            GoToCapturedState();
        }

        if (currentState == droneState.ALERT)
        {
            currentAlertTime += Time.deltaTime;
        }
    }

    public void GoToAttackState()
    {
        currentState = droneState.ATTACK;
        Debug.Log("Drone state: " + droneState.ATTACK);
    }

    public void GoToAlertState()
    {
        currentState = droneState.ALERT;
        Debug.Log("Drone state: " + droneState.ALERT);
    }

    public void GoToPatrolState()
    {
        currentState = droneState.PATROL;
        Debug.Log("Drone state: " + droneState.PATROL);
    }

    public void GoToCapturedState()
    {
        currentState = droneState.CAPTURED;
        Debug.Log("Drone state: " + droneState.CAPTURED);
    }
}
