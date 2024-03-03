using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    [Tooltip("Async operation for loading scenes.")]
    public AsyncOperation operation;

    [Tooltip("Reference to the loading screen")]
    public GameObject LoadingScreen;


    void Start()
    {
        instance = this; 
    }

    // Method to load a scene by its index
    public void LoadScene(int sceneID)
    {
        StartCoroutine(LoadSceneCoroutine(sceneID)); // Start the coroutine to load the scene
    }

    // Coroutine to load a scene asynchronously
    IEnumerator LoadSceneCoroutine(int sceneID)
    {
        LoadingScreen.SetActive(true); // Activate the loading screen UI

        operation = SceneManager.LoadSceneAsync(sceneID); // Start loading the scene asynchronously

        // Wait for the scene to finish loading
        while (!operation.isDone)
        {
            yield return null; // Wait for the next frame
        }

        // Scene loading is done, deactivate the loading screen
        LoadingScreen.SetActive(false);
    }

    // Method to quit the application
    public void QuitGame()
    {
        Application.Quit(); // Quit the application
    }

    void Awake()
    {
        // Ensure there is only one instance of the SceneLoader script in the scene.
        if (instance == null)
        {
            // Set the instance to this SceneLoader if it's the first one.
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
