using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Command Post Structure
/// </summary>
public class CommandPost : MonoBehaviour, StructuresInterfaces
{

    public int cpuPower;

    private bool isCaptured = false;

    private bool isAddedToHUD = false;

    private GameplayManager gameplayManager;

    // Start is called before the first frame update
    void Start()
    {
        gameplayManager = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();
    }

    public void Attack()
    {
        Debug.Log("Command Posts are unable to attack");
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
        isCaptured = GetComponent<BasicStructure>().isCaptured;
        if (isCaptured && !isAddedToHUD) {
            gameplayManager.AddCPUPower(cpuPower);
            isAddedToHUD = true;
        }
    }
   
}
