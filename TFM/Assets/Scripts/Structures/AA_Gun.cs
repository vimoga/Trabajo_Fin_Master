using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AA_Gun : MonoBehaviour, StructuresInterfaces, CommonInterface
{

    /// <summary>
    /// life of the structure
    /// </summary>
    public float life = 100;

    /// <summary>
    /// time between shoots
    /// </summary>
    public float timeBetweenShoots = 1.0f;

    /// <summary>
    /// current fire rate
    /// </summary>
    private float currentFireRate = 0;

    /// <summary>
    /// Misile spawn point
    /// </summary>
    public GameObject barrel;

    /// <summary>
    /// missile to shoot
    /// </summary>
    public GameObject missile;

    public float missileSpeed = 5f;

    public float missileDamage = 25f;

    private GameObject enemy;

    /// <summary>
    /// effect played when the estructure are destroyed
    /// </summary>
    public GameObject explosion;

    private bool isDestroyed = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    /// <summary>
    /// Obtenemos el disparo y restamos vida
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Enemy hitted: " + life);
    }


    /// <summary>
    /// gestiona el comportamiento de un disparo del arma
    /// </summary>
    public void Attack()
    {
        currentFireRate = 0;

        Missile shootMissile = missile.GetComponent<Missile>();
        shootMissile.enemy = enemy;
        shootMissile.speed = missileSpeed;
        shootMissile.damage = missileDamage;

        GameObject.Instantiate(missile, barrel.transform.position, barrel.transform.rotation);
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
            enemy = null;
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
            if (enemy == null)
            {
                enemy = other.gameObject;

            }
            else
            {
                if (Vector3.Distance(enemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    enemy = other.gameObject;
                }
            }
        }
    }

    bool CommonInterface.isDestroyed()
    {
        return isDestroyed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDestroyed)
        {
            if (life > 0)
            {

                if (!AuxiliarOpereations.IsDestroyed(enemy))
                {
                    currentFireRate += Time.deltaTime;

                    if ((currentFireRate > timeBetweenShoots))
                    {
                        Attack();
                    }
                }
                else {
                    enemy = null;
                }
            }
            else
            {
                explosion.SetActive(true);
                Object.Destroy(gameObject, 2.0f);
                isDestroyed = true;
            }
        }
    }

    
}
