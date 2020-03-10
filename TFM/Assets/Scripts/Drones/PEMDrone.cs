using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PEMDrone : MonoBehaviour, DroneInterface
{
    public float damage = 0.10f;

    // Distance the drone can aim and fire from
    public float firingRange = 25f;

    /// <summary>
    /// PEM effect
    /// </summary>
    public GameObject PEMEffect;

    private bool isCaptured = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        if (gameObject.tag == "Player" || gameObject.tag == "Player_Drone")
        {
            isCaptured = true;
        }
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                other.transform.gameObject.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                other.transform.gameObject.SendMessage("StuntIn", SendMessageOptions.RequireReceiver);
            }
        }
    }


    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }
        }
        else {
            if (AuxiliarOperations.IsEnemy(other))
            {
                other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }
        }

    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                other.transform.gameObject.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);
            }
        }
        else
        {
            if (AuxiliarOperations.IsEnemy(other))
            {
                other.transform.gameObject.SendMessage("StuntOut", SendMessageOptions.RequireReceiver);
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
 
    }

    // Update is called once per frame
    void Update()
    {

    }

    
}
