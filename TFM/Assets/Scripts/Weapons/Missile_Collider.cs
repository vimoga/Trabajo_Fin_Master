using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Indicates to the missile when a collision is detected
/// </summary>
public class Missile_Collider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        transform.parent.GetComponent<Missile>().CollisionDetected(other.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
