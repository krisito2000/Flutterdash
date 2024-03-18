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

    [Header("------- Score -------")]
    [Tooltip("UI text displaying current score")]
    public Text scoreText;
    [Tooltip("UI text displaying final score in results")]
    public Text resultsScoreText;
    private int currentScore; // Current score
    private int scorePerNote; // Score gained per note hit

    // Score thresholds for different note timings
    public float scorePerfect;
    public float scoreEarlyPerfect;
    public float scoreEarly;

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
    private int perfectCounter;
    private int earlyPerfectCounter;
    private int earlyCounter;
    private int missedCounter;

    // UI text elements displaying counters for different note timings
    public Text perfectText;
    public Text earlyPerfectText;
    public Text earlyText;
    public Text missedText;

    [Header("------- Hearths -------")]
    [Header("Hearth 1")]
    public GameObject Hearth1TopLeftHalf;
    public GameObject Hearth1TopRightHalf;
    public GameObject Hearth1BottomLeftHalf;
    public GameObject Hearth1BottomRightHalf;
    [Header("Hearth 2")]
    public GameObject Hearth2TopLeftHalf;
    public GameObject Hearth2TopRightHalf;
    public GameObject Hearth2BottomLeftHalf;
    public GameObject Hearth2BottomRightHalf;
    [Header("Hearth 3")]
    public GameObject Hearth3TopLeftHalf;
    public GameObject Hearth3TopRightHalf;
    public GameObject Hearth3BottomLeftHalf;
    public GameObject Hearth3BottomRightHalf;
    [Header("Hearth 4")]
    public GameObject Hearth4TopLeftHalf;
    public GameObject Hearth4TopRightHalf;
    public GameObject Hearth4BottomLeftHalf;
    public GameObject Hearth4BottomRightHalf;

    [Header("------- Health system -------")]
    [Tooltip("Current health of the player")]
    public float currentHealth;

    // Healing values for different note timings
    public float perfectHitHeal;
    public float earlyPerfectHitHeal;
    public float earlyHitHeal;
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

        scorePerfect *= levelSpeed;
        scoreEarlyPerfect *= levelSpeed;
        scoreEarly *= levelSpeed;
        
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
            }
        }

        // Handling hearth UI based on player health
        if (currentHealth == 100f)
        {
            HearthsSet(SetHearthsArray(16));
        }
        else if (currentHealth >= 93.75f && currentHealth < 100f)
        {
            HearthsSet(SetHearthsArray(15));
        }
        else if (currentHealth >= 87.5f && currentHealth < 93.75f)
        {
            HearthsSet(SetHearthsArray(14));
        }
        else if (currentHealth >= 81.25f && currentHealth < 87.5f)
        {
            HearthsSet(SetHearthsArray(13));
        }
        else if (currentHealth >= 75f && currentHealth < 81.25f)
        {
            HearthsSet(SetHearthsArray(12));
        }
        else if (currentHealth >= 68.75f && currentHealth < 75f)
        {
            HearthsSet(SetHearthsArray(11));
        }
        else if (currentHealth >= 62.5f && currentHealth < 68.75f)
        {
            HearthsSet(SetHearthsArray(10));
        }
        else if (currentHealth >= 56.25f && currentHealth < 62.5f)
        {
            HearthsSet(SetHearthsArray(9));
        }
        else if (currentHealth >= 50f && currentHealth < 56.25f)
        {
            HearthsSet(SetHearthsArray(8));
        }
        else if (currentHealth >= 43.75f && currentHealth < 50f)
        {
            HearthsSet(SetHearthsArray(7));
        }
        else if (currentHealth >= 37.5f && currentHealth < 43.75f)
        {
            HearthsSet(SetHearthsArray(6));
        }
        else if (currentHealth >= 31.25f && currentHealth < 37.5f)
        {
            HearthsSet(SetHearthsArray(5));
        }
        else if (currentHealth >= 25f && currentHealth < 31.25f)
        {
            HearthsSet(SetHearthsArray(4));
        }
        else if (currentHealth >= 18.75f && currentHealth < 25f)
        {
            HearthsSet(SetHearthsArray(3));
        }
        else if (currentHealth >= 12.5f && currentHealth < 18.75f)
        {
            HearthsSet(SetHearthsArray(2));
        }
        else if (currentHealth >= 6.25f && currentHealth < 12.5f)
        {
            HearthsSet(SetHearthsArray(1));
        }
        else if (currentHealth > 0f && currentHealth < 6.25f)
        {
            HearthsSet(SetHearthsArray(0));
        }
        else if (currentHealth <= 0f)
        {
            // Game over conditions
            Hearth1TopLeftHalf.SetActive(false);
            Hearth1TopRightHalf.SetActive(false);
            Hearth1BottomLeftHalf.SetActive(false);
            Hearth1BottomRightHalf.SetActive(false);
            Hearth2TopLeftHalf.SetActive(false);
            Hearth2TopRightHalf.SetActive(false);
            Hearth2BottomLeftHalf.SetActive(false);
            Hearth2BottomRightHalf.SetActive(false);
            Hearth3TopLeftHalf.SetActive(false);
            Hearth3TopRightHalf.SetActive(false);
            Hearth3BottomLeftHalf.SetActive(false);
            Hearth3BottomRightHalf.SetActive(false);
            Hearth4TopLeftHalf.SetActive(false);
            Hearth4TopRightHalf.SetActive(false);
            Hearth4BottomLeftHalf.SetActive(false);
            Hearth4BottomRightHalf.SetActive(false);

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
        UpdateBestStreak();
    }

    bool[] SetHearthsArray(int activeCount)
    {
        if (activeCount < 0 || activeCount > 16)
        {
            Debug.LogError("Invalid number of active hearths specified.");
            return null;
        }

        bool[] states = new bool[16];

        for (int i = 0; i < 16 - activeCount; i++)
        {
            states[i] = false;
        }

        for (int i = 16 - activeCount; i < 16; i++)
        {
            states[i] = true;
        }

        return states;
    }

    void HearthsSet(bool[] hearthStates)
    {
        if (hearthStates == null || hearthStates.Length != 16)
        {
            Debug.LogError("Invalid hearth states provided.");
            return;
        }

        Hearth1TopLeftHalf.SetActive(hearthStates[0]);
        Hearth1TopRightHalf.SetActive(hearthStates[1]);
        Hearth1BottomLeftHalf.SetActive(hearthStates[2]);
        Hearth1BottomRightHalf.SetActive(hearthStates[3]);
        Hearth2TopLeftHalf.SetActive(hearthStates[4]);
        Hearth2TopRightHalf.SetActive(hearthStates[5]);
        Hearth2BottomLeftHalf.SetActive(hearthStates[6]);
        Hearth2BottomRightHalf.SetActive(hearthStates[7]);
        Hearth3TopLeftHalf.SetActive(hearthStates[8]);
        Hearth3TopRightHalf.SetActive(hearthStates[9]);
        Hearth3BottomLeftHalf.SetActive(hearthStates[10]);
        Hearth3BottomRightHalf.SetActive(hearthStates[11]);
        Hearth4TopLeftHalf.SetActive(hearthStates[12]);
        Hearth4TopRightHalf.SetActive(hearthStates[13]);
        Hearth4BottomLeftHalf.SetActive(hearthStates[14]);
        Hearth4BottomRightHalf.SetActive(hearthStates[15]);
    }

    // Updating best streak achieved
    void UpdateBestStreak()
    {
        if (noteStreak >= bestStreak)
        {
            bestStreak = noteStreak;
        }
    }

    // Fetching player attempts from the database
    void FetchAttemptsFromDatabase()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        string playerName = DatabaseManager.instance.username;

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
        string playerName = DatabaseManager.instance.username;

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

        perfectText.text = perfectCounter.ToString();
        earlyPerfectText.text = earlyPerfectCounter.ToString();
        earlyText.text = earlyCounter.ToString();
        
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
