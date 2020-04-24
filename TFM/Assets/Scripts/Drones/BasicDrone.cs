using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Basic and shared behaviour of the drones
/// </summary>
public class BasicDrone : MonoBehaviour, CommonInterface
{
    /// <summary>
    /// Name of the drone
    /// </summary>
    public string name;

    /// <summary>
    /// Description of the drone
    /// </summary>
    public string description;

    /// <summary>
    /// Life of the drone
    /// </summary>
    public float life = 100;

    /// <summary>
    /// Max life of the drone
    /// </summary>
    public float maxHeath;

    /// <summary>
    /// Cost of capture the drone
    /// </summary>
    public float captureCost = 1f;

    /// <summary>
    /// Current ammo of the drone
    /// </summary>
    public int ammo = -1;

    /// <summary>
    /// Max ammo of the drone
    /// </summary>
    public int maxAmmo = -1;

    /// <summary>
    /// Cost of capture the drone
    /// </summary>
    private float captureStatus = 0f;

    /// <summary>
    /// Effect played when the drones are damaged
    /// </summary>
    public GameObject smallDamage;

    /// <summary>
    /// Effect played when the drones are really damaged
    /// </summary>
    public GameObject greatDamage;

    /// <summary>
    /// Effect played when the drones are destroyed
    /// </summary>
    public GameObject explosion;

    /// <summary>
    /// Effect played when the drones are stuned
    /// </summary>
    public GameObject stuntDamage;

    /// <summary>
    /// Health bar of the drone, shows current healt
    /// </summary>
    public SimpleHealthBar healthBar;

    /// <summary>
    /// Capture status bar of the drone, shows current capture status
    /// </summary>
    public SimpleHealthBar captureBar;

    /// <summary>
    /// Shows current ammo of the drone
    /// </summary>
    public Text ammoCount;

    /// <summary>
    /// Contains the health, capture status and ammo UI indicators
    /// </summary>
    public Canvas uiInfo;

    /// <summary>
    /// Indicathes if the dron is currently captured from the player
    /// </summary>
    public bool isCaptured = false;

    /// <summary>
    /// Drone is on cover of a radio tower
    /// </summary>
    public bool isOnCover = false;

    /// <summary>
    /// waypoints for the patrol route
    /// </summary>
    public Transform[] wayPoints;

    private Rigidbody rb;

    private AudioSource audioSource;

    private bool isDestroyed = false;  

    private bool isStuned = false;

    private float droneSpeed;

    private GameplayManager gameplayManager;

    [HideInInspector]
    public DroneState currentState = DroneState.PATROL;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        audioSource = GetComponent<AudioSource>();

        droneSpeed = GetComponent<NavMeshAgent>().speed;

        if (maxHeath == 0)
        {
            maxHeath = life;
        }
        else {
            healthBar.UpdateBar(life, maxHeath);
        }

        SetAmmoCount();

        isCaptured = AuxiliarOperations.IsCaptured(gameObject.tag);
        gameplayManager = GameObject.FindGameObjectWithTag("GameplayManager").GetComponent<GameplayManager>();
    }

    /// <summary>
    /// The drone recieves an impact and rest the damage to the health
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Drone hitted: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    /// <summary>
    /// The drone recieves health
    /// </summary>
    public void Heal(float heal)
    {
        life += heal;
        Debug.Log("Drone healed: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    /// <summary>
    /// The drone recieves stunt damage
    /// </summary>
    public void StuntIn()
    {
        isStuned = true;
        stuntDamage.SetActive(true);
        GetComponent<NavMeshAgent>().speed = 1;
    }

    /// <summary>
    /// The drone is no longer recives stunt damage
    /// </summary>
    public void StuntOut()
    {
        isStuned = false;
        stuntDamage.SetActive(false);
        GetComponent<NavMeshAgent>().speed = droneSpeed;
    }

    /// <summary>
    /// The drone recieves stunt damage
    /// </summary>
    public void InCover()
    {        
        if (!isOnCover)
        {
            OutCover();
        }
        isOnCover = true;
        stuntDamage.SetActive(false);
        GetComponent<NavMeshAgent>().speed = droneSpeed;
    }

    /// <summary>
    /// The drone is no longer recives stunt damage
    /// </summary>
    public void OutCover()
    {
        isOnCover = false;
        stuntDamage.SetActive(true);
        GetComponent<NavMeshAgent>().speed = 0.5f;
    }

    /// <summary>
    /// The drone recieves more ammo and is added to the current value
    /// </summary>
    public void AmmoIn(int ammo)
    {
        this.ammo += ammo;
        Debug.Log("Drone get ammo: " + this.ammo);
        SetAmmoCount();
    }

    /// <summary>
    /// The drone shots is guns and is rested from the current ammo level
    /// </summary>
    public void AmmoOut()
    {
        this.ammo -= 1;
        Debug.Log("Drone loss ammo: " + this.ammo);
        SetAmmoCount();
    }

    /// <summary>
    /// The drone is captured from the player to take control
    /// </summary>
    public void Capture() {
        if (gameplayManager.currentCPUPower>= captureCost)
        {
            captureStatus += (captureCost) * 0.5f;
            captureBar.UpdateBar(captureStatus, GameConstants.CAPTURE_LIMIT);
            if (captureStatus >= GameConstants.CAPTURE_LIMIT)
            {
                gameObject.tag = "Player_Drone";
                isCaptured = true;
                GetComponent<NavMeshAgent>().destination = gameObject.transform.position;
                Debug.Log("Drone captured: " + captureStatus);

                //add to the hud
                gameplayManager.RemoveCPUPower(captureCost);
                gameplayManager.AddPlayerDrone(this);

                //chage selection if active
                GameObject selection = AuxiliarOperations.GetChildObject(gameObject.transform, "Selection");
                if (selection)
                {
                    if (selection.activeSelf)
                    {
                        selection.GetComponent<RawImage>().texture = (Texture)Resources.Load("Textures/selection_friend");
                    }
                }
            }
        }
        
    }


    /// <summary>
    /// The current ammo level is updated on the UI
    /// </summary>
    private void SetAmmoCount() {
        if (isCaptured)
        {
            if (maxAmmo != -1)
            {
                ammoCount.text = ammo.ToString();
            }
        }
        else {
            ammoCount.text = "";
        }        
    }

    public void destroyDrone() {
        life = 0;
    }

    bool CommonInterface.isDestroyed()
    {
        return isDestroyed;
    }

    bool CommonInterface.isCaptured()
    {
        return isCaptured;
    }

    public void GoToAttackState()
    {
        currentState = DroneState.ATTACK;
        Debug.Log("Drone state: " + DroneState.ATTACK);
    }

    public void GoToAlertState()
    {
        currentState = DroneState.ALERT;
        Debug.Log("Drone state: " + DroneState.ALERT);
    }

    public void GoToPatrolState()
    {
        currentState = DroneState.PATROL;
        Debug.Log("Drone state: " + DroneState.PATROL);
    }

    public void GoToCapturedState()
    {
        currentState = DroneState.CAPTURED;
        Debug.Log("Drone state: " + DroneState.CAPTURED);
    }


    // Update is called once per frame
    void Update()
    {
        if (!isDestroyed)
        {
            if (life > 0)
            {
                if (life <= (maxHeath / 2))
                {
                    if (life <= (maxHeath / 3))
                    {
                            greatDamage.SetActive(true);
                            smallDamage.SetActive(false);
                    }
                    else
                    {                       
                            smallDamage.SetActive(true);
                            greatDamage.SetActive(false);
                    }
                }
                else
                {
                    smallDamage.SetActive(false);
                    greatDamage.SetActive(false);
                }

                //is out cover 
                if (isCaptured) {
                    if (!isOnCover)
                    {
                        Impact(0.1f);
                    }
                }                             
            }
            else
            {
                explosion.SetActive(true);
                isDestroyed = true;
                isCaptured = true;
                audioSource.Stop();
                rb.useGravity = true;
                rb.isKinematic = false;
                if (GetComponent<NavMeshAgent>()) {
                    GetComponent<NavMeshAgent>().enabled = false;
                }
                //removes from the hud
                gameplayManager.AddCPUPower(captureCost);
                gameplayManager.RemovePlayerDrone(this);
                Object.Destroy(gameObject, 2.0f);
            }
        }

    }

    /// <summary>
    /// Updates the user interface position to face it to the main camera 
    /// </summary>
    void LateUpdate()
    {
        uiInfo.transform.LookAt(Camera.main.transform);
        uiInfo.transform.Rotate(new Vector3(0,180,0));
    }


    }
