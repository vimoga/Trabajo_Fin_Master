using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects if the player has reaches a new tutorial point
/// </summary>
public class Tutorial : MonoBehaviour
{
    /// <summary>
    /// Text that is showed in the tutorial
    /// </summary>
    public string text;

    /// <summary>
    /// image name on the texture folder that is showed in the tutorial
    /// </summary>
    public string image;

    /// <summary>
    /// Tutorial to show
    /// </summary>
    public TutorialScript tutorial;

    private bool isShowed = false;

    void OnTriggerEnter(Collider col)
    {
        if (!isShowed) {
            if (col.gameObject.tag.Equals("Player"))
            {
                tutorial.text = text;
                tutorial.image = image;
                tutorial.UpdateTutorial();
                isShowed = true;
            }
        }      
    }

    
}
