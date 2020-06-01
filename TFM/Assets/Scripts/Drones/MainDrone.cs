using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the main Drone
/// </summary>
public class MainDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Sound of the beam
    /// </summary>
    public AudioClip shootSound;

    /// <summary>
    /// Beam generated from the drone
    /// </summary>
    public GameObject beam;

    private float firingRange;

    private GameObject main_enemy;

    private AudioSource audioSource;

    private bool isCaptured = false;

    private Vector3 oldPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        firingRange = this.GetComponentInChildren<SphereCollider>().radius;
        audioSource = GetComponent<AudioSource>();
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    /// <summary>
    /// Custom Attack function of the main drone
    /// </summary>
    /// <param name="enemy">objective of the capture</param>
    public void Attack(GameObject enemy)
    {
        if (enemy.GetComponent<BasicStructure>())
        {
            if (!enemy.GetComponent<BasicStructure>().isCapturable)
            {
                audioSource.PlayOneShot((AudioClip)Resources.Load("Sounds/Error"));
            }
            else {
                MakeAttack(enemy);
            }
        }
        else {
            if (AuxiliarOperations.IsCapturePosible(enemy.GetComponent<BasicDrone>().captureCost))
            {
                MakeAttack(enemy);
            }
            else {
                audioSource.PlayOneShot((AudioClip)Resources.Load("Sounds/No_CPU"));
            }
        }       
    }

    /// <summary>
    /// Makes the attack effect of the drone
    /// </summary>
    /// <param name="enemy">objective of the capture</param>
    public void MakeAttack(GameObject enemy) {
        main_enemy = enemy;
        enemy.SendMessage("Capture", SendMessageOptions.RequireReceiver);
        beam.SetActive(true);
        audioSource.PlayOneShot(shootSound, 1);
        oldPosition = gameObject.transform.position;

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
        return firingRange * transform.localScale.x;
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
        //cease beam when the capture is completed
        if (main_enemy)
        {
            if (AuxiliarOperations.IsCaptured(main_enemy) || Vector3.Distance(main_enemy.transform.position, transform.position) > GetFiringRange())
            {
                beam.SetActive(false);
                main_enemy = null;
            }
            else
            {
                //fix for player movement
                if (Vector3.Distance(oldPosition, transform.position) < 1)
                {
                    beam.GetComponent<VolumetricLines.VolumetricLineBehavior>().EndPos = gameObject.transform.InverseTransformPoint(main_enemy.transform.position);
                }
                else {
                    beam.SetActive(false);
                    main_enemy = null;
                }
                
            }
        }
        else {
            if (beam.activeSelf) {
                beam.SetActive(false);
            }
        }
        
        isCaptured = GetComponent<BasicDrone>().isCaptured;

        if (AuxiliarOperations.IsDestroyed(gameObject))
        {
            beam.SetActive(false);
        }
    }    
}
