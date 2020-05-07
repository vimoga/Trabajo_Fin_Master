using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the GunTurret (modified from Free Sci Fi Gatling Gun - Tower Defense Asset)
/// </summary>
public class GatlingGunCustom : MonoBehaviour, StructuresInterfaces
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
    public GameObject muzzelFlash;

    private AudioSource audioSource;

    private GameObject enemy;

    public float damage = 1f;

    public float firerate = 0.15f;

    private float currentFireRate = 0;

    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;
        audioSource = GetComponent<AudioSource>();
        audioSource.Stop();
    }

    void OnDrawGizmosSelected()
    {
        // Draw a red sphere at the transform's position to show the firing range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, firingRange);
    }

    // Detect an Enemy, aim and fire
    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        OnTriggerBehaviour(other);
    }

    // keep firing
    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        OnTriggerBehaviour(other);
    }

    
    // Stop firing
    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (AuxiliarOperations.IsPlayerDrone(other) && !AuxiliarOperations.EnemyIsAerial(other.transform.gameObject))
        {           
            go_target = null;
            enemy = null;
            CancelAttack();
        }      
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if (AuxiliarOperations.IsPlayerDrone(other) && !AuxiliarOperations.EnemyIsAerial(other.transform.gameObject))
        {
            if (go_target == null)
            {
                go_target = other.transform;
                enemy = other.gameObject;
            }
            else
            {
                if (Vector3.Distance(go_target.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    go_target = other.transform;
                    enemy = other.gameObject;
                }
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the AA Gun
    /// </summary>
    public void Attack()
    {
        // Gun barrel rotation
        go_barrel.transform.Rotate(0, 0, currentRotationSpeed * Time.deltaTime);

        // start rotation
        currentRotationSpeed = barrelRotationSpeed;

        // aim at enemy
        Vector3 baseTargetPostition = new Vector3(go_target.position.x, this.transform.position.y, go_target.position.z);
        Vector3 gunBodyTargetPostition = new Vector3(go_target.position.x, go_target.position.y, go_target.position.z);

        go_baseRotation.transform.LookAt(baseTargetPostition);
        go_GunBody.transform.LookAt(gunBodyTargetPostition);

        if ((currentFireRate > firerate))
        {
            if (enemy)
            {
                enemy.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
            }
               
            // start particle system 
            if (!muzzelFlash.activeSelf)
            {
                muzzelFlash.SetActive(true);
                audioSource.Play();
            }

            currentFireRate = 0;
        }                
    }

    /// <summary>
    /// Cancel attack effects when the enemy on no longer reachable
    /// </summary>
    private void CancelAttack()
    {
        // slow down barrel rotation and stop
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, 0, 10 * Time.deltaTime);

        // stop the particle system
        if (muzzelFlash.activeSelf)
        {
            muzzelFlash.SetActive(false);
            audioSource.Stop();
        }
    }

    public void SetCaptured(bool isCaptured)
    {
    }

    public bool GetCaptured()
    {
        return false;
    }

    public bool isCaptured()
    {
        return false;
    }

    void Update()
    {      
        if (!AuxiliarOperations.IsDestroyed(enemy))
        {
            if ((currentFireRate > firerate))
            {
                Attack();
            }
        }
        else
        {
            enemy = null;
            CancelAttack();
        }

        currentFireRate += Time.deltaTime;
    }
   
}