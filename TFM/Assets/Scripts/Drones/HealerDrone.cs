using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerDrone : MonoBehaviour, DroneInterface
{
    public float heal = 0.10f;

    // Distance the drone can aim and fire from
    public float firingRange = 35f;

    public GameObject beam;

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

    // Detect an Enemy, aim and fire
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
        //OnTriggerBehaviour(other);
    }


    // keep firing
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
        //OnTriggerBehaviour(other);
    }

    // Stop firing
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

    public void Attack(GameObject enemy)
    {
        gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>().destination = enemy.transform.position;
    }

    private void HealBeam(GameObject enemy)
    {
        healerObjective = enemy;
        gameObject.transform.LookAt(enemy.transform);
        enemy.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
        beam.SetActive(true);
        beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos = enemy.transform.position;
        audioSource.PlayOneShot(beamSound, 1);
    }


    // Update is called once per frame
    void Update()
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
        else {
            beam.SetActive(false);
        }
        
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }


}
