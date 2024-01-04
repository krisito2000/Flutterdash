using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public AsyncOperation operation;

    void Start()
    {
        instance = this;
    }

    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneAsync(sceneID));
    }

    IEnumerator LoadSceneAsync(int sceneID)
    {
        operation = SceneManager.LoadSceneAsync(sceneID);
        LoadingScreen.instance.LoadingScreenCanvas.SetActive(true);

        while (!operation.isDone)
        {
            yield return null;
        }

        LoadingScreen.instance.LoadingScreenCanvas.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
