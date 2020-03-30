using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Scout Drone
/// </summary>
public class ScoutDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Sound of the shoots
    /// </summary>
    public AudioClip shootSound;

    /// <summary>
    /// Damage of the shoots
    /// </summary>
    public float damage = 2f;

    /// <summary>
    /// Time between the shoots
    /// </summary>
    public float firerate = 10f;

    /// <summary>
    /// Distance the drone can fire from
    /// </summary> 
    public float firingRange = 7;

    /// <summary>
    /// effect of shooting
    /// </summary>
    public ParticleSystem muzzelFlash;

    private GameObject scout_enemy;

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
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }

    }

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

    /// <summary>
    /// Custom Attack function of the scout drone
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
                if (gameObject.GetComponent<BasicDrone>().ammo > 0)
                {
                    MakeAttack(enemy);
                }
            }
        }      
    }

    /// <summary>
    /// Generate the effects, animation and behaviour of the drone attack
    /// </summary>
    /// <param name="enemy">objective of the attack</param>
    private void MakeAttack(GameObject enemy)
    {
        Vector3 targetPostition = new Vector3(enemy.transform.position.x, gameObject.transform.position.y, enemy.transform.position.z);
        gameObject.transform.LookAt(targetPostition);
        muzzelFlash.gameObject.transform.LookAt(enemy.transform);

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
        //attack player drones when is not captured
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