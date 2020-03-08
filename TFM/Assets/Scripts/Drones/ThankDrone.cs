using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThankDrone : MonoBehaviour,DroneInterface
{

    public AudioClip shootSound;

    public float damage = 75f;

    public float firerate = 5f;

    // Gameobjects need to control rotation and aiming
    public GameObject tnk_turret;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    private GameObject tnk_enemy;

    // Used to start and stop the turret firing
    private bool canFire = false;

    private AudioSource audioSource;

    private float currentFireRate = 0;

    private bool isCaptured = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }
    }


    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (!isCaptured) {
            OnTriggerBehaviour(other);
        }
        
    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {

        if (!isCaptured)
        {
            if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
            {
                canFire = false;
                tnk_enemy = null;
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
            if (tnk_enemy == null)
            {
                tnk_enemy = other.gameObject;

            }
            else
            {
                if (Vector3.Distance(tnk_enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {

                    tnk_enemy = other.gameObject;
                }
            }

            canFire = true;
        }
    }

    public void Attack(GameObject enemy)
    {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
