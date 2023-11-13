using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool purfectTrigger = false;
    public KeyCode keyToPress;
    public Transform circle;

    private static List<NoteObject> activeNotes = new List<NoteObject>();

    void Start()
    {
        activeNotes.Add(this);
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
            gameObject.SetActive(false);
        }
    }

    //private NoteObject DivideDifferentCircles()
    //{
    //    Debug.Log("findCircle: " + findCircle);

    //    Debug.Log("activeNotesA count: " + activeNotesA.Count);
    //    Debug.Log("activeNotesW count: " + activeNotesW.Count);
    //    Debug.Log("activeNotesS count: " + activeNotesS.Count);
    //    Debug.Log("activeNotesD count: " + activeNotesD.Count);

    //    switch (findCircle)
    //    {
    //        case WhichCircle.W:
    //            return FindClosestNote(activeNotesW);
    //        case WhichCircle.A:
    //            return FindClosestNote(activeNotesA);
    //        case WhichCircle.S:
    //            return FindClosestNote(activeNotesS);
    //        case WhichCircle.D:
    //            return FindClosestNote(activeNotesD);
    //        default:
    //            return null;
    //    }
    //}

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
        if (distanceDetection >= 1.987153)
        {
            GameManager.instance.NoteEL();
            this.gameObject.SetActive(false);
        }
        // ELPurfect
        else if (distanceDetection >= 0.7786305)
        {
            GameManager.instance.NoteELPurfect();
            this.gameObject.SetActive(false);
        }
        // Purfect
        else
        {
            GameManager.instance.NotePurfect();
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other is CircleCollider2D)
        {
            purfectTrigger = true;
        }
        else if (other is BoxCollider2D)
        {
            purfectTrigger = false;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other is CircleCollider2D)
        {
            purfectTrigger = false;
        }
        else if (other is BoxCollider2D)
        {
            purfectTrigger = false;
        }
    }

}