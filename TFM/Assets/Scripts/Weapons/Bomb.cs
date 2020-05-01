using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements the behaviour of the bombs droped by drones
/// </summary>
public class Bomb : MonoBehaviour
{

    /// <summary>
    /// objective of the bomb
    /// </summary>
    public GameObject enemy;

    /// <summary>
    /// damage of the bomb
    /// </summary>
    public float damage = 200f;

    private AudioSource audioSource;

    private Transform target;

    private Rigidbody rb;

    private bool isExploted = false;

    private ArrayList affectedObjectives = new ArrayList();

    private string targetTag;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        target = enemy.transform;

        targetTag = enemy.tag;

        audioSource = GetComponent<AudioSource>();       
    }

    void OnCollisionEnter(Collision collision)
    {
        //if collides with terrain or enemy explotes
        if (collision.gameObject.tag.Equals(targetTag) || collision.gameObject.tag.Equals("Terrain"))
        {
            Explode();
        }
    }

    void OnTriggerStay(Collider other)
    {
        //if enemy if on the damage zone recieves damage (but only one time)
        if (isExploted)
        {
            if (!affectedObjectives.Contains(other.transform.position)) {
                if (other.tag.Equals(targetTag) || other.tag.Equals(AuxiliarOperations.GetAllies(targetTag)))
                {
                    if (!AuxiliarOperations.EnemyIsAerial(other.transform.gameObject))
                    {
                        other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                        affectedObjectives.Add(other.transform.position);
                    }
                    
                }
            }               
        }
    }

    /// <summary>
    /// Generate the effects of the bomb explosion
    /// </summary>
    private void Explode()
    {

        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Equals("Explosion"))
            {
                isExploted = true;         
                child.gameObject.SetActive(true);
                audioSource.Stop();
                Object.Destroy(gameObject, 1f);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
