using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Authentication : MonoBehaviour
{
    public static Authentication instance;

    public Animator animator;
    public bool inAuthentication;
    public Text LoginAs;
    public CanvasGroup ErrorMessage;

    void Start()
    {
        instance = this;
    }

    public void LoginButton()
    {
        if (LoginAs.text == "Login as Guest")
        {
            animator.SetBool("login", true);
            inAuthentication = true;
        }
        else
        {
            DatabaseManager.instance.LogoutUser();
            DatabaseManager.instance.loginButtonText.text = "Logout";
        }
    }
    public void LoginReturnButton()
    {
        animator.SetBool("login", false);
        inAuthentication = false;
    }
    public void RegisterButton()
    {
        animator.SetBool("register", true);
        inAuthentication = true;
    }
    public void RegisterReturnButton()
    {
        animator.SetBool("register", false);
        inAuthentication = true;
    }
}