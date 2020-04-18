using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Healer Drone
/// </summary>
public class HealerDrone : MonoBehaviour,DroneInterface
{
    /// <summary>
    /// Heal quantity
    /// </summary>
    public float heal = 0.10f;

    /// <summary>
    /// Beam generated from the drone
    /// </summary>
    public GameObject beam;

    /// <summary>
    /// Sound of the beam
    /// </summary>
    public AudioClip beamSound;

    private BasicDrone drone;

    private float firingRange;

    private bool isCaptured = false;

    private enum ColliderStatus { enter, stay, exit };

    private AudioSource audioSource;

    private GameObject healerObjective;

    private Transform[] wayPoints;

    private NavMeshAgent agent;

    private int nextWayPoint = 0;

    private float currentAlertTime = 0;

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
            if (AuxiliarOperations.IsEnemy(other) && !other.isTrigger)
            {
                OnTriggerBehaviour(other);
                drone.GoToAttackState();
            }
        }
        else
        {
            if (AuxiliarOperations.IsPlayer(other) && !other.isTrigger)
            {
                OnTriggerBehaviour(other);
            }
        }
    }

    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsEnemy(other) && !other.isTrigger)
            {
                OnTriggerBehaviour(other);
                //drone.GoToAttackState();
            }
        }
        else
        {
            if (AuxiliarOperations.IsPlayer(other) && !other.isTrigger)
            {
                OnTriggerBehaviour(other);
            }
        }
    }

    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                if (healerObjective != null && healerObjective.Equals(other.gameObject))
                {
                    drone.GoToAlertState();
                }
                else
                {
                    healerObjective = null;
                }
                HealBeamCease();
            }
        }
        else {
            if (AuxiliarOperations.IsPlayer(other))
            {
                if (healerObjective != null && healerObjective.Equals(other.gameObject))
                {
                    drone.GoToAlertState();
                }
                else
                {
                    healerObjective = null;
                }
                HealBeamCease();
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
                }
                else
                {
                    if (Vector3.Distance(healerObjective.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                    {
                        healerObjective = other.gameObject;
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
        return firingRange * transform.localScale.x;
    }

    /// <summary>
    /// Decies if attack or not
    /// </summary>
    private void MakeAttack() {
        //Keeps healing the objectives if their healt is not at maximum 
        if (!AuxiliarOperations.IsDestroyed(healerObjective))
        {
            if (Vector3.Distance(healerObjective.transform.position, transform.position) < GetFiringRange() && (healerObjective.GetComponent<BasicDrone>().life < healerObjective.GetComponent<BasicDrone>().maxHeath))
            {
                agent.destination = gameObject.transform.position;
                HealBeam(healerObjective);
            }
            else
            {
                healerObjective = null;
                if (!isCaptured) {
                    drone.GoToPatrolState();
                }               
            }
        }
        else
        {
            HealBeamCease();
        }
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
        gameObject.transform.LookAt(healerObjective.transform);
        healerObjective.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
        beam.SetActive(true);
        beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos = gameObject.transform.InverseTransformPoint(enemy.transform.position);
        audioSource.PlayOneShot(beamSound, 1);
    }

    /// <summary>
    /// Cease the beam fire
    /// </summary>
    private void HealBeamCease()
    {
        beam.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {      
        // Switch on the statr enum.
        switch (drone.currentState)
        {
            case DroneState.ATTACK:
                MakeAttack();
                break;
            case DroneState.PATROL:
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
            case DroneState.ALERT:
                //follow player drones when is not captured
                if (!isCaptured && healerObjective != null)
                {
                    if (!AuxiliarOperations.IsDestroyed(healerObjective))
                    {
                        if (currentAlertTime < GameConstants.ALERT_TIME)
                        {
                            currentAlertTime = 0;                         
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                agent.destination = healerObjective.transform.position;
                            }                      
                        }
                        else
                        {
                            if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                            {
                                healerObjective = null;
                                drone.GoToPatrolState();
                            }
                        }
                    }
                    else
                    {
                        if (drone.currentState != DroneState.ATTACK || drone.currentState != DroneState.CAPTURED)
                        {
                            healerObjective = null;
                            drone.GoToPatrolState();
                        }
                    }
                }
                break;
            case DroneState.CAPTURED:
                MakeAttack();
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
