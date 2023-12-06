using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;

    [Header("------- Register -------")]
    [Header("------- Fields -------")]
    public InputField RegisterUsernameField;
    public InputField RegisterEmailField;
    public InputField RegisterPasswordField;
    public InputField RegisterConfirmPasswordField;

    [Header("------- Error messages -------")]
    public Text RegisterUsernameErrorMessage;
    public Text RegisterEmailErrorMessage;
    public Text RegisterPasswordErrorMessage;
    public Text RegisterConfirmPasswordErrorMessage;

    [Header("------- Canvas Groups -------")]
    public CanvasGroup RegisterUsernameErrorCanvasGroup;
    public CanvasGroup RegisterEmailErrorCanvasGroup;
    public CanvasGroup RegisterPasswordErrorCanvasGroup;
    public CanvasGroup RegisterConfirmPasswordErrorCanvasGroup;

    [Header("------- Login -------")]
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
        if (Validations.instance.RegisterButton())
        {
            // Query the database to check if the username already exists
            var checkUsernameTask = databaseReference.Child("Users").Child(RegisterUsernameField.text).Child("Authentication").GetValueAsync();

            StartCoroutine(WaitForTaskCompletion(checkUsernameTask));
        }
        else
        {
            // Username
            if (!Validations.instance.ValidateUsername())
            {
                if (string.IsNullOrEmpty(RegisterUsernameField.text))
                {
                    RegisterUsernameErrorMessage.text = "Username cannot be empty.";
                    RegisterUsernameErrorCanvasGroup.alpha = 1;
                }
                else if (RegisterUsernameField.text.Length > 32)
                {
                    RegisterUsernameErrorMessage.text = "Username maximum character length cannot exceed 32 characters.";
                    RegisterUsernameErrorCanvasGroup.alpha = 1;
                }
                else
                {
                    RegisterUsernameErrorMessage.text = "Username should be at least 6 or more characters.";
                    RegisterUsernameErrorCanvasGroup.alpha = 1;
                }
            }

            //Email
            if (!Validations.instance.ValidateEmail())
            {
                if (string.IsNullOrEmpty(RegisterEmailField.text))
                {
                    RegisterEmailErrorMessage.text = "Email cannot be empty.";
                    RegisterEmailErrorCanvasGroup.alpha = 1;
                }
                else
                {
                    RegisterEmailErrorMessage.text = "Invalid email format.";
                    RegisterEmailErrorCanvasGroup.alpha = 1;
                }
            }

            //Password
            if (!Validations.instance.ValidatePassword())
            {
                if (RegisterPasswordField.text.Length < 6)
                {
                    RegisterPasswordErrorMessage.text = "Password should be at least 6 characters.";
                    RegisterPasswordErrorCanvasGroup.alpha = 1;
                }
                else if (string.IsNullOrEmpty(RegisterPasswordField.text))
                {
                    RegisterPasswordErrorMessage.text = "Password cannot be empty.";
                    RegisterPasswordErrorCanvasGroup.alpha = 1;
                }
            }

            // Confirm Password
            if (!Validations.instance.ValidateConfirmPassword())
            {
                RegisterConfirmPasswordErrorMessage.text = "Password and confirm password does not match";
                RegisterConfirmPasswordErrorCanvasGroup.alpha = 1;
            }
            else
            {
                RegisterConfirmPasswordErrorMessage.text = "";
                RegisterConfirmPasswordErrorCanvasGroup.alpha = 0;
            }
        }
    }

    private IEnumerator WaitForTaskCompletion(Task<DataSnapshot> task)
    {
        while (!task.IsCompleted)
        {
            yield return null; // Wait until the task is completed
        }

        if (task.Exception != null)
        {
            // Handle errors or failed database access
            Debug.LogError("Failed to check username existence: " + task.Exception);
            yield break;
        }

        DataSnapshot snapshot = task.Result;

        // Check if the task completed and modify UI on the main thread
        if (snapshot != null && snapshot.Exists)
        {
            RegisterUsernameErrorMessage.text = "Username already exists.";
            RegisterUsernameErrorCanvasGroup.alpha = 1;
        }
        else
        {
            RegisterUsernameErrorMessage.text = "";
            RegisterEmailErrorMessage.text = "";
            RegisterPasswordErrorMessage.text = "";
            RegisterPasswordErrorMessage.text = "";
            RegisterConfirmPasswordErrorMessage.text = "";
            RegisterUsernameErrorCanvasGroup.alpha = 0;
            RegisterEmailErrorCanvasGroup.alpha = 0;
            RegisterPasswordErrorCanvasGroup.alpha = 0;
            RegisterConfirmPasswordErrorCanvasGroup.alpha = 0;

            string enteredUsername = RegisterUsernameField.text;
            string hashedPassword = PasswordHashSystem.HashPassword(RegisterPasswordField.text);
            loginButtonText.text = "Logout";
            Guest.instance.LoginAs.text = enteredUsername;

            User newUser = new User(RegisterUsernameField.text, RegisterEmailField.text, hashedPassword);
            string json = JsonUtility.ToJson(newUser);

            databaseReference.Child("Users").Child(newUser.username).Child("Authentication").SetRawJsonValueAsync(json);

            Authentication.instance.RegisterReturnButton();
            Authentication.instance.LoginReturnButton();
        }
    }


    public void LoginUser()
    {
        string enteredUsername = loginUsername.text;
        string hashedPassword = PasswordHashSystem.HashPassword(loginPassword.text);
        loginButtonText.text = "Logout";

        StartCoroutine(GetUserAndPassword(enteredUsername, hashedPassword));
    }

    public void LogoutUser()
    {
        // Clear the logged-in user's information
        Guest.instance.LoginAs.text = "Login as Guest";
        loginButtonText.text = "Login";
        Authentication.instance.animator.SetBool("login", true);

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
                Guest.instance.LoginAs.text = username;
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
