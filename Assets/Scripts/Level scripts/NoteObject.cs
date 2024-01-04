using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public static NoteObject instance;

    [Header("------- Note verification -------")]
    public bool circleTrigger = false;
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

        if (Input.GetKeyDown(keyToPress) || Input.GetKeyDown(secondaryKey))
        {
            NoteObject closestNote = GetClosestNote();

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
            GameManager.instance.NoteMissed();
            activeNotes.Remove(this);
            gameObject.SetActive(false);
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

        //if (noteAnimation)
        //{
        //    AnimationManager.instance.NoteAnimation();
        //}

        // EL
        if (distanceDetection >= 1.187)
        {
            GameManager.instance.EarlyHit();
            this.gameObject.SetActive(false);
        }
        // ELPerfect
        else if (distanceDetection >= 0.874)
        {
            GameManager.instance.EarlyPerfectHit();
            this.gameObject.SetActive(false);
        }
        // Perfect
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