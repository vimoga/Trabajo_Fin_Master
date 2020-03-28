using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ApplicationManager : MonoBehaviour {
	  
    public void NewGame()
    {
        SceneManager.LoadScene("Demo");
    }

    public void Restart()
    {
        if(SceneManager.GetActiveScene().name.Equals("Demo")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            SceneManager.LoadScene("Demo");
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
