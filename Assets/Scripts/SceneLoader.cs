using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;
    public AsyncOperation operation;

    public GameObject LoadingScreen;

    void Start()
    {
        instance = this;
    }

    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneCoroutine(sceneID));
    }

    IEnumerator LoadSceneCoroutine(int sceneID)
    {
        LoadingScreen.SetActive(true);

        operation = SceneManager.LoadSceneAsync(sceneID);

        // Wait for the scene to finish loading
        while (!operation.isDone)
        {
            yield return null;
        }

        // Scene loading is done, deactivate the loading screen
        LoadingScreen.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            // Set the instance to this GameManager if it's the first one.
            instance = this;
        }
        else
        {
            // Destroy the existing instance if a new one is detected.
            Destroy(instance.gameObject);
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);
    }
}
