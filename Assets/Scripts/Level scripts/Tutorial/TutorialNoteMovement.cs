using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNoteMovement : MonoBehaviour
{
    public static TutorialNoteMovement instance;

    [Header("------- Direction -------")]
    public MovementDirection direction;
    public enum MovementDirection { Up, Left, Down, Right }

    [Header("------- Growth -------")]
    public float growthRate = 0.001f;

    [Header("------- Started -------")]
    public bool gameStart = false;

    private float bpm;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        bpm = TutorialGameManager.instance.bpm;
        bpm /= 60;
        growthRate *= bpm;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the movement direction based on the specified direction string.
        Vector3 movementDirection = GetMovementDirection();

        if (transform.position.z <= -1)
        {
            // Move the note in the calculated direction and scale it accordingly.
            transform.position += (movementDirection * bpm) * Time.deltaTime;

            Vector3 scale = transform.localScale;
            if (scale.x <= 0.4f || scale.y <= 0.4f)
            {
                scale += new Vector3(0.005f, 0.005f, 0);
                transform.localScale = scale;
                scale.x = growthRate * Time.deltaTime;
            }
        }
        else
        {
            transform.position += (Vector3.back * bpm) * Time.deltaTime;
        }
    }

    // Validating the movement direction
    private Vector3 GetMovementDirection()
    {
        Vector3 movementDirection = Vector3.zero;

        switch (direction)
        {
            case MovementDirection.Up:
                movementDirection = Vector3.up;
                break;
            case MovementDirection.Left:
                movementDirection = Vector3.left;
                break;
            case MovementDirection.Down:
                movementDirection = Vector3.down;
                break;
            case MovementDirection.Right:
                movementDirection = Vector3.right;
                break;
            default:
                Debug.LogError("Invalid direction specified in NoteMovement script.");
                break;
        }

        return movementDirection;
    }
}
