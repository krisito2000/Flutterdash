using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using B83.Win32;
using UnityEngine.SceneManagement;

public class DragAndDropReceiver : MonoBehaviour
{
    public static DragAndDropReceiver instance;
    [SerializeField]
    [Tooltip("Renderer target")]
    public AudioSource audioSource;

    public Text musicName;
    public string receivedFile = null;

    private CustomLevelCreator levelCreator = new CustomLevelCreator();

    void Start()
    {
        instance = this;

        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnDroppedFiles;
    }

    void OnDestroy()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    private void OnDroppedFiles(List<string> aPathNames, POINT aDropPoint)
    {
        string musicFileName = null;
        

        foreach (string file in aPathNames)
        {
            FileInfo fileInfo = new FileInfo(file);
            string ext = fileInfo.Extension.ToLower();

            // Check if the file extension is for a supported music format (e.g., mp3, wav)
            if (ext == ".mp3" || ext == ".wav" || ext == ".webm")
            {
                musicFileName = fileInfo.Name; // Store the file nam
                receivedFile = file;
                break; // Found a valid music file, stop checking other files
            }
        }

        // If a valid music file was found, change the text and play the music
        if (!string.IsNullOrEmpty(musicFileName))
        {
            musicName.text = musicFileName; // Set the Text component to display the file name
            LoadMusic(receivedFile);
        }
    }

    public void LoadMusic(string file)
    {
        StartCoroutine(LoadAudio(file));
    }

    private IEnumerator LoadAudio(string filePath)
    {
        // Convert local file path to URI format
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
                audioSource.clip = DownloadHandlerAudioClip.GetContent(www); // Set the audio clip to AudioSource
                audioSource.Play(); // Play the loaded audio
            }
        }
    }

    // Method to call the CopyAndSaveCustomLevel method from CustomLevelCreator
    public void CreateCustomLevel()
    {
        //levelCreator.CopyAndSaveCustomLevel();
    }
}
