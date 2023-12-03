using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using System.Security.Cryptography;
using System.Text;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    public InputField registerUsername;
    public InputField registerEmail;
    public InputField registerPassword;
    public InputField registerConfirmPassword;

    public InputField loginUsername;
    public InputField loginPassword;
    public Text loginButtonText;

    private DatabaseReference databaseReference;

    private void Start()
    {
        instance = this;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void CreateUser()
    {
        string hashedPassword = PasswordHashSystem.HashPassword(registerPassword.text);

        User newUser = new User(registerUsername.text, registerEmail.text, hashedPassword);
        string json = JsonUtility.ToJson(newUser);

        databaseReference.Child("Users").Child(newUser.username).Child("Authentication").SetRawJsonValueAsync(json);
    }

    public void LoginUser()
    {
        string enteredUsername = loginUsername.text;
        string enteredPassword = PasswordHashSystem.HashPassword(loginPassword.text);
        loginButtonText.text = "Logout";

        StartCoroutine(GetUserAndPassword(enteredUsername, enteredPassword));
    }

    public void LogoutUser()
    {
        // Clear the logged-in user's information
        Authentication.instance.LoginAs.text = "Login as Guest";
        loginButtonText.text = "Login";
        Authentication.instance.animator.SetBool("login", true);
        Authentication.instance.inAuthentication = true;

        // Hide the error message if it's shown
        HideErrorMessage();
    }

    void HideErrorMessage()
    {
        Authentication.instance.ErrorMessage.alpha = 0;
        Authentication.instance.ErrorMessage.interactable = false;
        Authentication.instance.ErrorMessage.blocksRaycasts = false;
        // Hide the error message text in your Canvas Group
    }

    private IEnumerator GetUserAndPassword(string username, string enteredPassword)
    {
        var userData = databaseReference.Child("Users").Child(username).Child("Authentication").GetValueAsync();

        yield return new WaitUntil(() => userData.IsCompleted);

        if (userData.Exception != null)
        {
            Debug.LogError("Failed to retrieve user data: " + userData.Exception.Message);
            yield break;
        }

        DataSnapshot snapshot = userData.Result;

        if (snapshot != null && snapshot.Exists)
        {
            string storedPassword = snapshot.Child("password").Value.ToString();

            if (storedPassword == enteredPassword)
            {
                Authentication.instance.animator.SetBool("login", false);
                Authentication.instance.inAuthentication = false;
                Authentication.instance.LoginAs.text = username;
            }
            else
            {
                ShowErrorMessage("Incorrect username or password!");
            }
        }
        else
        {
            ShowErrorMessage("User does not exist!");
        }
    }

    void ShowErrorMessage(string message)
    {
        Authentication.instance.ErrorMessage.alpha = 1;
        Authentication.instance.ErrorMessage.interactable = true;
        Authentication.instance.ErrorMessage.blocksRaycasts = true;
        // Show the error message text in your Canvas Group
    }

}
