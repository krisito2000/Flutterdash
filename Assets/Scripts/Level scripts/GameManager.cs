using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TerrainTools;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("------- Sound -------")]
    public AudioSource music;
    public float levelSpeed;
    //public AudioSource noteHitSound;

    [Header("------- Note manager -------")]
    public float bpm;
    private bool startMusic;
    public GameObject Notes;
    public NoteMovement NoteMovement;
    public Text pressAnyKey;
    public Animator circleAnimator;

    [Header("------- Score -------")]
    public Text scoreText;
    public Text resultsScoreText;
    private int currentScore;
    private int scorePerNote;

    private int scoreEarly;
    private int scoreEarlyPerfect;
    private int scorePerfect;
    private int scoreLatePerfect;
    private int scoreLate;

    private int noteStreak;
    public int bestStreak;
    public Text streakText;

    [Header("------- Multiplier -------")]
    public Text multiplierText;
    public SpriteRenderer multiplierBackground;
    private int currentMultiplier;
    private int multiplierTracker;
    public int[] multiplierThresholds;

    [Header("------- Results -------")]
    public Animator resultsAnimation;

    private int earlyCounter;
    private int earlyPerfectCounter;
    private int perfectCounter;
    private int latePerfectCounter;
    private int lateCounter;
    private int missedCounter;

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
    public float currentHealth;

    public float earlyHitHeal;
    public float earlyPerfectHitHeal;
    public float perfectHitHeal;
    public float latePerfectHitHeal;
    public float lateHitHeal;
    public float missedHitHeal;

    void Start()
    {
        if (levelSpeed == 0)
        {
            levelSpeed = 1;
        }

        Time.timeScale = levelSpeed;
        music.pitch = levelSpeed;

        instance = this;
        Application.runInBackground = true;
        music.enabled = false;
        //music.volume = //audio mixer;

        // Perfect            light blue  350
        // EPerfect/LPerfect  green       150
        // Early/Late         yellow      75
        // Missed             red         0

        scoreText.text = "0";
        noteStreak = 0;

        currentHealth = 100f;

        scoreEarly = 75;
        scoreEarlyPerfect = 150;
        scorePerfect = 350;
        scoreLatePerfect = 150;
        scoreLate = 75;
        currentMultiplier = 1;
    }

    void Update()
    {
        if (!startMusic)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape))
            {
                startMusic = true;
                NoteMovement.gameStart = true;
                music.enabled = true;
                if (pressAnyKey != null)
                {
                    pressAnyKey.text = string.Empty;
                }
                if (circleAnimator != null)
                {
                    circleAnimator.SetBool("isTriggered", true);
                }
            }
        }
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
            Hearth1.SetActive(false);
            Hearth2.SetActive(false);
            Hearth3.SetActive(false);
            Hearth4.SetActive(false);
            
            Notes.SetActive(false);
            music.Stop();
            pressAnyKey.text = "You're Dead";

            if (resultsAnimation != null)
            {
                Statistics();
            }
            else
            {
                Debug.Log("resultsAnimation is null. Cannot set animation parameter.");
            }
        }

        if (noteStreak >= bestStreak)
        {
            bestStreak = noteStreak;
        }
    }

    public void Statistics()
    {
        resultsAnimation.SetBool("isTriggered", true);

        if (Guest.instance.LoginAs.text != "Login as Guest")
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            int currentScore = GetScore();

            // Get the best score location in the database
            var scoreLocation = DatabaseManager.instance.databaseReference.Child("Users")
                                    .Child(Guest.instance.LoginAs.text)
                                    .Child("Levels")
                                    .Child(currentSceneName)
                                    .Child("BestScore");

            // Retrieve the current best score from the database
            scoreLocation.GetValueAsync().ContinueWith(scoreTask =>
            {
                if (scoreTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve best score: " + scoreTask.Exception.Message);
                    return;
                }

                DataSnapshot scoreSnapshot = scoreTask.Result;

                if (!scoreSnapshot.Exists || currentScore > int.Parse(scoreSnapshot.Value.ToString()))
                {
                    // If the score doesn't exist in the database or the current score is better, update the database with the new score
                    scoreLocation.SetValueAsync(currentScore).ContinueWith(scoreSaveTask =>
                    {
                        if (scoreSaveTask.IsFaulted)
                        {
                            Debug.LogError("Failed to save level stats: " + scoreSaveTask.Exception.Message);
                            return;
                        }

                        Debug.Log("Level score saved successfully!");
                    });
                }
                else
                {
                    // If the current score is not better, do nothing
                    Debug.Log("Current score is not better than the best score in the database.");
                }
            });

            // Only save the best streak if it's greater than the current best streak in the database
            var streakLocation = DatabaseManager.instance.databaseReference.Child("Users")
                                    .Child(Guest.instance.LoginAs.text)
                                    .Child("Levels")
                                    .Child(currentSceneName)
                                    .Child("BestStreak");

            streakLocation.GetValueAsync().ContinueWith(streakTask =>
            {
                if (streakTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve best streak: " + streakTask.Exception.Message);
                    return;
                }

                DataSnapshot streakSnapshot = streakTask.Result;
                int databaseBestStreak = streakSnapshot.Exists ? int.Parse(streakSnapshot.Value.ToString()) : 0;

                if (bestStreak > databaseBestStreak)
                {
                    // If the best streak in the code is greater than the best streak in the database, update the database with the new streak
                    streakLocation.SetValueAsync(bestStreak).ContinueWith(streakSaveTask =>
                    {
                        if (streakSaveTask.IsFaulted)
                        {
                            Debug.LogError("Failed to save best streak: " + streakSaveTask.Exception.Message);
                            return;
                        }

                        Debug.Log("Best streak saved successfully!");
                    });
                }
                else
                {
                    // If the current streak in the code is not better, do nothing
                    Debug.Log("Current streak is not better than the best streak in the database.");
                }
            });
        }

        // Update UI elements
        earlyText.text = earlyCounter.ToString();
        earlyPerfectText.text = earlyPerfectCounter.ToString();
        perfectText.text = perfectCounter.ToString();
        latePerfectText.text = latePerfectCounter.ToString();
        lateText.text = lateCounter.ToString();
        missedText.text = missedCounter.ToString();
        resultsScoreText.text = currentScore.ToString();
    }



    private int GetScore()
    {
        // Parse the string score to an int
        int score;
        if (int.TryParse(scoreText.text, out score))
        {
            return score;
        }
        else
        {
            // Handle the case where the conversion fails, perhaps by returning a default value
            Debug.LogError("Failed to parse score from GameManager: " + scoreText.text);
            return 0; // Default value
        }
    }

    public void Heal(float damageHeal)
    {
        currentHealth += damageHeal;
        if (currentHealth >= 100f)
        {
            currentHealth = 100f;
        }
    }

    public void MultiplierBackground()
    {
        //Multiplier background color change
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

    public void NoteHit()
    {
        // Multiplier
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

        currentScore += scorePerNote * currentMultiplier;
        scoreText.text = $"{currentScore}";
        MultiplierBackground();

        streakText.text = noteStreak + "x";
    }

    public void EarlyHit()
    {
        earlyCounter++;
        noteStreak = 0;

        currentScore += scoreEarly * currentMultiplier;
        NoteHit();
        Heal(earlyHitHeal);

        Debug.Log("Early Hit");
    }

    public void EarlyPerfectHit()
    {
        earlyPerfectCounter++;
        noteStreak ++;

        currentScore += scoreEarlyPerfect * currentMultiplier;
        NoteHit();
        Heal(earlyPerfectHitHeal);

        Debug.Log("Early Perfect Hit");
    }

    public void PerfectHit()
    {
        perfectCounter++;
        noteStreak++;

        currentScore += scorePerfect * currentMultiplier;
        NoteHit();
        Heal(perfectHitHeal);

        Debug.Log("Perfect Hit");
    }

    public void LatePerfectHit()
    {
        latePerfectCounter++;
        noteStreak++;

        currentScore += scoreLatePerfect * currentMultiplier;
        NoteHit();
        Heal(latePerfectHitHeal);

        Debug.Log("Late Perfect Hit");
    }
    public void LateHit()
    {
        lateCounter++;
        noteStreak = 0;

        currentScore += scoreLate * currentMultiplier;
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

    public void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
        Destroy(gameObject);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
        Destroy(gameObject);
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