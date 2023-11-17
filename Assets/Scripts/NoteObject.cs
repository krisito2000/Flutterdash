using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool purfectTrigger = false;
    public KeyCode keyToPress;
    public Transform circle;

    private static List<NoteObject> activeNotes = new List<NoteObject>();
    public float noteID;

    void Start()
    {
        activeNotes.Add(this);

        // Assign an ID based on Z-axis
        noteID = transform.position.z * 1000;
    }

    void Update()
    {
        float distanceDetection = Vector2.Distance(transform.position, circle.position);

        if (Input.GetKeyDown(keyToPress))
        {
            NoteObject closestNote = GetClosestNote();

            if (closestNote == this)
            {
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
        Vector3 circlePosition = circle.position;

        foreach (NoteObject note in activeNotes)
        {
            if (!note.purfectTrigger)
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

        // EL
        if (distanceDetection >= 2.25)
        {
            GameManager.instance.EarlyHit();
            this.gameObject.SetActive(false);
        }
        // ELPurfect
        else if (distanceDetection >= 2.77)
        {
            GameManager.instance.EarlyPurfectHit();
            this.gameObject.SetActive(false);
        }
        // Purfect
        else
        {
            GameManager.instance.PurfectHit();
            this.gameObject.SetActive(false);
        }
        activeNotes.Remove(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        purfectTrigger = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        purfectTrigger = false;
    }
}