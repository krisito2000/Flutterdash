using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest : MonoBehaviour
{
    public static Guest instance;

    [Header("------- Canvas -------")]
    public CanvasGroup guestCanvas;

    [Header("------- Is it Guest -------")]
    public Text LoginAs;
    public bool guest;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (LoginAs.text == "Login as Guest")
        {
            guest = true;
            MainMenuTransition.instance.animator.SetBool("isGuest", true);
        }
        else
        {
            guest = false;
            MainMenuTransition.instance.animator.SetBool("isGuest", false);
        }
    }

    public void YesButton()
    {
        MainMenuTransition.instance.animator.SetBool("GuestPlayTrigger", false);
        guestCanvas.alpha = 0;
    }

    public void NoButton()
    {
        MainMenuTransition.instance.animator.SetBool("GuestPlayTrigger", false);
        MainMenuTransition.instance.BackMainMenuPlay();
        guestCanvas.alpha = 0;
    }
}
