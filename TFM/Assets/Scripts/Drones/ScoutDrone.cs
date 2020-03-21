using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoutDrone : MonoBehaviour, DroneInterface
{

    public AudioClip shootSound;

    public float damage = 2f;

    public float firerate = 10f;

    // Distance the turret can aim and fire from
    public float firingRange = 7;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    private GameObject scout_enemy;

    private AudioSource audioSource;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    public bool attackAerialEnemies = false; 

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();

        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }


    // Detect an Enemy, aim and fire
    void DroneInterface.OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }
    }


    // keep firing
    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }

    }

    // Stop firing
    void DroneInterface.OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
            {
                scout_enemy = null;
            }
        }
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
        {
            if (scout_enemy == null)
            {
                scout_enemy = other.gameObject;
            }
            else
            {
                if (Vector3.Distance(scout_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    scout_enemy = other.gameObject;
                }
            }
        }
    }

    public void Attack(GameObject enemy)
    {
        gameObject.transform.LookAt(enemy.transform);
     
        if ((currentFireRate > firerate))
        {
            if (enemy)
            {
                enemy.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }

            currentFireRate = 0;

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
                audioSource.PlayOneShot(shootSound, 1);
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

    // Update is called once per frame
    void Update()
    {
        if (!isCaptured && scout_enemy != null)
        {
            if (!AuxiliarOperations.IsDestroyed(scout_enemy))
            {
                if (!scout_enemy.GetComponent<CommonInterface>().isDestroyed())
                {
                    Attack(scout_enemy);
                }

            }
            else
            {
                scout_enemy = null;
            }
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        currentFireRate += Time.deltaTime;
    }

}