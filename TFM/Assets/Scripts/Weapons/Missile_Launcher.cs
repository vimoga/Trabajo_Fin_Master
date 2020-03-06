using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile_Launcher : MonoBehaviour
{

    public Missile missile;

    public float timeBetweenShoots = 1f;

    public GameObject enemy;

    public float speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GameObject.Instantiate(missile, gameObject.transform.position, gameObject.transform.rotation);
    }
}
