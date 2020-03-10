using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BomberDrone : MonoBehaviour, DroneInterface
{
    public float damage = 200f;

    public float firerate = 6.0f;

    // Distance the drone can aim and fire from
    public float firingRange = 120f;

    /// <summary>
    /// bomb to shoot
    /// </summary>
    public GameObject bomb;

    public GameObject bombLauncher;

    private GameObject airDroneEnemy;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponent<SphereCollider>().radius = firingRange;

        if (gameObject.tag == "Player" || gameObject.tag == "Player_Drone")
        {
            isCaptured = true;
        }
    }


    // Detect an Enemy, aim and fire
    void OnTriggerEnter(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }
    }


    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (!isCaptured)
        {
            OnTriggerBehaviour(other);
        }

    }

    // Stop firing
    void OnTriggerExit(Collider other)
    {
        if (!isCaptured)
        {
            if (AuxiliarOperations.IsPlayer(other))
            {
                airDroneEnemy = null;
            }
        }
    }

    /// <summary>
    /// custom function to avoid code duplicity on colliders
    /// </summary>
    /// <param name="other">object collided</param>
    void OnTriggerBehaviour(Collider other)
    {
        if (AuxiliarOperations.IsPlayer(other))
        {
            if (airDroneEnemy == null)
            {
                airDroneEnemy = other.gameObject;
            }
            else
            {
                if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    airDroneEnemy = other.gameObject;
                }
            }
        }
    }

    public void Attack(GameObject enemy)
    {

        //gameObject.transform.LookAt(enemy.transform);

        if ((currentFireRate > firerate))
        {

            currentFireRate = 0;

            Bomb shootMissile = bomb.GetComponent<Bomb>();
            shootMissile.enemy = enemy;
            shootMissile.damage = damage;

            GameObject.Instantiate(bomb, bombLauncher.transform.position, bombLauncher.transform.rotation);

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
        return 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isCaptured && airDroneEnemy != null)
        {
            if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
            {
                if (!airDroneEnemy.GetComponent<CommonInterface>().isDestroyed())
                {
                    if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > 2)
                    {
                        gameObject.GetComponent<NavMeshAgent>().destination = airDroneEnemy.transform.position;
                    } else {
                        Attack(airDroneEnemy);
                    }                    
                }
            }
            else
            {
                airDroneEnemy = null;
            }
        }

        currentFireRate += Time.deltaTime;
    }
}

