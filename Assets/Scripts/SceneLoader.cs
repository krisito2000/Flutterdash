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
        StartCoroutine(LoadSceneAsync(sceneID, 1.0f));
    }

    IEnumerator LoadSceneAsync(int sceneID, float delayTime)
    {
        float elapsedTime = 0;
        LoadingScreen.instance.LoadingScreenCanvas.SetActive(true);
        // Wait for the delay time
        while (elapsedTime < delayTime)
        {
            elapsedTime += 0.01f;
        }
        CanvasGroup canvasGroup = MainMenuTransition.instance.mainMenuCanvas.GetComponent<CanvasGroup>();
        canvasGroup.gameObject.SetActive(false);

        LoadingScreen.instance.DeleteOrDeactivate();

        operation = SceneManager.LoadSceneAsync(sceneID);

        while (!operation.isDone)
        {
            LoadingScreen.instance.LoadingScreenCanvas.SetActive(false);
            yield return null;
        }

        
    }


    public void QuitGame()
    {
        Application.Quit();
    }
}
