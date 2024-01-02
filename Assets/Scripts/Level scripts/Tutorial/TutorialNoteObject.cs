using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialNoteObject : MonoBehaviour
{
    public static TutorialNoteObject instance;

    [Header("------- Note verification -------")]
    public bool circleTrigger = false;
    public CircleCollider2D circleCollider;
    public KeyCode keyToPress;
    public Transform circle;

    [Header("------- Note indentification -------")]
    private static List<TutorialNoteObject> activeNotes = new List<TutorialNoteObject>();
    public float noteID;

    void Start()
    {
        instance = this;

        // Assign an ID based on Z-axis
        noteID = transform.position.z * 1000;
    }

    void Update()
    {
        float distanceDetection = Vector2.Distance(transform.position, circle.position);

        if (Input.GetKeyDown(keyToPress))
        {
            TutorialNoteObject closestNote = GetClosestNote();

            if (closestNote == this)
            {
                //if (haveAnimation)
                //{
                //    noteAnimation.SetBool("isTriggered", true);
                //}
                NoteAccuracy();
            }
        }

        if (distanceDetection > 4)
        {
            Debug.Log("Note missed: " + this.gameObject.name);
            TutorialGameManager.instance.NoteMissed();
            activeNotes.Remove(this);
            gameObject.SetActive(false);
        }
    }

    private TutorialNoteObject GetClosestNote()
    {
        TutorialNoteObject closestNote = null;
        float closestDistance = float.MaxValue;
        Vector2 circlePosition = circle.position;

        foreach (TutorialNoteObject note in activeNotes)
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

        //if (noteAnimation)
        //{
        //    AnimationManager.instance.NoteAnimation();
        //}

        // EL
        if (distanceDetection >= 1.187)
        {
            TutorialGameManager.instance.EarlyHit();
            this.gameObject.SetActive(false);
        }
        // ELPerfect
        else if (distanceDetection >= 0.874)
        {
            TutorialGameManager.instance.EarlyPerfectHit();
            this.gameObject.SetActive(false);
        }
        // Perfect
        else
        {
            TutorialGameManager.instance.PerfectHit();
            this.gameObject.SetActive(false);
        }
        // TODO: Create Late and Late Perfect

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = true;
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
            activeNotes.Remove(this);
        }
    }
}
