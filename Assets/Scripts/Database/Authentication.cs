using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    public static Authentication instance;

    [Header("------- Animaton -------")]
    [Tooltip("The animator for the authentication menu")]
    public Animator animator;

    [Header("------- Error message -------")]
    [Tooltip("The error message displayed for validations")]
    public CanvasGroup ErrorMessage;

    void Start()
    {
        instance = this;
    }

    // Login button
    public void LoginButton()
    {
        if (Guest.instance.guest == true)
        {
            MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", true);
        }
        else
        {
            // Login out the user if he is loget in
            DatabaseManager.instance.LogoutUser();
            Guest.instance.guest = true;
        }
    }

    // Return to the main menu
    public void LoginReturnButton()
    {
        MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", false);
    }

    // Enter the register menu
    public void RegisterButton()
    {
        animator.SetBool("register", true);
    }

    // Return to the login menu
    public void RegisterReturnButton()
    {
        animator.SetBool("register", false);
    }
}