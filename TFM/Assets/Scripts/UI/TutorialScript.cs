using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviour of the tutorial point os the scenary
/// </summary>
public class TutorialScript : MonoBehaviour
{
    /// <summary>
    /// Text that is showed in the tutorial
    /// </summary>
    public string text;

    /// <summary>
    /// image name on the texture folder that is showed in the tutorial
    /// </summary>
    public string image;

    private Text tutorialText;

    private RawImage tutorialImage;

    private float currentTutorialTime = 0;


    // Start is called before the first frame update
    void Start()
    {
        UpdateTutorial();
    }

    /// <summary>
    /// Reloads the tutorial variables and show it
    /// </summary>
    public void UpdateTutorial()
    {
        currentTutorialTime = 0;
        gameObject.SetActive(true);
        tutorialText = GetComponentInChildren<Text>();
        tutorialText.text = text;
        tutorialImage = GetComponentInChildren<RawImage>();
        tutorialImage.texture = (Texture)Resources.Load("Textures/" + image);
    }

    // Update is called once per frame
    void Update()
    {
        currentTutorialTime += Time.deltaTime;

        if ((currentTutorialTime > GameConstants.TUTORIAL_TIME))
        {
            currentTutorialTime = 0;

            gameObject.SetActive(false);
        }
    }
}
