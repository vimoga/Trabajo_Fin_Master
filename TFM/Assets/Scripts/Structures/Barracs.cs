using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barracs : MonoBehaviour, StructuresInterfaces
{
    public float heal = 0.10f;

    // Distance the drone can aim and fire from
    public float firingRange = 25f;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject healEffect;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject HealWave;

    private bool isCaptured=false;

    private enum colliderStatus { enter, stay, exit };

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        isCaptured = GetComponent<BasicStructure>().isCaptured;
    }


    // Detect an Enemy, aim and fire
    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
    }


    // keep firing
    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }

    }

    // Stop firing
    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                ColliderBehaviour(colliderStatus.exit, other);
            }
        }
    }

    private void ColliderBehaviour(colliderStatus colStatus, Collider other)
    {
        switch (colStatus)
        {
            case colliderStatus.enter:

                other.transform.gameObject.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
                if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().life < other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                {
                    if (!healEffect.activeSelf && !HealWave.activeSelf)
                    {
                        healEffect.SetActive(true);
                        HealWave.SetActive(true);
                    }
                }

                break;
            case colliderStatus.stay:

                other.transform.gameObject.SendMessage("Heal", heal, SendMessageOptions.RequireReceiver);
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject) || (other.transform.gameObject.GetComponent<BasicDrone>().life >= other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                {
                    healEffect.SetActive(false);
                    HealWave.SetActive(false);
                }
                else {
                    if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().life < other.transform.gameObject.GetComponent<BasicDrone>().maxHeath))
                    {
                        if (!healEffect.activeSelf && !HealWave.activeSelf)
                        {
                            healEffect.SetActive(true);
                            HealWave.SetActive(true);
                        }
                    }

                }

                break;
            case colliderStatus.exit:

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
