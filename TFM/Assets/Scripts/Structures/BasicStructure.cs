using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public SimpleHealthBar healthBar;

    public SimpleHealthBar captureBar;

    public Canvas uiInfo;

    public bool isCapturable = false;

    public bool isCaptured = false;

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

    void LateUpdate()
    {
        uiInfo.transform.LookAt(Camera.main.transform);
        uiInfo.transform.Rotate(new Vector3(0, 180, 0));
    }


}
