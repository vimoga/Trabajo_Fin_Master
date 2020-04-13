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
    /// Distance the drone can fire from
    /// </summary> 
    public float firingRange = 7;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    /// <summary>
    /// waypoints for the patrol route
    /// </summary>
    public Transform[] wayPoints;

    private GameObject scout_enemy;

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
        else {
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
            if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
            {                
                if (scout_enemy.Equals(other.gameObject))
                {
                    GoToAlertState();
                }
                else {
                    scout_enemy = null;
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
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
        {
            if (scout_enemy == null)
            {
                scout_enemy = other.gameObject;
                GoToAttackState();
            }
            else
            {
                if (Vector3.Distance(scout_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    scout_enemy = other.gameObject;                   
                }
                GoToAttackState();
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
        return firingRange;
    }

    // Update is called once per frame
    void Update()
    {       
        // Switch on the statr enum.
        switch (currentState)
        {
            case droneState.ATTACK:                
                //attack player drones when is not captured
                if (!isCaptured && scout_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(scout_enemy))
                    {
                        if (!scout_enemy.GetComponent<CommonInterface>().isDestroyed())
                        {
                            agent.destination = gameObject.transform.position;
                            Attack(scout_enemy);
                        }
                    }
                    else
                    {
                        scout_enemy = null;
                        GoToPatrolState();
                    }
                }
                break;
            case droneState.PATROL:
                //patrol map by waypoints
                if (!gameObject.GetComponent<CommonInterface>().isDestroyed())
                {
                    agent.destination = wayPoints[nextWayPoint].position;

                    if (agent.remainingDistance <= agent.stoppingDistance+GameConstants.WAYPOINT_STOP_AVOID)
                    {
                        nextWayPoint = (nextWayPoint + 1) % wayPoints.Length;
                    }
                }               
                break;
            case droneState.ALERT:
                //follow player drones when is not captured
                if (!isCaptured && scout_enemy != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(scout_enemy))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME) {
                            currentAlertTime = 0;
                            if (!scout_enemy.GetComponent<CommonInterface>().isDestroyed())
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    agent.destination = scout_enemy.transform.position;
                                }
                            }
                            else {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    scout_enemy = null;
                                    GoToPatrolState();
                                }                                  
                            }
                        } else {
                            if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                            {
                                scout_enemy = null;
                                GoToPatrolState();
                            }
                        }                       
                    }
                    else
                    {
                        if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                        {
                            scout_enemy = null;
                            GoToPatrolState();
                        }
                    }
                }                
                break;
            case droneState.CAPTURED:
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (isCaptured && currentState!=droneState.CAPTURED) {
            GoToCapturedState();
        }

        currentFireRate += Time.deltaTime;

        if (currentState == droneState.ALERT) {
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