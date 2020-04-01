using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sets a new missile at the time indicated
/// </summary>
public class Missile_Launcher : MonoBehaviour
{
    /// <summary>
    /// Missile to instanciate
    /// </summary>
    public Missile missile;

    /// <summary>
    /// Time between misisile shotos
    /// </summary>
    public float timeBetweenShoots = 1f;

    /// <summary>
    /// Enemy of the missile
    /// </summary>
    public GameObject enemy;

    /// <summary>
    /// Speed of the missile
    /// </summary>
    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!AuxiliarOperations.IsDestroyed(enemy))
        {
            GameObject.Instantiate(missile, gameObject.transform.position, gameObject.transform.rotation);
        }        
    }
}
