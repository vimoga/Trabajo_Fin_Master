using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatlingGunCustom : MonoBehaviour
{
    // target the gun will aim at
    Transform go_target;

    // Gameobjects need to control rotation and aiming
    public Transform go_baseRotation;
    public Transform go_GunBody;
    public Transform go_barrel;

    // Gun barrel rotation
    public float barrelRotationSpeed;
    float currentRotationSpeed;

    // Distance the turret can aim and fire from
    public float firingRange;

    // Particle system for the muzzel flash
    public ParticleSystem muzzelFlash;

    // Used to start and stop the turret firing
    bool canFire = false;

    /// <summary>
    /// life of the structure
    /// </summary>
    public float life = 100;

    /// <summary>
    /// effect played when the drones are destroyed
    /// </summary>
    public GameObject explosion;

    private bool isDestroyed = false;

    private AudioSource audioSource;

    private GameObject enemy;

    public float damage = 1f;

    public float firerate = 0.15f;

    private float currentFireRate = 0;

    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    void Update()
    {
        if (!isDestroyed)
        {

            if (life > 0)
            {
                AimAndFire();
            }
            else
            {
                explosion.SetActive(true);

                audioSource.Stop();
                Object.Destroy(gameObject, 2.0f);
                isDestroyed = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {

        OnTriggerBehaviour(other);

    }

    // keep firing
    void OnTriggerStay(Collider other)
    {
        OnTriggerBehaviour(other);
    }

    
    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone") && !other.isTrigger)
        {
            canFire = false;
            go_target = null;
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
            if (go_target == null)
            {
                go_target = other.transform;

            }
            else
            {
                if (Vector3.Distance(go_target.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    go_target = other.transform;
                }
            }

            canFire = true;
        }
    }


    /// <summary>
    /// Obtenemos el disparo y restamos vida
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Gatling Gun hitted: " + life);
    }

    void AimAndFire()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // if can fire turret activates
        if (canFire)
        {
            // start rotation
            currentRotationSpeed = barrelRotationSpeed;

            // aim at enemy
            Vector3 baseTargetPostition = new Vector3(go_target.position.x, this.transform.position.y, go_target.position.z);
            Vector3 gunBodyTargetPostition = new Vector3(go_target.position.x, go_target.position.y, go_target.position.z);

            go_baseRotation.transform.LookAt(baseTargetPostition);
            go_GunBody.transform.LookAt(gunBodyTargetPostition);

            currentFireRate += Time.deltaTime;

            if ((currentFireRate > firerate))
            {
                if (enemy)
                {
                    enemy.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                }
                
            }
            

            // start particle system 
            if (!muzzelFlash.isPlaying)
            {
                muzzelFlash.Play();
                audioSource.Play();
            }
        }
        else
        {
            // slow down barrel rotation and stop
            currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

            // stop the particle system
            if (muzzelFlash.isPlaying)
            {
                audioSource.Stop();
                muzzelFlash.Stop();
            }
        }
    }
}