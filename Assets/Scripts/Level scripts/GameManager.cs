using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("------- Sound -------")]
    [Tooltip("The level music")]
    public AudioSource music;
    [Tooltip("Sound played when a note is hit")]
    public AudioSource noteHitSound;
    [Tooltip("Speed of the level")]
    public float levelSpeed;

    [Header("------- Note manager -------")]
    public float bpm;  // Beats per minute (controlls the note movement speed)
    private bool startMusic;  // Flag indicating whether music has started
    [Tooltip("The reference to the notes")]
    public GameObject Notes;
    [Tooltip("Reference to the note movement class")]
    public NoteMovement NoteMovement;
    [Tooltip("UI text prompting player to press any key to start")]
    public Text pressAnyKey;
    [Tooltip("Animator for circle animation")]
    public Animator circleAnimator;

    [Header("------- Score -------")]
    [Tooltip("UI text displaying current score")]
    public Text scoreText;
    [Tooltip("UI text displaying final score in results")]
    public Text resultsScoreText;
    private int currentScore; // Current score
    private int scorePerNote; // Score gained per note hit

    // Score thresholds for different note timings
    private float scoreEarly;
    private float scoreEarlyPerfect;
    private float scorePerfect;
    private float scoreLatePerfect;
    private float scoreLate;

    private int noteStreak; // Current streak of consecutive notes hit
    [Tooltip("Best streak achieved")]
    public int bestStreak;
    [Tooltip("UI text displaying current streak")]
    public Text streakText;
    [Tooltip("Number of attempts made by the player")]
    public int attempts;

    [Header("------- Multiplier -------")]
    [Tooltip("UI text displaying current multiplier")]
    public Text multiplierText;
    [Tooltip("Background sprite for multiplier UI")]
    public SpriteRenderer multiplierBackground;
    private int currentMultiplier; // Current multiplier value
    private int multiplierTracker; // Tracks progression towards increasing multiplier
    [Tooltip("Thresholds for the number of notes you need to click to increase the multiplier")]
    public int[] multiplierThresholds;

    [Tooltip("UI text displaying speed multiplier")]
    public Text speedMultiplier; 

    [Header("------- Results -------")]
    [Tooltip("Animator for results screen")]
    public Animator resultsAnimation;

    // Counters for different note timings
    private int earlyCounter;
    private int earlyPerfectCounter;
    private int perfectCounter;
    private int latePerfectCounter;
    private int lateCounter;
    private int missedCounter;

    // UI text elements displaying counters for different note timings
    public Text earlyText;
    public Text earlyPerfectText;
    public Text perfectText;
    public Text latePerfectText;
    public Text lateText;
    public Text missedText;

    [Header("------- Hearths -------")]
    public GameObject Hearth1;
    public GameObject Hearth2;
    public GameObject Hearth3;
    public GameObject Hearth4;

    [Header("------- Health system -------")]
    [Tooltip("Current health of the player")]
    public float currentHealth;

    // Healing values for different note timings
    public float earlyHitHeal;
    public float earlyPerfectHitHeal;
    public float perfectHitHeal;
    public float latePerfectHitHeal;
    public float lateHitHeal;
    public float missedHitHeal;

    void Start()
    {
        // Initializing GameManager instance and background application running
        instance = this;
        Application.runInBackground = true;
        music.enabled = false;

        // Adjusting level speed based on settings
        levelSpeed = PauseMenu.instance.speedUpPercentage / 100f;

        if (levelSpeed != 1)
        {
            speedMultiplier.text = $"x{levelSpeed}";
        }

        if (levelSpeed == 0)
        {
            levelSpeed = 1;
        }

        Time.timeScale = levelSpeed;
        music.pitch = levelSpeed;

        // Initializing score variables and fetching attempts from database
        scoreText.text = "0";
        noteStreak = 0;
        currentHealth = 100f;

        scoreEarly = 75 * levelSpeed;
        scoreEarlyPerfect = 150 * levelSpeed;
        scorePerfect = 350 * levelSpeed;
        scoreLatePerfect = 150 * levelSpeed;
        scoreLate = 75 * levelSpeed;
        currentMultiplier = 1;

        FetchAttemptsFromDatabase();
    }

    void Update()
    {
        // Checking for game start
        if (!startMusic)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !PauseMenu.instance.gameIsPaused)
            {
                startMusic = true;
                NoteMovement.gameStart = true;
                music.enabled = true;
                if (pressAnyKey != null)
                {
                    pressAnyKey.text = string.Empty;
                    attempts++;
                    UpdateAttemptsInDatabase();
                }
                if (circleAnimator != null)
                {
                    circleAnimator.SetBool("isTriggered", true);
                }
            }
        }

        // Handling hearth UI based on player health
        if (currentHealth >= 100f)
        {
            Hearth1.SetActive(true);
            Hearth2.SetActive(true);
            Hearth3.SetActive(true);
            Hearth4.SetActive(true);
        }
        else if (currentHealth >= 75f && currentHealth < 100f)
        {
            Hearth1.SetActive(false);
            Hearth2.SetActive(true);
            Hearth3.SetActive(true);
            Hearth4.SetActive(true);
        }
        else if (currentHealth >= 50f && currentHealth < 75f)
        {
            Hearth1.SetActive(false);
            Hearth2.SetActive(false);
            Hearth3.SetActive(true);
            Hearth4.SetActive(true);
        }
        else if (currentHealth >= 25f && currentHealth < 0f)
        {
            Hearth1.SetActive(false);
            Hearth2.SetActive(false);
            Hearth3.SetActive(false);
            Hearth4.SetActive(true);
        }
        else if (currentHealth <= 0f)
        {
            // Game over conditions
            Hearth1.SetActive(false);
            Hearth2.SetActive(false);
            Hearth3.SetActive(false);
            Hearth4.SetActive(false);

            Notes.SetActive(false);
            music.Stop();
            pressAnyKey.text = "You're Dead";

            if (resultsAnimation != null)
            {
                // Displaying results
                Statistics();
            }
            else
            {
                Debug.Log("resultsAnimation is null. Cannot set animation parameter.");
            }
        }

        // Updating best streak achieved
        if (noteStreak >= bestStreak)
        {
            bestStreak = noteStreak;
        }
    }

    // Fetching player attempts from the database
    void FetchAttemptsFromDatabase()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string playerName = Guest.instance.LoginAs.text;

        // Reference to attempts in the database
        var attemptsLocation = DatabaseManager.instance.databaseReference.Child("Users")
                                                                         .Child(playerName)
                                                                         .Child("Levels")
                                                                         .Child(currentSceneName)
                                                                         .Child("Attempts");

        // Fetching attempts value
        attemptsLocation.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    attempts = int.Parse(snapshot.Value.ToString());
                }
                else
                {
                    attempts = 0;
                }
            }
        });
    }
    // Updating attempts in the database
    void UpdateAttemptsInDatabase()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string playerName = Guest.instance.LoginAs.text;

        if (!Guest.instance.guest)
        {
            var attemptsLocation = DatabaseManager.instance.databaseReference.Child("Users")
                                                                             .Child(playerName)
                                                                             .Child("Levels")
                                                                             .Child(currentSceneName)
                                                                             .Child("Attempts");

            attemptsLocation.SetValueAsync(attempts);
        }
    }

    // Displaying statistics at the end of the game
    public void Statistics()
    {
        resultsAnimation.SetBool("isTriggered", true);

        earlyText.text = earlyCounter.ToString();
        earlyPerfectText.text = earlyPerfectCounter.ToString();
        perfectText.text = perfectCounter.ToString();
        latePerfectText.text = latePerfectCounter.ToString();
        lateText.text = lateCounter.ToString();
        missedText.text = missedCounter.ToString();
        resultsScoreText.text = currentScore.ToString();
    }

    // Healing function for the player
    public void Heal(float damageHeal)
    {
        currentHealth += damageHeal;
        if (currentHealth >= 100f)
        {
            currentHealth = 100f;
        }
    }

    // Handling multiplier UI background color
    public void MultiplierBackground()
    {
        if (currentMultiplier == 1)
        {
            multiplierBackground.color = Color.gray;
            multiplierText.color = Color.white;
        }
        else if (currentMultiplier == 2)
        {
            multiplierBackground.color = Color.green;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier == 4)
        {
            multiplierBackground.color = Color.blue;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier == 8)
        {
            multiplierBackground.color = Color.red;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier >= 16)
        {
            multiplierBackground.color = Color.yellow;
            multiplierText.color = Color.black;
        }
    }

    // Function called when a note is hit
    public void NoteHit()
    {
        // Handling multiplier
        if (currentMultiplier / 2 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[currentMultiplier / 2] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier *= 2;
            }
        }

        multiplierText.text = "x" + currentMultiplier;

        // Updating score and UI elements
        currentScore += scorePerNote * currentMultiplier;
        scoreText.text = $"{currentScore}";
        MultiplierBackground();

        streakText.text = noteStreak + "x";
    }

    // Functions for different note timings
    public void EarlyHit()
    {
        earlyCounter++;
        noteStreak = 0;

        currentScore += (int)(scoreEarly * currentMultiplier);
        NoteHit();
        Heal(earlyHitHeal);

        Debug.Log("Early Hit");
    }

    public void EarlyPerfectHit()
    {
        earlyPerfectCounter++;
        noteStreak++;

        currentScore += (int)(scoreEarlyPerfect * currentMultiplier);
        NoteHit();
        Heal(earlyPerfectHitHeal);

        Debug.Log("Early Perfect Hit");
    }

    public void PerfectHit()
    {
        perfectCounter++;
        noteStreak++;

        currentScore += (int)(scorePerfect * currentMultiplier);
        NoteHit();
        Heal(perfectHitHeal);

        Debug.Log("Perfect Hit");
    }

    public void LatePerfectHit()
    {
        latePerfectCounter++;
        noteStreak++;

        currentScore += (int)(scoreLatePerfect * currentMultiplier);
        NoteHit();
        Heal(latePerfectHitHeal);

        Debug.Log("Late Perfect Hit");
    }
    public void LateHit()
    {
        lateCounter++;
        noteStreak = 0;

        currentScore += (int)(scoreLate * currentMultiplier);
        NoteHit();
        Heal(lateHitHeal);

        Debug.Log("Late Hit");
    }

    public void NoteMissed()
    {
        missedCounter++;
        noteStreak = 0;
        streakText.text = noteStreak + "x";
        Heal(missedHitHeal);

        currentMultiplier = 1;
        multiplierTracker = 0;
        multiplierBackground.color = Color.gray;
        multiplierText.color = Color.white;

        multiplierText.text = "x" + currentMultiplier;
        Debug.Log("Missed");
    }

    // Function to restart the scene
    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Destroy(gameObject);
    }

    // Function to load the main menu
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }

    // Ensuring only one instance of GameManager exists in the scene
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance.gameObject);
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }
}
