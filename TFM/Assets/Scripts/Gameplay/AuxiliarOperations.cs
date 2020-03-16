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
        if (gameObject == null && !ReferenceEquals(gameObject, null)) {

            return true;
        }
        if (gameObject == null)
        {

            return true;
        }

        if (gameObject.GetComponent<BasicDrone>() != null)
        {
            return gameObject.GetComponent<BasicDrone>().life <= 0;
        }
        
        return false;
    }

    public static bool IsCaptured(this GameObject gameObject)
    {
       
        if (gameObject.GetComponent<BasicDrone>() != null)
        {
            return gameObject.GetComponent<BasicDrone>().isCaptured;
        }
        return false;
    }


    public static bool IsPlayer(this Collider other)
    {
        return (other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone" || other.gameObject.tag == "Player_Structure") && !other.isTrigger;
    }

    public static bool IsEnemy(this Collider other)
    {
        return (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Enemy_Structure") && !other.isTrigger;
    }

    public static string GetAllies(string tag)
    {
        string allies = "";
        switch (tag)
        {
            case "Enemy":
                allies = "Enemy_Structure";
                break;
            case "Enemy_Structure":
                allies = "Enemy";
                break;
            case "Player_Drone":
                allies = "Player_Structure";
                break;
            case "Player_Structure":
                allies = "Player_Drone";
                break;

        }

        return allies;
    }

    public static bool EnemyIsAerial(GameObject player, GameObject enemy)
    {
        return (enemy.transform.position.y - player.transform.position.y) >= GameConstants.SEPARATION_TERRAIN_AERIAL;
    }
}
