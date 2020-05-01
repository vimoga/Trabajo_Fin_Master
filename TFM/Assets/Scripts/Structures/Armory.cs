using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Armory Structure
/// </summary>
public class Armory : MonoBehaviour, StructuresInterfaces
{
    /// <summary>
    /// Ammo recovery quantity
    /// </summary>
    public int ammoRecovery = 1;

    /// <summary>
    /// Ammo recovery rate
    /// </summary>
    public int ammoRecoveryRate = 5;

    /// <summary>
    /// Distance the structure can provide ammo
    /// </summary>
    public float firingRange = 25f;

    /// <summary>
    /// damage dealed when the structure is destroyed
    /// </summary>
    public float explosionDamage = 200;

    /// <summary>
    /// Ammo recovery effect
    /// </summary>
    public GameObject ammoEffect;

    /// <summary>
    /// Ammo recovery ground wave effect
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
        if (isDestroyed) {
            ColliderBehaviour(colliderStatus.destroyed, other);
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
        if (isDestroyed)
        {

            ColliderBehaviour(colliderStatus.destroyed, other);
        }
    }

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

                // provide ammo recovery
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

                // provide ammo recovery until the drone reaches maximun ammo
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
                        currentRecoveryRate = 0;
                    }
                }

                break;
            case colliderStatus.exit:

                //cease ammo recovery effect
                if (ammoEffect.activeSelf && ammoWave.activeSelf)
                {
                    ammoEffect.SetActive(false);
                    ammoWave.SetActive(false);
                }

                break;

            case colliderStatus.destroyed:

                // if the estructure is damaged deals explosion damage
                if (!AuxiliarOperations.IsDestroyed(other.transform.gameObject) && !other.isTrigger)
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
