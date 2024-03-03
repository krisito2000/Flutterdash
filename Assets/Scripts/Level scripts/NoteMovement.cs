using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    public static NoteMovement instance;

    [Header("------- Direction -------")]
    [Tooltip("Direction of movement for the note")]
    public MovementDirection direction;
    public enum MovementDirection { Up, Left, Down, Right }

    [Header("------- Growth -------")]
    [Tooltip("Rate of growth for the note")]
    public float growthRate = 0.001f;

    // Flag indicating whether the game has started
    [Header("------- Started -------")]
    public bool gameStart = false;

    private float bpm; // Beats per minute

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        bpm = GameManager.instance.bpm; // Retrieve the beats per minute from GameManager
        bpm /= 60; // Convert beats per minute to beats per second
        growthRate *= bpm; // Adjust growth rate based on beats per second
    }

    // Update is called once per frame
    void Update()
    {
        // Check if any key is pressed to start the game
        if (Input.anyKeyDown)
        {
            gameStart = true;
        }
        if (gameStart)
        {
            // Calculate the movement direction based on the specified direction
            Vector3 movementDirection = GetMovementDirection();

            // Check if the note is within the allowed movement range
            if (transform.position.z <= -1)
            {
                // Move the note in the calculated direction and scale it accordingly
                transform.position += (movementDirection * bpm) * Time.deltaTime;

                Vector3 scale = transform.localScale;
                // Check if the scale is within bounds, then increment it
                if (scale.x <= 0.4f || scale.y <= 0.4f)
                {
                    scale += new Vector3(0.005f, 0.005f, 0);
                    transform.localScale = scale;
                    scale.x = growthRate * Time.deltaTime;
                }
            }
            else
            {
                // Move the note towards the player when it goes beyond the allowed range
                transform.position += (Vector3.back * bpm) * Time.deltaTime;
            }
        }
        // Enable or disable rendering and animation based on the note's position
        if (transform.position.z <= 1)
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
            gameObject.GetComponent<Animator>().enabled = true;
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            gameObject.GetComponent<Animator>().enabled = false;
        }
    }

    // Retrieve the movement direction based on the specified enum
    private Vector3 GetMovementDirection()
    {
        Vector3 movementDirection = Vector3.zero;

        // Switch statement to assign movement direction based on enum value
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
                // Log an error if an invalid direction is specified
                Debug.LogError("Invalid direction specified in NoteMovement script.");
                break;
        }

        return movementDirection; // Return the calculated movement direction
    }
}
