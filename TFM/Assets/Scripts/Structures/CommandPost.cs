using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviour for the Command Post Structure
/// </summary>
public class CommandPost : MonoBehaviour, StructuresInterfaces
{

    /// <summary>
    /// CPU power to gain when is captured
    /// </summary>
    public int cpuPower;

    private bool isCaptured = false;

    private bool isAddedToHUD = false;

    private GameplayManager gameplayManager;

    // Start is called before the first frame update
    void Start()
    {
        gameplayManager = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();
        if (GameConstants.postCaptured.Contains(gameObject.name))
        {
            GetComponent<BasicStructure>().SetAsCatured();
            //chage selection if active
            GameObject selection = AuxiliarOperations.GetChildObject(gameObject.transform, "Selection");
            if (selection)
            {
                selection.SetActive(true);
                selection.GetComponent<RawImage>().texture = (Texture)Resources.Load("Textures/selection_friend");
            }
        }
        else
        {
            isCaptured = GetComponent<BasicStructure>().isCaptured;
        }
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
            gameplayManager.AddMaxCPU(cpuPower);
            isAddedToHUD = true;
            if (!GameConstants.postCapturedTemp.Contains(gameObject.name))
            {
                GameConstants.postCapturedTemp.Add(gameObject.name);
            }               
        }
    }
   
}
