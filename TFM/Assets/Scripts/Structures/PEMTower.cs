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
        this.GetComponent<SphereCollider>().radius = firingRange;
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator)) {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
               
    }


    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (!AuxiliarOperations.IsDestroyed(energyGenerator))
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
    }

    // Stop firing
    void OnTriggerExit(Collider other)
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

                other.transform.gameObject.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
                if (!PEMEffect.activeSelf && !PEMWave.activeSelf)
                {
                    PEMEffect.SetActive(true);
                    PEMWave.SetActive(true);
                }

                break;
            case colliderStatus.stay:

                other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                {
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }

                break;
            case colliderStatus.exit:

                other.transform.gameObject.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);

                if (PEMEffect.activeSelf && PEMWave.activeSelf)
                {
                    PEMEffect.SetActive(false);
                    PEMWave.SetActive(false);
                }

                break;
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
