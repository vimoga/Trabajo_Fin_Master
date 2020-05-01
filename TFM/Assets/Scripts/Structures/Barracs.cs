using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Barracs Structure
/// </summary>
public class Barracs : MonoBehaviour, StructuresInterfaces
{
    /// <summary>
    /// Health recovery quantity
    /// </summary>
    public float heal = 0.10f;

    /// <summary>
    /// Distance the structure can provide health
    /// </summary>
    public float firingRange = 25f;

    /// <summary>
    /// Health recovery effect
    /// </summary>
    public GameObject healEffect;

    /// <summary>
    /// Health recovery ground wave effect
    /// </summary>
    public GameObject HealWave;

    private bool isCaptured=false;

    private enum colliderStatus { enter, stay, exit };

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        isCaptured = GetComponent<BasicStructure>().isCaptured;
    }

    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemyDrone(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
    }

    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemyDrone(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }

    }

    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemyDrone(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="colStatus">type of collider</param>
    /// <param name="other">object collided</param>
    private void ColliderBehaviour(colliderStatus colStatus, Collider other)
    {
        switch (colStatus)
        {
            case colliderStatus.enter:

                
                if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().life < other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                {
                    // provide health recovery
                    other.transform.gameObject.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
                    if (!healEffect.activeSelf && !HealWave.activeSelf)
                    {

                        healEffect.SetActive(true);
                        HealWave.SetActive(true);
                    }
                }

                break;
            case colliderStatus.stay:

                
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject) || (other.transform.gameObject.GetComponent<BasicDrone>().life >= other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                {
                    healEffect.SetActive(false);
                    HealWave.SetActive(false);
                }
                else {
                    if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().life < other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                    {
                        // provide health recovery until the drone reaches maximun health
                        other.transform.gameObject.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
                        if (!healEffect.activeSelf && !HealWave.activeSelf)
                        {                           
                            healEffect.SetActive(true);
                            HealWave.SetActive(true);
                        }
                    }

                }

                break;
            case colliderStatus.exit:

                //cease health recovery effect
                if (healEffect.activeSelf && HealWave.activeSelf)
                {
                    healEffect.SetActive(false);
                    HealWave.SetActive(false);
                }

                break;
        }
    }

    public void Attack()
    {
        Debug.Log("Barracs are unable to attack");
    }

    public void SetCaptured(bool isCaptured)
    {
        GetComponent<BasicStructure>().isCaptured = isCaptured;
    }

    public bool GetCaptured()
    {
        return isCaptured;
    }

    // Update is called once per frame
    void Update()
    {
        isCaptured = GetComponent<BasicStructure>().isCaptured;
    }
    
}
