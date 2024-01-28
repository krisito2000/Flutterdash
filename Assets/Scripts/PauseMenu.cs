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

    public bool gameIsPaused = false;

    public GameObject pauseMenuCanvas;
    public TextMeshProUGUI speedUpText;
    public int speedUpPercentage;
    public GameObject speedUpMenuCanvas;

    void Start()
    {
        instance = this;
        speedUpPercentage = 100;
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

    // Pause
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

    // Resume
    public void Resume()
    {
        pauseMenuCanvas.SetActive(false);
        speedUpMenuCanvas.SetActive(false);

        if (GameManager.instance != null)
        {
            Time.timeScale = GameManager.instance.levelSpeed;
        }
        else
        {
            Time.timeScale = 1.0f;
        }

        gameIsPaused = false;

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

    // Speed up
    public void SpeedUpButton()
    {
        pauseMenuCanvas.SetActive(false);
        speedUpMenuCanvas.SetActive(true);
    }

    public void LeftArrowButton()
    {
        speedUpPercentage -= 10;
        if (speedUpPercentage <= 10)
        {
            speedUpPercentage = 10;
        }
        speedUpText.text = $"{speedUpPercentage}%";
    }
    public void RightArrowButton()
    {
        speedUpPercentage += 10;
        if (speedUpPercentage >= 10000000)
        {
            speedUpPercentage = 10000000;
        }
        speedUpText.text = $"{speedUpPercentage}%";
    }

    // Retry
    public void Retry()
    {
        // Unload unused assets
        Resources.UnloadUnusedAssets();
        pauseMenuCanvas.SetActive(false);
        speedUpMenuCanvas.SetActive(false);
        Resume();

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

    // Quit
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
