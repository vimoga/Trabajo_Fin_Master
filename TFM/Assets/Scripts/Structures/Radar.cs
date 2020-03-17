using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
