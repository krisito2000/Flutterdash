using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    void Start()
    {
        instance = this;
    }

    public void LoadScene(int sceneID)
    {
        SceneManager.LoadScene(sceneID);
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
