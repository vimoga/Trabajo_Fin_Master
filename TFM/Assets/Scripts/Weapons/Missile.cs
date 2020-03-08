using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Behaviour of the missiles
/// </summary>
public class Missile : MonoBehaviour
{

    /// <summary>
    /// objective of the missile
    /// </summary>
    public GameObject enemy;

    /// <summary>
    /// speed of the missile
    /// </summary>
    public float speed = 5f;

    /// <summary>
    /// damage of the missile
    /// </summary>
    public float damage = 25f;

    /// <summary>
    /// explosion sound
    /// </summary>
    public AudioClip explosionSound;

    private AudioSource audioSource;

    private Transform target;

    private Rigidbody rb;

    private bool isDestroyed =  false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        target = enemy.transform;

        audioSource = GetComponent<AudioSource>();
    }

    public void CollisionDetected(GameObject childInpact)
    {
        //si colisiona con un enemigo se le resta a la vida la fuerza del inpacto
        if (childInpact.tag == "Player" || childInpact.tag == "Player_Drone" )
        {
            childInpact.SendMessage("Impact", damage, SendMessageOptions.RequireReceiver);
        }
    }

    //add explosion effect
    private void Explode()
    {
        
        foreach (Transform child in transform)
        {
            if (child.gameObject.name.Equals("Explosion"))
            {
                child.gameObject.SetActive(true);
                isDestroyed = true;
                audioSource.Stop();
                audioSource.PlayOneShot(explosionSound, 1);
                Object.Destroy(gameObject, 2.0f);
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDestroyed) {

            if (!AuxiliarOperations.IsDestroyed(enemy))
            {
                transform.LookAt(target);

                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                transform.position = Vector3.MoveTowards(transform.position, target.position, step);

                // Check if the position of the cube and sphere are approximately equal.
                if (Vector3.Distance(transform.position, target.position) < 0.001f)
                {
                    Explode();
                }
            }
            else {
                Explode();
            }
        }
    }
}
