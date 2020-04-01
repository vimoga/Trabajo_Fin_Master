using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Implements functions that are used multiple times arround the proyect,
/// whit the objective of remove duplicated code.
/// </summary>
public static class AuxiliarOperations
{
    /// <summary>
    /// Check if the game object is destroyed or will be destroyed
    /// </summary>
    /// <param name="gameObject">object to check</param>
    /// <returns></returns>
    public static bool IsDestroyed(this GameObject gameObject)
    {
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

        if (gameObject.GetComponent<BasicStructure>() != null)
        {
            return gameObject.GetComponent<BasicStructure>().life <= 0;
        }

        return false;
    }

    /// <summary>
    /// Check if the game object is captured by the user
    /// </summary>
    /// <param name="gameObject">object to check</param>
    public static bool IsCaptured(this GameObject gameObject)
    {
       
        if (gameObject.GetComponent<BasicDrone>() != null)
        {
            return gameObject.GetComponent<BasicDrone>().isCaptured;
        }

        if (gameObject.GetComponent<BasicStructure>() != null)
        {
            return gameObject.GetComponent<BasicStructure>().isCaptured;
        }


        return false;
    }

    /// <summary>
    /// Check if the game object is propierty of the player
    /// </summary>
    /// <param name="gameObject">object to check</param>
    public static bool IsPlayer(this Collider other)
    {
        return (other.gameObject.tag == "Player" || other.gameObject.tag == "Player_Drone" || other.gameObject.tag == "Player_Structure") && !other.isTrigger;
    }


    /// <summary>
    /// Check if the game object is enemy of the player
    /// </summary>
    /// <param name="gameObject">object to check</param>
    public static bool IsEnemy(this Collider other)
    {
        return (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Enemy_Structure") && !other.isTrigger;
    }

    /// <summary>
    /// Get the provided alies of the provided tag
    /// </summary>
    /// <param name="tag">tag to check</param>
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

    /// <summary>
    /// Check if the tag is a element of the game
    /// </summary>
    /// <param name="tag">tag to check</param>
    public static bool IsPlayableObject(string tag)
    {
        return tag == "Enemy" || tag == "Enemy_Structure" || tag == "Player_Drone" || tag == "Player_Structure" || tag == "Player";
    }

    /// <summary>
    /// Check if the provided tag is from an allied element
    /// </summary>
    /// <param name="tag">tag to check</param>
    public static bool IsCaptured(string tag)
    {
        bool captured = false;
        switch (tag)
        {
            case "Enemy":
                captured = false;
                break;
            case "Enemy_Structure":
                captured = false;
                break;
            case "Player":
                captured = true;
                break;
            case "Player_Drone":
                captured = true;
                break;
            case "Player_Structure":
                captured = true;
                break;

        }

        return captured;
    }

    /// <summary>
    /// Check if the enemy game object is an aerial object
    /// </summary>
    /// <param name="player">gameobject of the player</param>
    /// <param name="enemy">game object of the enemy to check</param>
    /// <returns></returns>
    public static bool EnemyIsAerial(GameObject player, GameObject enemy)
    {
        //correguir no sirve en escenario real
        //return (enemy.transform.position.y - player.transform.position.y) >= GameConstants.SEPARATION_TERRAIN_AERIAL;
        return enemy.transform.position.y >= GameConstants.SEPARATION_TERRAIN_AERIAL;
    }

    /// <summary>
    /// Check if the enemy game object is a ground object
    /// </summary>
    /// <param name="player">gameobject of the player</param>
    /// <param name="enemy">gameobject of the enemy to check</param>
    /// <returns></returns>
    public static bool EnemyIsOnTerrain(GameObject player, GameObject enemy)
    {
        return (enemy.transform.position.y - player.transform.position.y) <= GameConstants.SEPARATION_TERRAIN_AERIAL;
    }

    /// <summary>
    /// Get an element from heriarchy
    /// </summary>
    /// <param name="parent">heriarchy to check</param>
    /// <param name="_tag">tag to find</param>
    /// <returns></returns>
    public static GameObject GetChildObject(Transform parent, string _tag)
    {
        GameObject find = null;
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.tag == _tag)
            {
                find = child.gameObject;
            }
            if (child.childCount > 0)
            {
                find = GetChildObject(child, _tag);
            }
        }

        return find;
    }
}
