using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Behaviour for the Bomber Drone
/// </summary>
public class BomberDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Damage of the bombs
    /// </summary>
    public float damage = 200f;

    /// <summary>
    /// Time between the shoots
    /// </summary>
    public float firerate = 6.0f;

    /// <summary>
    /// Distance the drone can fire from
    /// </summary> 
    public float firingRange = 120f;

    /// <summary>
    /// Bomb to drop
    /// </summary>
    public GameObject bomb;

    /// <summary>
    /// Gameobject from the bombs are drop
    /// </summary>
    public GameObject bombLauncher;

    private GameObject airDroneEnemy;

    private float currentFireRate = 0;

    private bool isCaptured = false;

    // Start is called before the first frame update
    void Start()
    {
        // Set the firing range distance
        this.GetComponentInChildren<SphereCollider>().radius = firingRange;

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
                if ((Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position)) 
                    && !AuxiliarOperations.EnemyIsAerial(gameObject, other.transform.gameObject))
                {
                    airDroneEnemy = other.gameObject;
                }
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the bomber drone
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
            if (gameObject.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO && !AuxiliarOperations.EnemyIsAerial(gameObject, enemy))
            {
                MakeAttack(enemy);
            }
            else
            {
                if (gameObject.GetComponent<BasicDrone>().ammo > 0 && !AuxiliarOperations.EnemyIsAerial(gameObject, enemy))
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
        //gameObject.transform.LookAt(enemy.transform);

        if ((currentFireRate > firerate))
        {
            currentFireRate = 0;

            //generate new bomb instance
            Bomb bombScript = bomb.GetComponentInChildren<Bomb>();
            bombScript.enemy = enemy;
            bombScript.damage = damage;

            GameObject.Instantiate(bomb, bombLauncher.transform.position, bombLauncher.transform.rotation);

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
        return gameObject.GetComponent<NavMeshAgent>().radius+4;
    }

    // Update is called once per frame
    void Update()
    {
        //attack player drones when is not captured
        if (!isCaptured && airDroneEnemy != null)
        {
            if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
            {
                if (!airDroneEnemy.GetComponent<CommonInterface>().isDestroyed()  && !AuxiliarOperations.EnemyIsAerial(gameObject, airDroneEnemy))
                {
                    if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > (gameObject.GetComponent<NavMeshAgent>().radius+3))
                    {
                        gameObject.GetComponent<NavMeshAgent>().destination = airDroneEnemy.transform.position;
                    } else {
                        Attack(airDroneEnemy);
                        gameObject.GetComponent<NavMeshAgent>().destination = gameObject.transform.position;
                    }                    
                }
            }
            else
            {
                airDroneEnemy = null;
            }
        }

        currentFireRate += Time.deltaTime;
        isCaptured = GetComponent<BasicDrone>().isCaptured;
    }
}

