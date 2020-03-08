using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Basic behaviour of the drones
/// </summary>
public class BasicDrone : MonoBehaviour, CommonInterface
{

    /// <summary>
    /// life of the structure
    /// </summary>
    public float life = 100;

    /// <summary>
    /// effect played when the drones are damaged
    /// </summary>
    public GameObject smallDamage;

    /// <summary>
    /// effect played when the drones are really damaged
    /// </summary>
    public GameObject greatDamage;

    /// <summary>
    /// effect played when the drones are destroyed
    /// </summary>
    public GameObject explosion;

    private Rigidbody rb;

    private AudioSource audioSource;

    private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Obtenemos el disparo y restamos vida
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Drone hitted: " + life);
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

                if (life <= (life/2))
                {

                    if (life <= (life/3))
                    {
                        if (!greatDamage.activeSelf)
                        {
                            greatDamage.SetActive(true);
                            smallDamage.SetActive(false);
                        }
                    }
                    else
                    {
                        if (!smallDamage.activeSelf)
                        {
                            smallDamage.SetActive(true);
                            greatDamage.SetActive(false);
                        }
                    }
                }
                else
                {
                    smallDamage.SetActive(false);
                }
            }
            else
            {
                explosion.SetActive(true);
                isDestroyed = true;
                audioSource.Stop();
                rb.useGravity = true;
                Object.Destroy(gameObject, 2.0f);
            }
        }
        
    }

    
}
