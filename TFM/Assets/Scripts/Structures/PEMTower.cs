using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEMTower : MonoBehaviour, StructuresInterfaces
{
    public float damage = 0.10f;

    // Distance the drone can aim and fire from
    public float firingRange = 85f;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMEffect;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMWave;

    public GameObject energyGenerator;

    private enum colliderStatus { enter, stay, exit };

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;
    }


    // Detect an Enemy, aim and fire
    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator)) {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }               
    }


    // keep firing
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

    // Stop firing
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

    private void ColliderBehaviour(colliderStatus colStatus, Collider other)
    {
        switch (colStatus)
        {
            case colliderStatus.enter:

                DealDamage(other.transform.gameObject);

                break;
            case colliderStatus.stay:

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

                break;
            case colliderStatus.exit:

                StopDamage(other.transform.gameObject);
                break;
        }
    }

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
