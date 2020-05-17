using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    private bool isAdded = false;

    // Start is called before the first frame update
    void Start()
    {
        if (GameConstants.generatorDestroyed.Contains(gameObject.name))
        {
            GetComponent<AudioSource>().enabled = false;
            GetComponent<BasicStructure>().DestroyStructure();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<CommonInterface>().isDestroyed() && !isAdded)
        {
            isAdded = true;

            if (!GameConstants.generatorDestroyedTemp.Contains(gameObject.name))
            {
                GameConstants.generatorDestroyedTemp.Add(gameObject.name);
            }
        }
    }
}
