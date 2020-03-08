using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThankDrone : MonoBehaviour,DroneInterface
{

    public AudioClip shootSound;

    public float damage = 75f;

    public float firerate = 5f;

    // Distance the turret can aim and fire from
    public float firingRange = 30;

    // Gameobjects need to control rotation and aiming
    public GameObject tnk_turret;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    private GameObject tnk_enemy;

    private AudioSource audioSource;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();

        if (gameObject.tag == "Player" || gameObject.tag == "Player_Drone") {
            isCaptured = true;
        }
        //audioSource.Stop();
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
                tnk_enemy = null;
            }
        }

        //fix
        //tnk_turret.transform.rotation = gameObject.transform.rotation;

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
        }
    }

    public void Attack(GameObject enemy)
    {
        //fix
        //tnk_turret.transform.LookAt(enemy.transform);
        gameObject.transform.LookAt(enemy.transform);

        if ((currentFireRate > firerate))
        {
            if (enemy)
            {
                enemy.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }

            currentFireRate = 0;

        }

        // start particle system 
        if (!muzzelFlash.isPlaying)
        {
            muzzelFlash.Play();
            audioSource.PlayOneShot(shootSound,1);
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
        if (!isCaptured && tnk_enemy != null) {
            if (!AuxiliarOpereations.IsDestroyed(tnk_enemy))
            {
                if (!tnk_enemy.GetComponent<CommonInterface>().isDestroyed())
                {
                    Attack(tnk_enemy);
                }
                
            }
            else {
                tnk_enemy = null;
            }           
        }

        currentFireRate += Time.deltaTime;
    }
    
}
