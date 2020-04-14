using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Healer Drone
/// </summary>
public class HealerDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Heal quantity
    /// </summary>
    public float heal = 0.10f;

    /// <summary>
    /// Distance the drone can heal from
    /// </summary>
    public float firingRange = 35f;

    /// <summary>
    /// Beam generated from the drone
    /// </summary>
    public GameObject beam;

    /// <summary>
    /// Sound of the beam
    /// </summary>
    public AudioClip beamSound;

    /// <summary>
    /// waypoints for the patrol route
    /// </summary>
    public Transform[] wayPoints;

    private bool isCaptured = false;

    private enum ColliderStatus { enter, stay, exit };

    private AudioSource audioSource;

    private GameObject healerObjective;

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
            if (AuxiliarOperations.IsEnemy(other))
            {
                OnTriggerBehaviour(other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                OnTriggerBehaviour(other);
            }
        }
    }

    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                OnTriggerBehaviour(other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                OnTriggerBehaviour(other);
            }
        }
    }

    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (healerObjective.Equals(other.gameObject))
                {
                    GoToAlertState();
                }
                else
                {
                    healerObjective = null;
                }              
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                if (healerObjective.Equals(other.gameObject))
                {
                    GoToAlertState();
                }
                else
                {
                    healerObjective = null;
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
        if (AuxiliarOperations.IsPlayer(other) || AuxiliarOperations.IsEnemy(other))
        {
            if (other.gameObject.GetComponent<BasicDrone>().life < other.gameObject.GetComponent<BasicDrone>().maxHeath)
            {
                if (healerObjective == null)
                {
                    healerObjective = other.gameObject;
                    if (!isCaptured) {
                        GoToAttackState();
                    } else {
                        GoToCapturedState();
                    }
                }
                else
                {
                    if (Vector3.Distance(healerObjective.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                    {
                        healerObjective = other.gameObject;
                    }

                    if (!isCaptured)
                    {
                        GoToAttackState();
                    }
                    else
                    {
                        GoToCapturedState();
                    }
                }
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

    /// <summary>
    /// Custom Attack function of the healer drone
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    public void Attack(GameObject enemy)
    {
        //because the drone heals from a close area, the drone closes to their objective
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemy.transform.position;
    }

    /// <summary>
    /// Generate the beam
    /// </summary>
    /// <param name="enemy">objective of the beam</param>
    private void HealBeam(GameObject enemy)
    {
        healerObjective = enemy;
        gameObject.transform.LookAt(enemy.transform);
        enemy.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
        beam.SetActive(true);
        beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos = gameObject.transform.InverseTransformPoint(enemy.transform.position);
        audioSource.PlayOneShot(beamSound, 1);
    }


    // Update is called once per frame
    void Update()
    {
        

        // Switch on the statr enum.
        switch (currentState)
        {
            case droneState.ATTACK:
                //Keeps healing the objectives if their healt is not at maximum
                if (!GetComponent<CommonInterface>().isDestroyed())
                {
                    if (healerObjective != null)
                    {
                        if (!AuxiliarOperations.IsDestroyed(healerObjective) && (healerObjective.GetComponent<BasicDrone>().life < healerObjective.GetComponent<BasicDrone>().maxHeath))
                        {
                            if (!healerObjective.GetComponent<CommonInterface>().isDestroyed())
                            {
                                agent.destination = gameObject.transform.position;
                                HealBeam(healerObjective);
                            }
                        }
                        else
                        {
                            healerObjective = null;
                            GoToPatrolState();
                        }
                    }
                    else
                    {
                        beam.SetActive(false);
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
                if (!isCaptured && healerObjective != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(healerObjective))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;
                            if (!healerObjective.GetComponent<CommonInterface>().isDestroyed())
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    agent.destination = healerObjective.transform.position;
                                }
                            }
                            else
                            {
                                if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                                {
                                    healerObjective = null;
                                    GoToPatrolState();
                                }
                            }
                        }
                        else
                        {
                            if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                            {
                                healerObjective = null;
                                GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (currentState != droneState.ATTACK || currentState != droneState.CAPTURED)
                        {
                            healerObjective = null;
                            GoToPatrolState();
                        }
                    }
                }
                break;
            case droneState.CAPTURED:
                //Keeps healing the objectives if their healt is not at maximum
                if (!GetComponent<CommonInterface>().isDestroyed())
                {
                    if (healerObjective != null)
                    {
                        if (!AuxiliarOperations.IsDestroyed(healerObjective) && (healerObjective.GetComponent<BasicDrone>().life < healerObjective.GetComponent<BasicDrone>().maxHeath))
                        {
                            if (!healerObjective.GetComponent<CommonInterface>().isDestroyed())
                            {
                                HealBeam(healerObjective);
                            }
                        }
                        else
                        {
                            healerObjective = null;
                        }
                    }
                    else
                    {
                        beam.SetActive(false);
                    }
                }
                break;
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;
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
