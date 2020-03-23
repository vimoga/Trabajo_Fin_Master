﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainDrone : MonoBehaviour, DroneInterface
{

    public AudioClip shootSound;


    // Distance the turret can aim and fire from
    public float firingRange = 7;

    public GameObject beam;

    private GameObject main_enemy;

    private AudioSource audioSource;

    private bool isCaptured = false;


    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();

        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    public void Attack(GameObject enemy)
    {
        main_enemy = enemy;
        gameObject.transform.LookAt(enemy.transform);
        enemy.SendMessage("Capture", SendMessageOptions.RequireReceiver);
        beam.SetActive(true);
        //beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos=enemy.transform.position.normalized;
        audioSource.PlayOneShot(shootSound, 1);
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
        if (main_enemy) {
            if (AuxiliarOperations.IsCaptured(main_enemy))
            {
                beam.SetActive(false);
            }
            else
            {
                //beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().StartPos = gameObject.transform.position.normalized;
                beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos = main_enemy.transform.position.normalized;
            }
        }
        
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    
}
