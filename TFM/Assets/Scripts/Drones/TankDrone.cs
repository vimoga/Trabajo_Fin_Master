using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Tank Drone
/// </summary>
public class TankDrone : MonoBehaviour,DroneInterface
{
    /// <summary>
    /// Sound of the shoots
    /// </summary>
    public AudioClip shootSound;

    /// <summary>
    /// Damage of the shoots
    /// </summary>
    public float damage = 75f;

    /// <summary>
    /// Time between the shoots
    /// </summary>
    public float firerate = 5f;

    /// <summary>
    /// Turret of the tank
    /// </summary>
    public GameObject tnk_turret;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    private BasicDrone drone;

    private float firingRange;

    private Transform[] wayPoints;

    private GameObject tnk_enemy;

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();

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
        if (!isCaptured) {
            OnTriggerBehaviour(other);
        }      
    }

    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
            {
                if (tnk_enemy == null) {
                    if (tnk_enemy.Equals(other.gameObject))
                    {
                        drone.GoToAlertState();
                    }
                    else
                    {
                        tnk_enemy = null;
                    }
                }               
            }
        }

        //fix
        //tnk_turret.transform.rotation = gameObject.transform.rotation;
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger && !AuxiliarOperations.EnemyIsAerial(gameObject, other.gameObject))
        {
            if (tnk_enemy == null)
            {
                tnk_enemy = other.gameObject;
                drone.GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(tnk_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    tnk_enemy = other.gameObject;
                    drone.GoToAttackState();
                }
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the tank drone
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
                if (gameObject.GetComponent<BasicDrone>().ammo > 0 && !AuxiliarOperations.EnemyIsAerial(gameObject, enemy))
                {
                    MakeAttack(enemy);
                }
            }
        }             
    }

    private void MakeAttack(GameObject enemy)
    {
        //fix
        //tnk_turret.transform.LookAt(enemy.transform);
        gameObject.transform.LookAt(enemy.transform);

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
                //attack player drones when is not capture
                if (!isCaptured && tnk_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(tnk_enemy) && !AuxiliarOperations.EnemyIsAerial(gameObject, tnk_enemy))
                    {
                        if (!tnk_enemy.GetComponent<CommonInterface>().isDestroyed())
                        {
                            agent.destination = gameObject.transform.position;
                            Attack(tnk_enemy);
                        }
                    }
                    else
                    {
                        tnk_enemy = null;
                        drone.GoToPatrolState();
                    }
                }
                break;
            case DroneState.PATROL:
                //patrol map by waypoints
                if (!gameObject.GetComponent<CommonInterface>().isDestroyed())
                {
                    agent.destination = wayPoints[nextWayPoint].position;

                    if (Vector3.Distance(gameObject.transform.position, agent.destination) <= agent.stoppingDistance + GameConstants.WAYPOINT_STOP_AVOID)
                    {
                        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
                    }
                }
                break;
            case DroneState.ALERT:
                //follow player drones when is not captured
                if (!isCaptured && tnk_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(tnk_enemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                agent.destination = tnk_enemy.transform.position;
                            }                           
                        }
                        else
                        {
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                tnk_enemy = null;
                                drone.GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                        {
                            tnk_enemy = null;
                            drone.GoToPatrolState();
                        }
                    }
                }
                break;
            case DroneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured && drone.currentState != DroneState.CAPTURED)
        {
            drone.GoToCapturedState();
        }

        currentFireRate += Time.deltaTime;

        if (drone.currentState == DroneState.ALERT)
        {
            currentAlertTime += Time.deltaTime;
        }
    }

}
