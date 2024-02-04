using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [Header("------- Imges -------")]
    private SpriteRenderer theSR;
    public Sprite defaultImg;
    public Sprite pressedImg;

    [Header("------- Keys -------")]
    public KeyCode keyToPress;
    public KeyCode secondaryKey;

    // Getting the SpriteRenderer of the component
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
    }

    // Checking if the button on the keyboard is pressed and changing the img
    void Update()
    {
        if (!PauseMenu.instance.gameIsPaused)
        {
            if (Input.GetKeyDown(keyToPress))
            {
                theSR.sprite = pressedImg;
            }

            if (Input.GetKeyUp(keyToPress))
            {
                theSR.sprite = defaultImg;
            }
        }
    }
}
