using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Scout Drone
/// </summary>
public class ScoutDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Sound of the shoots
    /// </summary>
    public AudioClip shootSound;

    /// <summary>
    /// Damage of the shoots
    /// </summary>
    public float damage = 2f;

    /// <summary>
    /// Time between the shoots
    /// </summary>
    public float firerate = 10f;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    private BasicDrone drone;

    private float firingRange;

    private Transform[] wayPoints;

    private GameObject scout_enemy;

    private AudioSource audioSource;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    private NavMeshAgent agent;

    private int nextWayPoint = 0;

    private float currentAlertTime = 0;

    private float topSpeed = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        firingRange = this.GetComponentInChildren<SphereCollider>().radius;

        audioSource = GetComponent<AudioSource>();

        drone = GetComponent<BasicDrone>();

        isCaptured = drone.isCaptured;

        wayPoints = drone.wayPoints;

        if (isCaptured)
        {
            drone.currentState = DroneState.CAPTURED;
        }
        else {
            drone.currentState = DroneState.PATROL;
        }

        if (wayPoints.Length == 0)
        {
            wayPoints = new Transform[1] { gameObject.transform };
        }

        agent = gameObject.GetComponent<NavMeshAgent>();

        topSpeed = agent.speed;
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
            if (scout_enemy != null)
            {
                if (AuxiliarOperations.IsPlayerDrone(other))
                {
                    if (scout_enemy.Equals(other.gameObject) && !AuxiliarOperations.IsDestroyed(other.gameObject))
                    {
                        drone.GoToAlertState();
                    }
                    else
                    {
                        scout_enemy = null;
                        drone.GoToPatrolState();
                    }
                }
            }
            else {
                drone.GoToPatrolState();
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
            if (scout_enemy == null)
            {
                scout_enemy = other.gameObject;
                drone.GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(scout_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    scout_enemy = other.gameObject;                   
                }
                drone.GoToAttackState();
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the scout drone
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
            else
            {
                if (gameObject.GetComponent<BasicDrone>().ammo > 0)
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
        Vector3 targetPostition = new Vector3(enemy.transform.position.x, gameObject.transform.position.y, enemy.transform.position.z);
        gameObject.transform.LookAt(targetPostition);
        muzzelFlash.gameObject.transform.LookAt(enemy.transform);

        if ((currentFireRate > firerate))
        {
            if (enemy)
            {
                enemy.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }

            currentFireRate = 0;

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
                audioSource.PlayOneShot(shootSound, 1);
            }

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
                if (!isCaptured && scout_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(scout_enemy))
                    {
                        if (!scout_enemy.GetComponent<CommonInterface>().isDestroyed())
                        {
                            if (agent.isOnNavMesh) {
                                agent.destination = gameObject.transform.position;
                                Attack(scout_enemy);
                            }
                            
                        }
                    }
                    else
                    {
                        scout_enemy = null;
                        drone.GoToPatrolState();
                    }
                }
                break;
            case DroneState.PATROL:
                //patrol map by waypoints
                if (!gameObject.GetComponent<CommonInterface>().isDestroyed())
                {
                    agent.destination = wayPoints[nextWayPoint].position;

                    float distance = Vector3.Distance(gameObject.transform.position, agent.destination);

                    //reducir velocidad temporalmente
                    if (nextWayPoint > 0 && distance < (Vector3.Distance(wayPoints[nextWayPoint - 1].position, wayPoints[nextWayPoint].position) * 0.25))
                    {
                        agent.speed = topSpeed / 2;
                    }
                    else {
                        agent.speed = topSpeed;
                    }

                    if ( distance <= agent.stoppingDistance+GameConstants.WAYPOINT_STOP_AVOID)
                    {                        
                        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
                    }
                }               
                break;
            case DroneState.ALERT:
                //follow player drones when is not captured
                if (!isCaptured && scout_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(scout_enemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME) {
                            currentAlertTime = 0;
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                agent.destination = scout_enemy.transform.position;
                            }                          
                        } else {
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                scout_enemy = null;
                                drone.GoToPatrolState();
                            }
                        }                       
                    }
                    else
                    {
                        if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                        {
                            scout_enemy = null;
                            drone.GoToPatrolState();
                        }
                    }
                }                
                break;
            case DroneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured && drone.currentState !=DroneState.CAPTURED) {
            drone.GoToCapturedState();
        }

        /*if (isCaptured) {
            float distance = Vector3.Distance(gameObject.transform.position, agent.destination);
            //reducir velocidad temporalmente
            if (distance < GetFiringRange())
            {
                agent.speed = topSpeed / 2;
            }
            else
            {
                agent.speed = topSpeed;
            }
        }*/

        currentFireRate += Time.deltaTime;

        if (drone.currentState == DroneState.ALERT) {
            currentAlertTime += Time.deltaTime;
        }

    }
}