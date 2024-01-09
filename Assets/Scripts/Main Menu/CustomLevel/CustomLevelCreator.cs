using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CustomLevelCreator : MonoBehaviour
{
    public static CustomLevelCreator instance;
    private string copiedSceneName; // Define copiedSceneName at the class level


    public Text statusText1;
    public Text statusText2;

    void Start()
    {
        instance = this;
        CheckForCustomLevelsScene();
    }

    void CheckForCustomLevelsScene()
    {
        Scene customLevelsScene = SceneManager.GetSceneByName("Copied_ClearLevel");
        if (customLevelsScene.isLoaded)
        {
            // The CustomLevels scene is loaded, perform your actions here...
            GameObject musicObject = GameObject.Find("Sound/Music");
            if (musicObject != null)
            {
                AudioSource audioSource = musicObject.GetComponentInChildren<AudioSource>();
                if (audioSource != null)
                {
                    StartCoroutine(LoadAudioAndAssignToSource(DragAndDropReceiver.instance.receivedFile, audioSource));
                }
                else
                {
                    Debug.LogWarning("Could not find AudioSource component.");
                }
            }
            else
            {
                Debug.LogWarning("Could not find 'Music' GameObject.");
            }
        }
        else
        {
            Debug.Log("CustomLevels scene is not yet loaded...");
            // Retry after a certain delay or check periodically
            Invoke("CheckForCustomLevelsScene", 1f); // Retry after 1 second (adjust as needed)
        }
    }


    // Method to create a copy of the specified scene and save it in the CustomLevels folder
    public void CopyAndSaveCustomLevel()
    {
        string sceneToCopy = "ClearLevel"; // Replace with the scene name you want to copy
        copiedSceneName = "Copied_" + sceneToCopy; // Assign the value here

        string customLevelsFolderPath = Application.persistentDataPath + "/CustomLevels"; // Use persistentDataPath to save in the game's data folder

        if (!Directory.Exists(customLevelsFolderPath))
        {
            Directory.CreateDirectory(customLevelsFolderPath);
        }

        string originalScenePath = Application.dataPath + "/Scenes/Levels/" + sceneToCopy + ".unity";
        string copiedScenePath = customLevelsFolderPath + "/" + copiedSceneName + ".unity";

        if (File.Exists(originalScenePath))
        {
            File.Copy(originalScenePath, copiedScenePath, true);
        }

        if (SceneWasCopied(copiedSceneName))
        {
            LoadCopiedScene(copiedSceneName);
        }
    }

    private bool SceneWasCopied(string sceneName)
    {
        string copiedScenePath = Application.persistentDataPath + "/CustomLevels/" + sceneName + ".unity";
        return File.Exists(copiedScenePath);
    }

    // Load the scene with the given name
    private void LoadCopiedScene(string sceneName)
    {
        SceneManager.LoadScene("Scenes/Levels/CustomLevels/" + sceneName, LoadSceneMode.Single);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Loaded Scene Name: " + scene.name); // Add this line to check the loaded scene's name
        if (scene.name.Equals("Scenes/Levels/CustomLevels/" + copiedSceneName))
        {
            GameObject musicObject = GameObject.Find("Sound/Music"); // Adjust the path if needed

            if (musicObject != null)
            {
                AudioSource audioSource = musicObject.GetComponentInChildren<AudioSource>();

                if (audioSource != null)
                {
                    StartCoroutine(LoadAudioAndAssignToSource(DragAndDropReceiver.instance.receivedFile, audioSource));
                }
            }
            else
            {
                Debug.LogWarning("Could not find 'Music' GameObject or AudioSource component.");
            }
        }
        else
        {
            Debug.LogWarning("Copied scene is not loaded or does not exist. Expected: " + copiedSceneName);
        }
    }

    private IEnumerator LoadAudioAndAssignToSource(string filePath, AudioSource audioSource)
    {
        string fileUri = new Uri(filePath).AbsoluteUri;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(fileUri, AudioType.UNKNOWN))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading audio: " + www.error);
            }
            else
            {
                AudioClip audioClip = DownloadHandlerAudioClip.GetContent(www);
                if (audioClip != null)
                {
                    audioSource.clip = audioClip;
                    audioSource.Play();
                }
                else
                {
                    Debug.LogError("Audio clip is null.");
                }
            }
        }
    }
    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy this instance if there is already another one in the scene.
            Destroy(gameObject);
            return;
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);
    }
}
