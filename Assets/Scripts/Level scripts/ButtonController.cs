using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [Header("------- Images -------")]
    private SpriteRenderer theSR;
    [Tooltip("Default sprite image")]
    public Sprite defaultImg;
    [Tooltip("Sprite image when button is pressed")]
    public Sprite pressedImg; // 

    [Header("------- Keys -------")]
    [Tooltip("The direction that the circle is")]
    public Direction keyToPress;
    public enum Direction { Up, Left, Down, Right } // Enum for different keys

    void Start()
    {
        // Getting the SpriteRenderer component from the GameObject
        theSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!PauseMenu.instance.gameIsPaused) // Check if the game is not paused
        {
            // Check if the corresponding button is being held down
            if ((keyToPress == Direction.Up && InputSystemController.instance.UpCircleClicked) ||
                (keyToPress == Direction.Down && InputSystemController.instance.DownCircleClicked) ||
                (keyToPress == Direction.Left && InputSystemController.instance.LeftCircleClicked) ||
                (keyToPress == Direction.Right && InputSystemController.instance.RightCircleClicked))
            {
                theSR.sprite = pressedImg; // Set the sprite to pressed image when the button is held down
            }
            // Check if the corresponding button was released
            else if ((keyToPress == Direction.Up && InputSystemController.instance.UpCircleRelease) ||
                     (keyToPress == Direction.Down && InputSystemController.instance.DownCircleRelease) ||
                     (keyToPress == Direction.Left && InputSystemController.instance.LeftCircleRelease) ||
                     (keyToPress == Direction.Right && InputSystemController.instance.RightCircleRelease))
            {
                theSR.sprite = defaultImg; // Revert the sprite to default image when the button is released
            }
        }
    }
}
