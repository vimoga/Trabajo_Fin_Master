using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the AA Gun
/// </summary>
public class AA_Gun : MonoBehaviour, StructuresInterfaces
{

    /// <summary>
    /// Time between missiles are shoot
    /// </summary>
    public float timeBetweenShoots = 2.0f;

    /// <summary>
    /// Misile spawn point
    /// </summary>
    public GameObject barrel;

    /// <summary>
    /// missile to shoot
    /// </summary>
    public GameObject missile;

    /// <summary>
    /// Speed of the missiles
    /// </summary>
    public float missileSpeed = 5f;

    /// <summary>
    /// Damage of the missiles
    /// </summary>
    public float missileDamage = 25f;

    private float currentFireRate = 0;

    private GameObject aa_Enemy;

    private bool isDestroyed = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }


    /// <summary>
    /// Custom Attack function of the AA Gun
    /// </summary>
    public void Attack()
    {
        currentFireRate = 0;

        Missile shootMissile = missile.GetComponent<Missile>();
        shootMissile.enemy = aa_Enemy;
        shootMissile.speed = missileSpeed;
        shootMissile.damage = missileDamage;

        GameObject.Instantiate(missile, barrel.transform.position, barrel.transform.rotation);
    }

    void StructuresInterfaces.OnTriggerEnter(Collider other)
    {
        OnTriggerBehaviour(other);
    }

    void StructuresInterfaces.OnTriggerStay(Collider other)
    {
        OnTriggerBehaviour(other);
    }

    void StructuresInterfaces.OnTriggerExit(Collider other)
    {
        if (AuxiliarOperations.IsPlayerDrone(other))
        {
            aa_Enemy = null;
        }
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if (!isDestroyed)
        {
            if (AuxiliarOperations.IsPlayerDrone(other))
            {
                if (aa_Enemy == null)
                {
                    aa_Enemy = other.gameObject;
                }
                else
                {
                    if (Vector3.Distance(aa_Enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                    {
                        aa_Enemy = other.gameObject;
                    }
                }
            }
        }
        else {
            aa_Enemy = null;
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

    // Update is called once per frame
    void Update()
    {
        isDestroyed = GetComponent<CommonInterface>().isDestroyed();
        //attack player drones if are in the attack area
        if (aa_Enemy != null)
        {           
            if (!AuxiliarOperations.IsDestroyed(aa_Enemy) && AuxiliarOperations.EnemyIsAerial(aa_Enemy))
            {
                if ((currentFireRate > timeBetweenShoots))
                {
                    Attack();
                }
            } else {
                aa_Enemy = null;
            }
            
        }
        currentFireRate += Time.deltaTime;
    }
    
}
