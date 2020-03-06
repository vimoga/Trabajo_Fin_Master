using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class AA_Gun : MonoBehaviour
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
    private void Shoot()
    {
        currentFireRate = 0;

        Missile shootMissile = missile.GetComponent<Missile>();
        shootMissile.enemy = enemy;
        shootMissile.speed = missileSpeed;
        shootMissile.damage = missileDamage;

        GameObject.Instantiate(missile, barrel.transform.position, barrel.transform.rotation);
    }

    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player") {
            enemy = col.gameObject;
        }

    }

    public void OnTriggerStay(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            enemy = col.gameObject;
        }
    }

    public void OnTriggerExit(Collider col)
    {
        enemy = null;
    }

    // Update is called once per frame
    void Update()
    {
        
        if (life > 0)
        {

            if (enemy) { 
                currentFireRate += Time.deltaTime;

                if ((currentFireRate > timeBetweenShoots))
                {
                    Shoot();
                }
            }
  
        }
    }
}
