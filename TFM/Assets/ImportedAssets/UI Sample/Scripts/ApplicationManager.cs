using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {
	  
    public void NewGame()
    {
        SceneManager.LoadScene("MainLevel");
        GameConstants.currentLevel = "MainLevel";
    }

    public void Restart()
    {
        if (SceneManager.GetActiveScene().name.Equals("MainLevel")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        } else if (SceneManager.GetActiveScene().name.Equals("MainLevel2")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene(GameConstants.currentLevel);
        }
    }

    public void Quit () 
	{
		#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
		#else
		Application.Quit();
		#endif
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
