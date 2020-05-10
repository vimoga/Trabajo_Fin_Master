using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWar : MonoBehaviour
{ 
    public void activateRadar(int radarNumber,Vector3 position, float fogOfWarCover)
    {
        GetComponent<Renderer>().material.SetVector("_Radar" + radarNumber + "_Pos", position);
        GetComponent<Renderer>().material.SetFloat("_FogRadius" + radarNumber, fogOfWarCover);
    }

    public void clearRadars()
    {
        for (int x = 1; x<17;x++)
        {
            GetComponent<Renderer>().material.SetVector("_Radar" + x + "_Pos", new Vector3(0,0,0));
            GetComponent<Renderer>().material.SetFloat("_FogRadius" + x, 0);
        }

    }
    
}
