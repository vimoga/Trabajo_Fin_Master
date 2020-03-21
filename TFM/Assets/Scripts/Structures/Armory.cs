using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armory : MonoBehaviour, StructuresInterfaces
{
    public int ammoRecovery = 1;

    public int ammoRecoveryRate = 5;

    // Distance the drone can aim and fire from
    public float firingRange = 25f;

    public float explosionDamage = 200;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject ammoEffect;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject ammoWave;

    private bool isCaptured = false;

    private bool isDestroyed = false;

    private float currentRecoveryRate = 0;

    private enum colliderStatus { enter, stay, exit, destroyed };

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        isCaptured = GetComponent<BasicStructure>().isCaptured;
    }


    // Detect an player
    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.enter, other);
            }
        }
        if (isDestroyed) {
            ColliderBehaviour(colliderStatus.destroyed, other);
        }
    }


    // keep player recovery
    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                ColliderBehaviour(colliderStatus.stay, other);
            }
        }
        if (isDestroyed)
        {
            ColliderBehaviour(colliderStatus.destroyed, other);
        }
    }

    // Stop recovery
    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (isCaptured)
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

                if ((currentRecoveryRate > ammoRecoveryRate))
                {
                    other.transform.gameObject.SendMessage("AmmoIn", ammoRecovery, SendMessageOptions.RequireReceiver);
                    if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().ammo !=-1) && (other.transform.gameObject.GetComponent<BasicDrone>().ammo < other.transform.gameObject.GetComponent<BasicDrone>().maxAmmo))
                    {
                        if (!ammoEffect.activeSelf && !ammoWave.activeSelf)
                        {
                            ammoEffect.SetActive(true);
                            ammoWave.SetActive(true);
                        }
                    }
                }
                break;
            case colliderStatus.stay:

                
                if (AuxiliarOperations.IsDestroyed(other.transform.gameObject) || (other.transform.gameObject.GetComponent<BasicDrone>().ammo >= other.transform.gameObject.GetComponent<BasicDrone>().maxAmmo))
                {
                    ammoEffect.SetActive(false);
                    ammoWave.SetActive(false);
                }
                else
                {
                    if ((currentRecoveryRate > ammoRecoveryRate))
                    {
                        other.transform.gameObject.SendMessage("AmmoIn", ammoRecovery, SendMessageOptions.RequireReceiver);
                        if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && (other.transform.gameObject.GetComponent<BasicDrone>().ammo != -1) && (other.transform.gameObject.GetComponent<BasicDrone>().ammo < other.transform.gameObject.GetComponent<BasicDrone>().maxAmmo))
                        {
                            if (!ammoEffect.activeSelf && !ammoWave.activeSelf)
                            {
                                ammoEffect.SetActive(true);
                                ammoWave.SetActive(true);
                            }
                        }
                    }
                }

                break;
            case colliderStatus.exit:

                if (ammoEffect.activeSelf && ammoWave.activeSelf)
                {
                    ammoEffect.SetActive(false);
                    ammoWave.SetActive(false);
                }

                break;

            case colliderStatus.destroyed:

                if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject))
                {
                    other.transform.gameObject.SendMessage("Impact", explosionDamage, SendMessageOptions.RequireReceiver);
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
        isDestroyed = GetComponent<CommonInterface>().isDestroyed();
        currentRecoveryRate += Time.deltaTime;
    }


}
