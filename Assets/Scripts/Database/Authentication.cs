using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    public static Authentication instance;

    [Header("------- Animaton -------")]
    public Animator animator;

    [Header("------- Error message -------")]
    public CanvasGroup ErrorMessage;

    void Start()
    {
        instance = this;
    }

    public void LoginButton()
    {
        if (Guest.instance.guest == true)
        {
            MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", true);
        }
        else
        {
            DatabaseManager.instance.LogoutUser();
            Guest.instance.guest = true;
        }
    }
    public void LoginReturnButton()
    {
        MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", false);
    }
    public void RegisterButton()
    {
        animator.SetBool("register", true);
    }
    public void RegisterReturnButton()
    {
        animator.SetBool("register", false);
    }
}