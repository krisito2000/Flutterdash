using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("------- Images -------")]
    private SpriteRenderer theSR;
    public Sprite defaultImg;
    public Sprite pressedImg;

    [Header("------- Keys -------")]
    public KeyToPress keyToPress;

    // Enum to define the keys
    public enum KeyToPress { Up, Left, Down, Right }

    void Start()
    {
        // Getting the SpriteRenderer of the component
        theSR = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!PauseMenu.instance.gameIsPaused)
        {
            // Check if the button is being held down
            if ((keyToPress == KeyToPress.Up && InputSystemController.instance.UpCircleClicked) ||
                (keyToPress == KeyToPress.Down && InputSystemController.instance.DownCircleClicked) ||
                (keyToPress == KeyToPress.Left && InputSystemController.instance.LeftCircleClicked) ||
                (keyToPress == KeyToPress.Right && InputSystemController.instance.RightCircleClicked))
            {
                theSR.sprite = pressedImg; // Set pressed image when button is held
            }
            // Check if the button was released
            else if ((keyToPress == KeyToPress.Up && InputSystemController.instance.UpCircleRelease) ||
                     (keyToPress == KeyToPress.Down && InputSystemController.instance.DownCircleRelease) ||
                     (keyToPress == KeyToPress.Left && InputSystemController.instance.LeftCircleRelease) ||
                     (keyToPress == KeyToPress.Right && InputSystemController.instance.RightCircleRelease))
            {
                theSR.sprite = defaultImg; // Revert to default image when button is released
            }
        }
    }
}