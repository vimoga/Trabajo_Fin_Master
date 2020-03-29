using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

/// <summary>
/// Basic behaviour of the drones
/// </summary>
public class BasicDrone : MonoBehaviour, CommonInterface
{
    /// <summary>
    /// name of the drone
    /// </summary>
    public string name;

    /// <summary>
    /// description of the drone
    /// </summary>
    public string description;

    /// <summary>
    /// life of the drone
    /// </summary>
    public float life = 100;

    /// <summary>
    /// max life of the drone
    /// </summary>
    public float maxHeath;

    /// <summary>
    /// cost of capture the drone
    /// </summary>
    public float captureCost = 1f;

    /// <summary>
    /// current ammo of the drone
    /// </summary>
    public int ammo = -1;

    /// <summary>
    /// max ammo of the drone
    /// </summary>
    public int maxAmmo = -1;

    /// <summary>
    /// cost of capture the drone
    /// </summary>
    private float captureStatus = 0f;

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

    /// <summary>
    /// effect played when the drones are stuned
    /// </summary>
    public GameObject stuntDamage;

    public SimpleHealthBar healthBar;

    public SimpleHealthBar captureBar;

    public Text ammoCount;

    public Canvas uiInfo;

    private Rigidbody rb;

    private AudioSource audioSource;

    private bool isDestroyed = false;

    public bool isCaptured = false;

    private bool isStuned = false;

    private float droneSpeed;


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
    }

    /// <summary>
    /// Obtenemos el disparo y restamos vida
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Drone hitted: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    /// <summary>
    /// Obtenemos curacion y sumamos vida
    /// </summary>
    public void Heal(float heal)
    {
        life += heal;
        Debug.Log("Drone healed: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    public void StuntIn()
    {
        isStuned = true;
        stuntDamage.SetActive(true);
        GetComponent<NavMeshAgent>().speed = 1;
    }

    public void StuntOut()
    {
        isStuned = false;
        stuntDamage.SetActive(false);
        GetComponent<NavMeshAgent>().speed = droneSpeed;
    }

    public void AmmoIn(int ammo)
    {
        this.ammo += ammo;
        Debug.Log("Drone get ammo: " + this.ammo);
        SetAmmoCount();
    }

    public void AmmoOut()
    {
        this.ammo -= 1;
        Debug.Log("Drone loss ammo: " + this.ammo);
        SetAmmoCount();
    }

    public void Capture() {
        captureStatus +=  (captureCost)*0.5f;
        captureBar.UpdateBar(captureStatus, GameConstants.CAPTURE_LIMIT);
        if (captureStatus >= GameConstants.CAPTURE_LIMIT)
        {
            gameObject.tag = "Player_Drone";
            isCaptured = true;
            Debug.Log("Drone captured: " + captureStatus);

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

    bool CommonInterface.isDestroyed()
    {
        return isDestroyed;
    }

    bool CommonInterface.isCaptured()
    {
        return isCaptured;
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
                Object.Destroy(gameObject, 2.0f);
            }
        }

    }

    void LateUpdate()
    {
        uiInfo.transform.LookAt(Camera.main.transform);
        uiInfo.transform.Rotate(new Vector3(0,180,0));
    }


    }
