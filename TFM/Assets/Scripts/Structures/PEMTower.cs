using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the PEM tower Structure
/// </summary>
public class PEMTower : MonoBehaviour, StructuresInterfaces
{
    /// <summary>
    /// Damage of the PEM effect
    /// </summary>
    public float damage = 0.10f;

    /// <summary>
    /// Distance the structure can provide damage
    /// </summary>
    public float firingRange = 85f;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMEffect;

    /// <summary>
    /// PEM ground wave effect
    /// </summary>
    public GameObject PEMWave;

    /// <summary>
    /// Energy generator of the PEM Tower
    /// </summary>
    public GameObject energyGenerator;

    private enum colliderStatus { enter, stay, exit };

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;
    }

    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator)) {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }               
    }

    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator))
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        else
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                StopDamage(other.transform.gameObject);
            }
        }
    }

    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator))
        {
            if (AuxiliarOperations.IsPlayer(other))
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

                DealDamage(other.transform.gameObject);

                break;
            case colliderStatus.stay:

                // provide PEM recovery until the enemy drone is destroyed
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                {
                    other.transform.gameObject.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }
                else
                {
                    DealDamage(other.transform.gameObject);
                }

                if (gameObject.IsDestroyed())
                {
                    StopDamage(other.transform.gameObject);
                }

                break;
            case colliderStatus.exit:

                StopDamage(other.transform.gameObject);
                break;
        }
    }

    /// <summary>
    /// Custom Attack function of the AA Gun
    /// </summary>
    /// <param name="other">enemy of the PEM Tower</param>
    private void DealDamage(GameObject other)
    {
        other.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
        other.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
        if (!PEMEffect.activeSelf && !PEMWave.activeSelf)
        {
            PEMEffect.SetActive(true);
            PEMWave.SetActive(true);
        }
    }

    /// <summary>
    /// Cancel attack effects when the enemy on no longer reachable
    /// </summary>
    private void StopDamage(GameObject other)
    {
        other.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);

        if (PEMEffect.activeSelf && PEMWave.activeSelf)
        {
            PEMEffect.SetActive(false);
            PEMWave.SetActive(false);
        }
    }

    public void SetCaptured(bool isCaptured)
    {
        GetComponent<BasicStructure>().isCaptured = isCaptured;
    }

    public bool GetCaptured()
    {
        return GetComponent<BasicStructure>().isCaptured;
    }


    public void Attack()
    {
        Debug.Log("PEMTowers are unable to attack");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
