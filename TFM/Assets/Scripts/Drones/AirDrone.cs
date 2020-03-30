using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour for the Air Drone
/// </summary>
public class AirDrone : MonoBehaviour, DroneInterface
{
    /// <summary>
    /// Damage of the missiles
    /// </summary>
    public float damage = 25f;

    /// <summary>
    /// Time between the missiles
    /// </summary>
    public float firerate = 1.0f;

    /// <summary>
    /// Distance the drone can fire from
    /// </summary> 
    public float firingRange = 1.5f;

    /// <summary>
    /// Missile to shoot
    /// </summary>
    public GameObject missile;

    /// <summary>
    /// Speed of the missiles
    /// </summary>
    public float missileSpeed = 10f;

    /// <summary>
    /// Gameobject from the missile is shoot
    /// </summary>
    public GameObject missileLauncher;

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
                if (Vector3.Distance(airDroneEnemy.transform.position, gameObject.transform.position) > Vector3.Distance(other.transform.position, gameObject.transform.position))
                {
                    airDroneEnemy = other.gameObject;
                }
            }
        }
    }

    /// <summary>
    /// Custom Attack function of the air drone
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
            if (gameObject.GetComponent<BasicDrone>().maxAmmo == GameConstants.INFINITE_AMMO)
            {
                MakeAttack(enemy);
            }
            else {
                if (gameObject.GetComponent<BasicDrone>().ammo >0) {
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
        if ((currentFireRate > firerate))
        {
            currentFireRate = 0;

            //generate new missile instance
            Missile shootMissile = missile.GetComponent<Missile>();
            shootMissile.enemy = enemy;
            shootMissile.speed = missileSpeed;
            shootMissile.damage = damage;

            GameObject.Instantiate(missile, missileLauncher.transform.position, missileLauncher.transform.rotation);

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
        return firingRange*10f;
    }

    // Update is called once per frame
    void Update()
    {
        //attack player drones when is not captured
        if (!isCaptured && airDroneEnemy != null)
        {
            if (!AuxiliarOperations.IsDestroyed(airDroneEnemy))
            {
                if (!airDroneEnemy.GetComponent<CommonInterface>().isDestroyed())
                {
                    Attack(airDroneEnemy);
                }
            }
            else
            {
                airDroneEnemy = null;
            }
        }

        isCaptured = GetComponent<BasicDrone>().isCaptured;

        currentFireRate += Time.deltaTime;
    }
}
