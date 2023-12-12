using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckBoxClick : MonoBehaviour
{
    public static CheckBoxClick instance;

    public Image img;
    public bool isOn = false;

    public Sprite onImg;
    public Sprite offImge;

    public void Start()
    {
        instance = this;
    }

    public void CheckBoxClickCheck()
    {
        if (!isOn)
        {
            img.sprite = onImg;
            isOn = true;
        }
        else
        {
            img.sprite = offImge;
            isOn = false;
        }  
    }
}
