using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Basic and shared behaviour of the structures
/// </summary>
public class BasicStructure : MonoBehaviour,CommonInterface
{

    /// <summary>
    /// name of the structure
    /// </summary>
    public string name;

    /// <summary>
    /// description of the structure
    /// </summary>
    public string description;

    /// <summary>
    /// life of the structure
    /// </summary>
    public float life = 100;

    /// <summary>
    /// max life of the structure
    /// </summary>
    public float maxHeath;

    /// <summary>
    /// cost of capture the structure
    /// </summary>
    public float captureCost = 1f;

    /// <summary>
    /// cost of capture the structure
    /// </summary>
    private float captureStatus = 0f;

    /// <summary>
    /// effect played when the structures are damaged
    /// </summary>
    public GameObject smallDamage;

    /// <summary>
    /// effect played when the structures are really damaged
    /// </summary>
    public GameObject greatDamage;

    /// <summary>
    /// effect played when the structures are destroyed
    /// </summary>
    public GameObject explosion;

    /// <summary>
    /// Health bar of the structures, shows current healt
    /// </summary>
    public SimpleHealthBar healthBar;

    /// <summary>
    /// Capture status bar of the structures, shows current capture status
    /// </summary>
    public SimpleHealthBar captureBar;

    /// <summary>
    /// Contains the health, capture status and ammo UI indicators
    /// </summary>
    public Canvas uiInfo;

    /// <summary>
    /// Enables the possibility of cature the structure from the player
    /// </summary>
    public bool isCapturable = false;

    /// <summary>
    /// Indicathes if the structures is currently captured from the player
    /// </summary
    public bool isCaptured = false;

    /// <summary>
    /// Indicates if the unit is aerial
    /// </summary>
    public bool isAerialUnit = false;

    private bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        if (maxHeath == 0)
        {
            maxHeath = life;
        }
        else
        {
            healthBar.UpdateBar(life, maxHeath);
        }

        isCaptured = AuxiliarOperations.IsCaptured(gameObject.tag);

        isCaptured = AuxiliarOperations.IsCaptured(gameObject.tag);
    }

    /// <summary>
    /// The structure recieves an impact and rest the damage to the health
    /// </summary>
    public void Impact(float damage)
    {
        life -= damage;
        Debug.Log("Drone hitted: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    /// <summary>
    /// The structure recieves health
    /// </summary>
    public void Heal(float heal)
    {
        life += heal;
        Debug.Log("Drone healed: " + life);
        healthBar.UpdateBar(life, maxHeath);
    }

    /// <summary>
    /// The structure is captured from the player to take control
    /// </summary>
    public void Capture()
    {
        if (isCapturable)
        {
            captureStatus += (captureCost) * 0.5f;
            captureBar.UpdateBar(captureStatus, GameConstants.CAPTURE_LIMIT);
            if (captureStatus >= GameConstants.CAPTURE_LIMIT)
            {
                gameObject.tag = "Player_Structure";
                isCaptured = true;
                Debug.Log("Player_Structure captured: ");
            }
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

    public bool isAerial()
    {
        return isAerialUnit;
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
                }
            }
            else
            {
                explosion.SetActive(true);
                isDestroyed = true;
                isCaptured = true;
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
        uiInfo.transform.Rotate(new Vector3(0, 180, 0));
    }


}
