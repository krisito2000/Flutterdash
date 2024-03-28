using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu instance;

    [Tooltip("Flag indicating whether the game is currently paused.")]
    public bool gameIsPaused = false;

    [Tooltip("Reference to the canvas displaying the pause menu.")]
    public GameObject pauseMenuCanvas;

    [Tooltip("Displaying the speed up percentage.")]
    public TextMeshProUGUI speedUpText;

    [Tooltip("The initial speed up percentage value.")]
    public int speedUpPercentage;

    [Tooltip("Reference to the canvas displaying the speed up menu.")]
    public GameObject speedUpMenuCanvas;


    void Start()
    {
        instance = this;
        speedUpPercentage = 100; // Initialize speed up percentage
    }

    // Update is called once per frame
    void Update()
    {
        // Check for Escape key press to toggle pause menu
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume(); // If game is paused, resume gameplay
            }
            else
            {
                Pause(); // If game is not paused, pause gameplay
            }
        }
    }

    // Pause the game
    public void Pause()
    {
        // Activate the pause menu canvas
        pauseMenuCanvas.SetActive(true);

        // Set time scale to 0 to pause gameplay
        Time.timeScale = 0f;

        // Update game paused flag
        gameIsPaused = true;

        // Pause all audio sources in the scene
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.Pause();
        }
    }

    // Resume the game
    public void Resume()
    {
        // Deactivate the pause menu canvas and speed up menu canvas
        pauseMenuCanvas.SetActive(false);
        speedUpMenuCanvas.SetActive(false);

        // Set time scale to normal to resume gameplay
        if (GameManager.instance != null)
        {
            Time.timeScale = GameManager.instance.levelSpeed;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        // Update game paused flag
        gameIsPaused = false;

        // Resume all audio sources in the scene and adjust pitch if needed
        AudioSource[] audios = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audio in audios)
        {
            audio.Play();
            if (GameManager.instance != null)
            {
                audio.pitch = GameManager.instance.levelSpeed;
            }
            else
            {
                audio.pitch = 1.0f;
            }
        }
    }

    // Open speed up menu
    public void SpeedUpButton()
    {
        pauseMenuCanvas.SetActive(false);
        speedUpMenuCanvas.SetActive(true);
    }

    // Decrease speed up percentage
    public void LeftArrowButton()
    {
        speedUpPercentage -= 10;
        if (speedUpPercentage <= 10)
        {
            speedUpPercentage = 10;
        }
        speedUpText.text = $"{speedUpPercentage}%";
    }

    // Increase speed up percentage
    public void RightArrowButton()
    {
        speedUpPercentage += 10;
        if (speedUpPercentage >= 10000000)
        {
            speedUpPercentage = 10000000;
        }
        speedUpText.text = $"{speedUpPercentage}%";
    }

    public void DoneButtonSpeedUp()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            Retry();
        }
        else
        {
            Resume();
        }
    }

    // Retry the current level
    public void Retry()
    {
        // Unload unused assets to free memory
        // Resources.UnloadUnusedAssets();

        // Resume the game from the pause menu
        Resume();

        // Reload the current scene asynchronously
        Scene currentScene = SceneManager.GetActiveScene();
        StartCoroutine(LoadSceneAsync(currentScene.name));
    }

    // Asynchronously loads a scene by name
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            // Check if the load has finished (90% progress)
            if (asyncLoad.progress >= 0.9f)
            {
                // Activate the scene
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    // Quit the game
    public void QuitGame()
    {
        GameObject gameManager = GameObject.Find("GameManager");

        // If current scene is the main menu, call QuitGame method from SceneLoader instance
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            SceneLoader.instance.QuitGame();
        }
        else
        {
            // Otherwise, destroy the game manager if it exists, then load the main menu scene
            if (gameManager != null)
            {
                Destroy(gameManager);
            }
            else
            {
                Debug.Log("GameManager not found in the scene.");
            }
            SceneManager.LoadScene(0);
        }

        // Resume gameplay after quitting
        Resume();
    }

    // Ensure only one instance of PauseMenu script exists throughout the game
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
}
