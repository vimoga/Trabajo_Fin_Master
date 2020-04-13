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
    /// Distance the drone can fire from
    /// </summary>
    public float firingRange = 30;

    /// <summary>
    /// Turret of the tank
    /// </summary>
    public GameObject tnk_turret;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    /// <summary>
    /// waypoints for the patrol route
    /// </summary>
    public Transform[] wayPoints;

    private GameObject tnk_enemy;

    private AudioSource audioSource;

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

        audioSource = GetComponent<AudioSource>();

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
                if (tnk_enemy.Equals(other.gameObject))
                {
                    GoToAlertState();
                }
                else
                {
                    tnk_enemy = null;
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
                GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(tnk_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    tnk_enemy = other.gameObject;
                    GoToAttackState();
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
        return firingRange;
    }

    // Update is called once per frame
    void Update()
    {
       

        // Switch on the statr enum.
        switch (currentState)
        {
            case droneState.ATTACK:
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
                if (!isCaptured && tnk_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(tnk_enemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (!tnk_enemy.GetComponent<CommonInterface>().isDestroyed())
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    agent.destination = tnk_enemy.transform.position;
                                }
                            }
                            else
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    tnk_enemy = null;
                                    GoToPatrolState();
                                }
                            }
                        }
                        else
                        {
                            if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                            {
                                tnk_enemy = null;
                                GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                        {
                            tnk_enemy = null;
                            GoToPatrolState();
                        }
                    }
                }
                break;
            case droneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured && currentState != droneState.CAPTURED)
        {
            GoToCapturedState();
        }

        currentFireRate += Time.deltaTime;

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
