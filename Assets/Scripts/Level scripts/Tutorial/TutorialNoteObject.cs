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

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(keyToPress))
        {
            NoteAccuracy();
        }
    }

    private void NoteAccuracy()
    {
        float distanceDetection = Vector2.Distance(transform.position, circle.position);

        // EL
        if (distanceDetection >= 0.774)
        {
            this.gameObject.SetActive(false);
        }
        // ELPerfect
        else if (distanceDetection >= 0.263)
        {
            this.gameObject.SetActive(false);
        }
        // Perfect
        else
        {
            this.gameObject.SetActive(false);
        }
        // TODO: Create Late and Late Perfect
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Activator") && other.gameObject.tag == circleCollider.tag)
        {
            circleTrigger = false;
            gameObject.SetActive(false);
        }
    }
}
