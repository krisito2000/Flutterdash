using B83.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DragAndDropReciver : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Renderer target")]
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnDroppedFiles;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        UnityDragAndDropHook.UninstallHook();
    }
    // Function to handle dropped music files
    private void OnDroppedFiles(List<string> aPathNames, POINT aDropPoint)
    {
        string musicFileName = null;
        // Loop through the dropped files
        foreach (string receivedFile in aPathNames)
        {
            FileInfo fileInfo = new FileInfo(receivedFile);
            string ext = fileInfo.Extension.ToLower();

            // Check if the file extension is for a supported music format (e.g., mp3, wav)
            if (ext == ".mp3" || ext == ".wav" || ext == ".webm")
            {
                musicFileName = fileInfo.Name; // Store the file name
                break; // Found a valid music file, stop checking other files
            }
        }

        // Load the music file if a valid one was found
        if (!string.IsNullOrEmpty(musicFileName))
        {
            string path = Path.Combine(Path.Combine(Application.dataPath, "Sound"), "music");
            LoadMusic(path);
        }
    }

    public void LoadMusic(string file)
    {
        // Load and play the music file
        StartCoroutine(LoadAudio(file));
    }
    private IEnumerator LoadAudio(string filePath)
    {
        var www = new WWW("file://" + filePath); // Load the file using WWW
        yield return www;

        if (www.error != null)
        {
            Debug.LogError("Error loading audio: " + www.error);
        }
        else
        {
            audioSource.clip = www.GetAudioClip(false, false); // Set the audio clip to AudioSource
            audioSource.Play(); // Play the loaded audio
        }
    }
}
