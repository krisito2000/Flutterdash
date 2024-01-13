using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    public static bool gameIsPaused = false;

    public GameObject pauseMenuCanvas;

    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Pause()
    {
        pauseMenuCanvas.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in audios)
        {
            audio.Pause();
        }
    }
    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        gameIsPaused = false;

        AudioSource[] audios = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audio in audios)
        {
            audio.Play();
        }
    }
    public void Retry()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Resume();
    }

    public void QuitGame()
    {
        GameObject gameManager = GameObject.Find("GameManager");

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneLoader.instance.QuitGame();
        }
        else
        {
            if (gameManager != null)
            {
                Destroy(gameManager);
            }
            else
            {
                Debug.Log("GameManager not found in the scene.");
            }
            SceneManager.LoadScene(0);
            //SceneLoader.instance.LoadScene(0);
        }

        Resume();
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
