using Firebase;
using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NoteObject : MonoBehaviour
{
    public static NoteObject instance;

    [Header("------- Note verification -------")]
    public bool circleTrigger = false;
    private bool noteExited = false;
    public CircleCollider2D circleCollider;
    
    public KeyCode keyToPressKeyCode;

    public Transform circle;

    public enum KeyToPress { Up, Left, Down, Right }
    public KeyToPress keyToPress;

    private static List<NoteObject> activeNotes = new List<NoteObject>();
    public bool isTheLastNote;
    public float noteID;

    void Start()
    {
        instance = this;
    }
    private void Pressed()
    {
        NoteObject closestNote = GetClosestNote();

        if (closestNote == this)
        {
            noteExited = true;
            NoteAccuracy();

            GameManager.instance.noteHitSound.Play();
        }
    }


    void Update()
    {
        if (CompassInputSystem.instance.UpCircleClicked && keyToPress == KeyToPress.Up && !PauseMenu.instance.gameIsPaused)
        {
            Pressed();
        }
        else if (CompassInputSystem.instance.DownCircleClicked && keyToPress == KeyToPress.Down && !PauseMenu.instance.gameIsPaused)
        {
            Pressed();
        }
        else if (CompassInputSystem.instance.LeftCircleClicked && keyToPress == KeyToPress.Left && !PauseMenu.instance.gameIsPaused)
        {
            Pressed();
        }
        else if (CompassInputSystem.instance.RightCircleClicked && keyToPress == KeyToPress.Right && !PauseMenu.instance.gameIsPaused)
        {
            Pressed();
        }
        //// For when you spam the note to take damage (does not work)
        //if (!circleTrigger && transform.position.x != 0 && transform.position.y != 0)
        //{
        //    GameManager.instance.currentHealth += GameManager.instance.missedHitHeal;
        //}
    }

    private NoteObject GetClosestNote()
    {
        NoteObject closestNote = null;
        float closestDistance = float.MaxValue;
        Vector2 circlePosition = circle.position;

        foreach (NoteObject note in activeNotes)
        {
            if (!note.circleTrigger)
            {
                // Skip notes that can't be pressed.
                continue;
            }

            float distance = Vector2.Distance(note.transform.position, circlePosition);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNote = note;
            }
        }

        return closestNote;
    }

    private void NoteAccuracy()
    {
        float distanceDetection = Vector2.Distance(transform.position, circle.position);
        if (isTheLastNote && !string.IsNullOrEmpty(DatabaseManager.instance.username))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            GameManager.instance.Statistics();

            // Get the best speed location in the database
            var speedLocation = DatabaseManager.instance.databaseReference
                                    .Child("Users")
                                    .Child(Guest.instance.LoginAs.text)
                                    .Child("Levels")
                                    .Child(currentSceneName)
                                    .Child("BestSpeed");

            // Retrieve the current best speed from the database
            speedLocation.GetValueAsync().ContinueWith(speedTask =>
            {
                if (speedTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve best speed: " + speedTask.Exception.Message);
                    return;
                }

                DataSnapshot speedSnapshot = speedTask.Result;
                float databaseBestSpeed = speedSnapshot.Exists ? float.Parse(speedSnapshot.Value.ToString()) : 0f;

                float currentSpeed = PauseMenu.instance.speedUpPercentage;

                if (currentSpeed > databaseBestSpeed)
                {
                    // If the current speed in the game is greater than the best speed in the database, update the database with the new speed
                    speedLocation.SetValueAsync(currentSpeed).ContinueWith(speedSaveTask =>
                    {
                        if (speedSaveTask.IsFaulted)
                        {
                            Debug.LogError("Failed to save best speed: " + speedSaveTask.Exception.Message);
                            return;
                        }

                        Debug.Log("Best speed saved successfully!");
                    });
                }
                else
                {
                    // If the current speed in the game is not better, do nothing
                    Debug.Log("Current speed is not better than the best speed in the database.");
                }
            });

            // Update best streak and best score

            // Get the best streak location in the database
            var streakLocation = DatabaseManager.instance.databaseReference.Child("Users")
                                    .Child(Guest.instance.LoginAs.text)
                                    .Child("Levels")
                                    .Child(currentSceneName)
                                    .Child("BestStreak");

            // Retrieve the current best streak from the database
            streakLocation.GetValueAsync().ContinueWith(streakTask =>
            {
                if (streakTask.IsFaulted)
                {
                    Debug.LogError("Failed to retrieve best streak: " + streakTask.Exception.Message);
                    return;
                }

                DataSnapshot streakSnapshot = streakTask.Result;
                int databaseBestStreak = streakSnapshot.Exists ? int.Parse(streakSnapshot.Value.ToString()) : 0;

                int currentStreak = GameManager.instance.bestStreak;

                if (currentStreak > databaseBestStreak)
                {
                    // If the best streak in the game is greater than the best streak in the database, update the database with the new streak
                    streakLocation.SetValueAsync(currentStreak).ContinueWith(streakSaveTask =>
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
                    // If the current streak in the game is not better, do nothing
                    Debug.Log("Current streak is not better than the best streak in the database.");
                }
            });

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
                int databaseBestScore = scoreSnapshot.Exists ? int.Parse(scoreSnapshot.Value.ToString()) : 0;

                int currentScore = GetScore();

                if (currentScore > databaseBestScore)
                {
                    // If the current score in the game is greater than the best score in the database, update the database with the new score
                    scoreLocation.SetValueAsync(currentScore).ContinueWith(scoreSaveTask =>
                    {
                        if (scoreSaveTask.IsFaulted)
                        {
                            Debug.LogError("Failed to save best score: " + scoreSaveTask.Exception.Message);
                            return;
                        }

                        Debug.Log("Best score saved successfully!");
                    });
                }
                else
                {
                    // If the current score in the game is not better, do nothing
                    Debug.Log("Current score is not better than the best score in the database.");
                }
            });
        }

        //if (noteAnimation)
        //{
        //    AnimationManager.instance.NoteAnimation();
        //}

        // EL
        if (distanceDetection >= 0.774)
        {
            GameManager.instance.EarlyHit();
            this.gameObject.SetActive(false);
        }
        // ELPerfect  0.263
        else if (distanceDetection >= 0.263)
        {
            GameManager.instance.EarlyPerfectHit();
            this.gameObject.SetActive(false);
        }
        // Perfect  0.116
        else
        {
            GameManager.instance.PerfectHit();
            this.gameObject.SetActive(false);
        }
        // TODO: Create Late and Late Perfect
    }

    private int GetScore()
    {
        // Parse the string score to an int
        int score;
        if (int.TryParse(GameManager.instance.scoreText.text, out score))
        {
            return score;
        }
        else
        {
            // Handle the case where the conversion fails, perhaps by returning a default value
            Debug.LogError("Failed to parse score from GameManager: " + GameManager.instance.scoreText.text);
            return 0; // Default value
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = true;
            noteExited = false;
            if (!activeNotes.Contains(this))
            {
                activeNotes.Add(this);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = false;
            if (!noteExited)
            {
                noteExited = true;
                Debug.Log("Note missed: " + this.gameObject.name);
                GameManager.instance.NoteMissed();
                activeNotes.Remove(this);
                gameObject.SetActive(false);
            }
            if (isTheLastNote)
            {
                GameManager.instance.Statistics();
            }
        }
    }
}