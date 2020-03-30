using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Tank Drone
/// </summary>
public class TankDrone : MonoBehaviour,DroneInterface
{
    /// <summary>
    /// Sound of the shoots
    /// </summary>
    public AudioClip shootSound;

    /// <summary>
    /// Damage of the shoots
    /// </summary>
    public float damage = 75f;

    /// <summary>
    /// Time between the shoots
    /// </summary>
    public float firerate = 5f;

    /// <summary>
    /// Distance the drone can fire from
    /// </summary>
    public float firingRange = 30;

    /// <summary>
    /// Turret of the tank
    /// </summary>
    public GameObject tnk_turret;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    private GameObject tnk_enemy;

    private AudioSource audioSource;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();

        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }

    void DroneInterface.OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }
    }

    void DroneInterface.OnTriggerStay(Collider other)
    {
        if (!isCaptured) {
            OnTriggerBehaviour(other);
        }      
    }

    void DroneInterface.OnTriggerExit(Collider other)
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

    /// <summary>
    /// Custom Attack function of the tank drone
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    public void Attack(GameObject enemy)
    {

        if (!isCaptured)
        {
            MakeAttack(enemy);
        }
        else
        {
            //only attack if there is ammo remaining
            if (gameObject.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO)
            {
                MakeAttack(enemy);
            }
            else
            {
                if (gameObject.GetComponent<BasicDrone>().ammo > 0 && !AuxiliarOperations.EnemyIsAerial(gameObject, enemy))
                {
                    MakeAttack(enemy);
                }
            }
        }             
    }

    private void MakeAttack(GameObject enemy)
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

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
                audioSource.PlayOneShot(shootSound, 1);
            }

            if (isCaptured)
            {
                gameObject.GetComponent<BasicDrone>().AmmoOut();
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
        //attack player drones when is not capture
        if (!isCaptured && tnk_enemy != null) {
            if (!AuxiliarOperations.IsDestroyed(tnk_enemy) && !AuxiliarOperations.EnemyIsAerial(gameObject, tnk_enemy))
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

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        currentFireRate += Time.deltaTime;
    }
    
}
