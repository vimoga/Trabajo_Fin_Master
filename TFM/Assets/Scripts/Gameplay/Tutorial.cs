using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Detects if the player has reaches a new tutorial point
/// </summary>
public class Tutorial : MonoBehaviour
{
    /// <summary>
    /// HUD element of the tutorials
    /// </summary>
    public GameObject tutorialHUD;

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



    void Start()
    {
        if (GameConstants.showedTutorials.Contains(gameObject.name))
        {
            gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter(Collider col)
    {
        if (!isShowed) {
            
            if (col.gameObject.tag.Equals("Player"))
            {
                if (!tutorialHUD.activeSelf)
                {
                    tutorialHUD.SetActive(true);
                }
                tutorial.text = text;
                tutorial.image = image;
                tutorial.UpdateTutorial();
                isShowed = true;
                GameConstants.showedTutorials.Add(gameObject.name);
            }
        }      
    }

    
}
