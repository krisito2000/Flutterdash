using Firebase.Database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NoteObject : MonoBehaviour
{
    public static NoteObject instance;

    [Header("------- Note verification -------")]
    public bool circleTrigger = false;
    private bool noteExited = false;
    public CircleCollider2D circleCollider;
    public KeyCode keyToPress;
    public KeyCode secondaryKey;
    public Transform circle;

    [Header("------- Animation -------")]
    public bool noteAnimation;
    public enum SpinDirection { Left, Right }
    public SpinDirection direction;

    [Header("------- Note indentification -------")]
    private static List<NoteObject> activeNotes = new List<NoteObject>();
    public bool isTheLastNote;
    public float noteID;

    void Start()
    {
        instance = this;

        // Assign an ID based on Z-axis
        noteID = transform.position.z * 1000;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPress) || Input.GetKeyDown(secondaryKey))
        {
            NoteObject closestNote = GetClosestNote();

            if (closestNote == this)
            {
                //if (haveAnimation)
                //{
                //    noteAnimation.SetBool("isTriggered", true);
                //}
                noteExited = true;
                NoteAccuracy();
            }
            //// For when you spam the note to take damage (does not work)
            //if (!circleTrigger && transform.position.x != 0 && transform.position.y != 0)
            //{
            //    GameManager.instance.currentHealth += GameManager.instance.missedHitHeal;
            //}
        }
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
        if (isTheLastNote)
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            GameManager.instance.Statistics();

            // Get the best speed location in the database
            var speedLocation = DatabaseManager.instance.databaseReference.Child("Users")
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
                int databaseBestSpeed = speedSnapshot.Exists ? int.Parse(speedSnapshot.Value.ToString()) : 0;

                int currentSpeed = PauseMenu.instance.speedUpPercentage;

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