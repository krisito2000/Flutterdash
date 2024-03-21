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

    // Flag indicating whether the note has been triggered by the circle
    [Header("------- Note verification -------")]
    [Tooltip("Indicator for the note that has been triggered in the collider")]
    public bool circleTrigger = false;
    private bool noteExited = false; // Flag indicating whether the note has exited the circle

    [Tooltip("Enum representing the key to press for this note")]
    public enum KeyToPress { Up, Left, Down, Right }
    public KeyToPress keyToPress;

    private static List<NoteObject> activeNotes = new List<NoteObject>(); // List of active notes in the circle collider
    [Tooltip("Indicates whether this note is the last note in the level")]
    public bool isTheLastNote;
    //[Tooltip("Unique identifier for the note")]
    //public float noteID;

    [Header("------- Compass circle -------")]
    [Tooltip("Collider representing the circle area")]
    public CircleCollider2D circleCollider;
    [Tooltip("The compass circle")]
    public Transform circle;

    [Header("Animations")]
    public ParticleSystem onClickParticle;
    

    void Start()
    {
        instance = this;
        //noteID = transform.position.z;

        switch (keyToPress)
        {
            case KeyToPress.Up:
                onClickParticle = GameObject.Find("UpCircleParticle").GetComponent<ParticleSystem>();
                break;
            case KeyToPress.Left:
                onClickParticle = GameObject.Find("LeftCircleParticle").GetComponent<ParticleSystem>();
                break;
            case KeyToPress.Down:
                onClickParticle = GameObject.Find("DownCircleParticle").GetComponent<ParticleSystem>();
                break;
            case KeyToPress.Right:
                onClickParticle = GameObject.Find("RightCircleParticle").GetComponent<ParticleSystem>();
                break;
            default:
                Debug.LogError("Invalid KeyToPress enum value.");
                break;
        }
    }

    void Update()
    {
        // Check if the corresponding circle is clicked
        if ((InputSystemController.instance.UpCircleClicked && keyToPress == KeyToPress.Up && !PauseMenu.instance.gameIsPaused) ||
            (InputSystemController.instance.DownCircleClicked && keyToPress == KeyToPress.Down && !PauseMenu.instance.gameIsPaused) ||
            (InputSystemController.instance.LeftCircleClicked && keyToPress == KeyToPress.Left && !PauseMenu.instance.gameIsPaused) ||
            (InputSystemController.instance.RightCircleClicked && keyToPress == KeyToPress.Right && !PauseMenu.instance.gameIsPaused))
        {
            Pressed(); // Handle the press event
        }
    }

    private void Pressed()
    {
        NoteObject closestNote = GetClosestNote(); // Get the closest note to the circle

        if (closestNote == this)
        {
            noteExited = true;

            NoteAccuracy();
        }
    }

    private NoteObject GetClosestNote()
    {
        NoteObject closestNote = null; // Initialize the closest note variable
        float closestDistance = float.MaxValue; // Initialize the closest distance variable to a high value
        Vector2 circlePosition = circle.position; // Get the position of the circle

        foreach (NoteObject note in activeNotes) // Iterate through all active notes
        {
            if (!note.circleTrigger)
            {
                // Skip notes that can't be pressed.
                continue;
            }

            // Calculate the distance between the current note and the circle
            float distance = Vector2.Distance(note.transform.position, circlePosition);

            // Check if the current note is closer than the previously closest note
            if (distance < closestDistance)
            {
                closestDistance = distance; // Update the closest distance
                closestNote = note; // Update the closest note
            }
        }

        return closestNote; // Return the closest note to the circle
    }


    private void NoteAccuracy()
    {
        float distanceDetection = Vector2.Distance(transform.position, circle.position);

        // Check if this is the last note and the username is not empty
        if (isTheLastNote && !string.IsNullOrEmpty(DatabaseManager.instance.username))
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            GameManager.instance.Statistics(); // Update game statistics

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

        // Determine note accuracy based on distance to the circle
        if (distanceDetection > 0.682)
        {
            GameManager.instance.NoteMissed(); // Handle missed note
            Debug.Log("Too early");
        }
        // EL
        else if (distanceDetection <= 0.682 && distanceDetection > 0.304)
        {
            NoteParticles(Color.yellow);
            GameManager.instance.EarlyHit(); // Handle early hit
            NoteBeingClicked();
        }
        // ELPerfect
        else if (distanceDetection <= 0.304 && distanceDetection > 0.2)
        {
            NoteParticles(new Color(1.0f, 0.64f, 0.0f));
            GameManager.instance.EarlyPerfectHit(); // Handle early perfect hit
            NoteBeingClicked();
        }
        // Perfect
        else
        {
            NoteParticles(Color.green);
            GameManager.instance.PerfectHit(); // Handle perfect hit
            NoteBeingClicked();
        }
    }

    private void NoteBeingClicked()
    {
        activeNotes.Remove(this);
        GameManager.instance.noteHitSound.Play();
        gameObject.SetActive(false);
    }

    private void NoteParticles(Color color)
    {
        var mainModule = onClickParticle.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(color);
        onClickParticle.Play();
    }

    // Get the score from the GameManager
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

    // Handle OnTriggerEnter2D event
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collider is tagged as an activator and matches the circle collider tag
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = true;
            noteExited = false;
            if (!activeNotes.Contains(this))
            {
                activeNotes.Add(this); // Add this note to the list of active notes
            }
        }
    }

    // Handle OnTriggerExit2D event
    private void OnTriggerExit2D(Collider2D other)
    {
        // Check if the collider is tagged as an activator and matches the circle collider tag
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = false;
            if (!noteExited)
            {
                noteExited = true;
                Debug.Log("Note missed: " + this.gameObject.name);
                GameManager.instance.NoteMissed(); // Handle note missed event
                activeNotes.Remove(this); // Remove this note from the list of active notes
                gameObject.SetActive(false); // Deactivate the note object
            }
            if (isTheLastNote)
            {
                GameManager.instance.Statistics(); // Update game statistics
            }
        }
    }

}
