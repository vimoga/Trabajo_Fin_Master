using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Radar Tower Structure
/// </summary>
public class Radar : MonoBehaviour, StructuresInterfaces
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Attack()
    {
        Debug.Log("Eadars are unable to attack");
    }

    public void SetCaptured(bool isCaptured)
    {
        GetComponent<BasicStructure>().isCaptured = isCaptured;
    }

    public bool GetCaptured()
    {
        return GetComponent<BasicStructure>().isCaptured;
    }

    public void OnTriggerEnter(Collider other)
    {
    }

    public void OnTriggerStay(Collider other)
    {
       
    }

    public void OnTriggerExit(Collider other)
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
