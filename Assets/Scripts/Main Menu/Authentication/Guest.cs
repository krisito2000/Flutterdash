using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest : MonoBehaviour
{
    public static Guest instance;

    [Header("------- Canvas -------")]
    [Tooltip("Canvas group for the guest interface")]
    public CanvasGroup guestCanvas;

    // Text indicating whether logged in as guest
    [Header("------- Is it Guest -------")]
    [Tooltip("This text displays the username you are logged in with.")]
    public Text LoginAs;
    [Tooltip("Indication whether you are logged in as guest")]
    public bool guest;

    public void Awake()
    {
        guest = true;

        if (instance == null)
            instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if logged in as guest
        if (LoginAs.text == "Login as Guest")
        {
            guest = true; // Set guest bool to true
            MainMenuTransition.instance.animator.SetBool("isGuest", true); // Set animation parameter for guest
        }
        else
        {
            guest = false; // Set guest bool to false
            MainMenuTransition.instance.animator.SetBool("isGuest", false); // Set animation parameter for non-guest
        }
    }

    // Method invoked when the Yes button is pressed in the guest menu
    public void YesButton()
    {
        MainMenuTransition.instance.animator.SetBool("GuestPlayTrigger", false); // Set animation parameter for guest play
        guestCanvas.alpha = 0; // Hide the guest canvas
    }

    // Method invoked when the No button is pressed in the guest menu
    public void NoButton()
    {
        MainMenuTransition.instance.animator.SetBool("GuestPlayTrigger", false); // Set animation parameter for non-guest play
        MainMenuTransition.instance.BackMainMenuPlay(); // Return to the main menu
        guestCanvas.alpha = 0; // Hide the guest canvas
    }
}
