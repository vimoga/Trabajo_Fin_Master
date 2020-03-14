using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{

    /// <summary>
    /// objective of the missile
    /// </summary>
    public GameObject enemy;

    /// <summary>
    /// damage of the missile
    /// </summary>
    public float damage = 200f;

    private AudioSource audioSource;

    private Transform target;

    private Rigidbody rb;

    private bool isExploted = false;

    private ArrayList affectedObjectives = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        target = enemy.transform;

        audioSource = GetComponent<AudioSource>();       
    }

    void OnCollisionEnter(Collision collision)
    {
        //si colisiona con un enemigo se le resta a la vida la fuerza del inpacto
        if (collision.gameObject.tag.Equals(enemy.tag) || collision.gameObject.tag.Equals("Terrain"))
        {
            Explode();
        }
    }

    // keep firing
    void OnTriggerStay(Collider other)
    {
        if (isExploted)
        {
            if (!affectedObjectives.Contains(other.transform.position)) {
                if (other.tag.Equals(enemy.tag) || other.tag.Equals(AuxiliarOperations.GetAllies(enemy.tag)))
                {
                    other.transform.gameObject.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
                    affectedObjectives.Add(other.transform.position);
                }
            }               
        }
    }

    private void Explode()
    {

        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Equals("Explosion"))
            {
                isExploted = true;
                child.gameObject.SetActive(true);
                audioSource.Stop();
                Object.Destroy(gameObject, 2.0f);
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
