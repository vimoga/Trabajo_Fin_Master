using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Healer Drone
/// </summary>
public class HealerDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// heal quantity
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

    private bool isCaptured = false;

    private enum ColliderStatus { enter, stay, exit };

    private AudioSource audioSource;

    private GameObject healerObjective;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();

        isCaptured = GetComponent<BasicDrone>().isCaptured;
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
                healerObjective = null;
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                healerObjective = null;
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
              
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }


}
