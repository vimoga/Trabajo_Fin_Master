﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Radar Tower Structure
/// </summary>
public class Radar : MonoBehaviour, StructuresInterfaces
{

    /// <summary>
    /// Plane of where the fog of war is implemented
    /// </summary>
    public GameObject fogOfWarPlane;

    /// <summary>
    /// Used to identify the radar on the shader variables
    /// </summary>
    public int radarNumber;

    public Transform spawnPoint;

    /// <summary>
    /// Distance the structure can provide cover
    /// </summary>
    //private float coverRange;

    /// <summary>
    /// Distance where the fog of war is revelated
    /// </summary>
    private float fogOfWarCover;

    //private float coverRangeSqr { get { return coverRange * coverRange; } }
    /*private float coverRangeSqr;
    private Mesh m_Mesh;
    private Vector3[] vertices;
    private Color[] colors;*/

    private bool isCaptured = false;

    private bool isDestroyed = false;

    private bool isDrawed = false;

    // Start is called before the first frame update
    void Start()
    {

        // Set the firing range distance
        float coverRange = this.GetComponentInChildren<SphereCollider>().radius;

        fogOfWarCover = coverRange * transform.localScale.x;

        if (GameConstants.radarCaptured.Contains(gameObject.name))
        {
            GetComponent<BasicStructure>().SetAsCatured();
        }
        else {
            isCaptured = GetComponent<BasicStructure>().isCaptured;
        }
        

    }

    public void Attack()
    {
        Debug.Log("Radars are unable to attack");
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
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (!other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("InCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }       
    }

    public void OnTriggerStay(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (!other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("InCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (isCaptured)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                if (other.transform.gameObject.GetComponent<BasicDrone>())
                {
                    if (other.transform.gameObject.GetComponent<BasicDrone>().isOnCover)
                    {
                        other.transform.gameObject.SendMessage("OutCover", SendMessageOptions.RequireReceiver);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isCaptured && !isDrawed )
        {
            fogOfWarPlane.GetComponent<FogOfWar>().activateRadar(radarNumber, gameObject.transform.position,fogOfWarCover);
            isDrawed = true;

            if (!GameConstants.radarCaptured.Contains(gameObject.name)) {
                GameConstants.spawnPoint = new Vector3(spawnPoint.position.x, spawnPoint.position.y, spawnPoint.position.z);
                GameConstants.radarCaptured.Add(gameObject.name);
            }                          
        }
       
        isCaptured = GetComponent<BasicStructure>().isCaptured;
        isDestroyed = GetComponent<CommonInterface>().isDestroyed();
    }
    
}
