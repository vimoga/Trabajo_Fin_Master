using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AuxiliarOperations
{
    public static bool IsDestroyed(this GameObject gameObject)
    {
        // UnityEngine overloads the == opeator for the GameObject type
        // and returns null when the object has been destroyed, but 
        // actually the object is still there but has not been cleaned up yet
        // if we test both we can determine if the object has been destroyed.
        return gameObject == null && !ReferenceEquals(gameObject, null);
    }


    public static bool IsPlayer(this Collider other)
    {
        return (other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone" || other.gameObject.tag == "Player_Structure") && !other.isTrigger;
    }

    public static bool IsEnemy(this Collider other)
    {
        return (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Enemy_Structure") && !other.isTrigger;
    }
}
