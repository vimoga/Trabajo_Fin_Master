using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderComunicator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (transform.parent.GetComponent<DroneInterface>()!= null) {
            transform.parent.GetComponent<DroneInterface>().OnTriggerEnter(other);
        } else if (transform.parent.GetComponent<StructuresInterfaces>() != null)
        {
            transform.parent.GetComponent<StructuresInterfaces>().OnTriggerEnter(other);
        }
    }

    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (transform.parent.GetComponent<DroneInterface>() != null)
        {
            transform.parent.GetComponent<DroneInterface>().OnTriggerStay(other);
        }
        else if (transform.parent.GetComponent<StructuresInterfaces>() != null)
        {
            transform.parent.GetComponent<StructuresInterfaces>().OnTriggerStay(other);
        }
    }


    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (transform.parent.GetComponent<DroneInterface>() != null)
        {
            transform.parent.GetComponent<DroneInterface>().OnTriggerExit(other);
        }
        else if (transform.parent.GetComponent<StructuresInterfaces>() != null)
        {
            transform.parent.GetComponent<StructuresInterfaces>().OnTriggerExit(other);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
