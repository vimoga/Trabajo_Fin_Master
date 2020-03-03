using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{

    public bool isEnemy;

    public float speed = 5f;

    public Transform target;

    private Rigidbody rb;

    private bool isDestroyed =  false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();

        if (isEnemy) {
            target = GameObject.FindWithTag("Player").transform;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDestroyed) {
            transform.LookAt(target);

            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            transform.position = Vector3.MoveTowards(transform.position, target.position, step);

            // Check if the position of the cube and sphere are approximately equal.
            if (Vector3.Distance(transform.position, target.position) < 0.001f)
            {
                //add explosion effect
                foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
                {
                    if (obj.name.Equals("Explosion"))
                    {
                        obj.SetActive(true);
                    }
                    else {
                        obj.SetActive(false);
                    }
                }
                
            }
        }
    }
}
