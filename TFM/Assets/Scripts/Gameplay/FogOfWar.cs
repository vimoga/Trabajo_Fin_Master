using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class to manage the behaviour of the fog of war
/// </summary>
public class FogOfWar : MonoBehaviour
{ 
    /// <summary>
    /// activates a radar cover area, making the zone visible
    /// </summary>
    /// <param name="radarNumber">id of the radar to activate</param>
    /// <param name="position">position of the radar</param>
    /// <param name="fogOfWarCover">range of cover of the radar</param>
    public void activateRadar(int radarNumber,Vector3 position, float fogOfWarCover)
    {
        GetComponent<Renderer>().material.SetVector("_Radar" + radarNumber + "_Pos", position);
        GetComponent<Renderer>().material.SetFloat("_FogRadius" + radarNumber, fogOfWarCover);
    }

    /// <summary>
    /// Restarts the fog of wor to their original state
    /// </summary>
    public void clearRadars()
    {
        for (int x = 1; x<17;x++)
        {
            GetComponent<Renderer>().material.SetVector("_Radar" + x + "_Pos", new Vector3(0,0,0));
            GetComponent<Renderer>().material.SetFloat("_FogRadius" + x, 0);
        }

    }
    
}
