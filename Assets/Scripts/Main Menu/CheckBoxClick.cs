using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxClick : MonoBehaviour
{
    public static CheckBoxClick instance;

    [Tooltip("Image component representing the checkbox")]
    public Image img;
    [Tooltip("Boolean indicating whether the checkbox is checked")]
    public bool isOn = false;

    [Tooltip("Image to display when the checkbox is checked")]
    public Sprite onImg;
    [Tooltip("Image to display when the checkbox is unchecked")]
    public Sprite offImage;

    // Start is called before the first frame update
    public void Start()
    {
        instance = this;
    }

    // Method to handle checkbox click event
    public void CheckBoxClickCheck()
    {
        if (!isOn)
        {
            img.sprite = onImg; // Set the image to the checked sprite
            isOn = true; // Update the state to checked
        }
        else
        {
            img.sprite = offImage; // Set the image to the unchecked sprite
            isOn = false; // Update the state to unchecked
        }
    }
}
