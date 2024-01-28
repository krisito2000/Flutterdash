using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMusicPlayer : MonoBehaviour
{
    [Header("------- Music -------")]
    public AudioClip[] MusicArray;
    private AudioSource audioSource;

    private AudioClip GetRandomClip()
    {
        return MusicArray[Random.Range(0, MusicArray.Length)];
    }

    void Start()
    {
        audioSource = FindAnyObjectByType<AudioSource>();
        audioSource.loop = false;
        Time.timeScale = 1.0f;
        audioSource.pitch = 1.0f;
        Application.runInBackground = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!audioSource.isPlaying && !PauseMenu.instance.gameIsPaused && !MainMenuTransition.instance.GetCustomSong())
        {
            audioSource.clip = GetRandomClip();
            audioSource.Play();
        }
    }
}
