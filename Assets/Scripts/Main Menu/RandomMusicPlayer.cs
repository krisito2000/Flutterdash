using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    public static RandomMusicPlayer instance;

    [Header("------- Music -------")]
    [Tooltip("Array of music clips to choose from")]
    public AudioClip[] MusicArray;
    public AudioSource audioSource; // Reference to the AudioSource component

    // Method to get a random music clip from the MusicArray
    private AudioClip GetRandomClip()
    {
        return MusicArray[Random.Range(0, MusicArray.Length)];
    }

    void Start()
    {
        instance = this;

        // Find and assign the AudioSource component
        audioSource = FindAnyObjectByType<AudioSource>();

        // Configure AudioSource settings
        audioSource.loop = false; // Disable looping
        Time.timeScale = 1.0f; // Set time scale to normal speed
        audioSource.pitch = 1.0f; // Set pitch to normal speed
        Application.runInBackground = true; // Allow the application to run in the background
    }

    // Update is called once per frame
    void Update()
    {
        // Check if the audio source is not playing, the game is not paused, and custom song is not selected
        if (!audioSource.isPlaying && !PauseMenu.instance.gameIsPaused && !MainMenuTransition.instance.GetCustomSong())
        {
            // Get a random music clip and play it
            audioSource.clip = GetRandomClip();
            if (Guest.instance.guest && (!System.IO.File.Exists(DatabaseManager.instance.userDataFilePath) || new System.IO.FileInfo(DatabaseManager.instance.userDataFilePath).Length == 0))
            {
                audioSource.Play();
            }
        }
    }
}
