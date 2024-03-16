using Firebase.Database;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.Windows;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Firebase;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager instance;
    // User data file
    private string userDataFilePath = "userdata.txt";
    [Tooltip("The username of the user")]
    public string username;

    [Header("------- Register -------")]
    [Header("------- Fields -------")]
    [Tooltip("The Field for the username in the register menu")]
    public InputField RegisterUsernameField;
    [Tooltip("The Field for the email in the register menu")]
    public InputField RegisterEmailField;
    [Tooltip("The Field for the password in the register menu")]
    public InputField RegisterPasswordField;
    [Tooltip("The Field for the confirm password in the register menu")]
    public InputField RegisterConfirmPasswordField;

    [Header("------- Error messages -------")]
    [Tooltip("The error for the username field")]
    public Text RegisterUsernameErrorMessage;
    [Tooltip("The error for the email field")]
    public Text RegisterEmailErrorMessage;
    [Tooltip("The error for the password field")]
    public Text RegisterPasswordErrorMessage;
    [Tooltip("The error for the confirm password field")]
    public Text RegisterConfirmPasswordErrorMessage;

    [Header("------- Canvas Groups -------")]
    [Tooltip("The Canvas Group for the username field")]
    public CanvasGroup RegisterUsernameErrorCanvasGroup;
    [Tooltip("The Canvas Group for the email field")]
    public CanvasGroup RegisterEmailErrorCanvasGroup;
    [Tooltip("The Canvas Group for the password field")]
    public CanvasGroup RegisterPasswordErrorCanvasGroup;
    [Tooltip("The Canvas Group for the confirm password field")]
    public CanvasGroup RegisterConfirmPasswordErrorCanvasGroup;

    [Header("------- Login -------")]
    [Tooltip("The Field for the username in the login menu")]
    public InputField loginUsername;
    [Tooltip("The Field for the password in the login menu")]
    public InputField loginPassword;
    [Tooltip("The display text for the login button")]
    public Text loginButtonText;

    [Header("------- Level stats -------")]
    [Header("------- Tutorial level -------")]
    [Tooltip("The text displaying the highest score achieved in the tutorial")]
    public Text TutorialBestScoreText;
    [Tooltip("The text displaying the number of attempts made in the tutorial")]
    public Text TutorialAttemptsText;
    [Tooltip("The text displaying the highest speed achieved in the tutorial")]
    public Text TutorialBestSpeedText;
    [Tooltip("The text displaying the highest streak achieved in the tutorial")]
    public Text TutorialBestStreakText;

    [Header("------- Level 1 -------")]
    [Tooltip("The text displaying the highest score achieved in level 1")]
    public Text Level1BestScoreText;
    [Tooltip("The text displaying the number of attempts made in level 1")]
    public Text Level1AttemptsText;
    [Tooltip("The text displaying the highest speed achieved in level 1")]
    public Text Level1BestSpeedText;
    [Tooltip("The text displaying the highest streak achieved in level 1")]
    public Text Level1BestStreakText;

    public DatabaseReference databaseReference;

    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            // Set the instance to this GameManager if it's the first one.
            instance = this;
        }
        else
        {
            // Destroy the existing instance if a new one is detected.
            Destroy(instance.gameObject);
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);

        databaseReference = FirebaseDatabase.DefaultInstance.RootReference; // Set the database reference to the root reference of Firebase

        // Check if the user data file exists
        if (!System.IO.File.Exists(userDataFilePath))
        {
            // If not, create an empty user data file
            System.IO.File.WriteAllText(userDataFilePath, "");
        }
        else
        {
            // If yes, read the user data from the file
            string userData = System.IO.File.ReadAllText(userDataFilePath);
            string[] userDataLines = userData.Split('\n');

            // Iterate through each line of user data
            foreach (string userDataLine in userDataLines)
            {
                // Split the line into username and hashed password
                string[] userDataParts = userDataLine.Split(':');
                if (userDataParts.Length == 2)
                {
                    // Extract username and hashed password
                    string username = userDataParts[0];
                    string hashedPassword = userDataParts[1].Trim();

                    // Check user data on startup
                    StartCoroutine(CheckUserDataOnStartup(username, hashedPassword));
                }
            }
        }
    }

    // Load statistics for all levels
    public void LoadEveryLevelStats()
    {
        if (!Guest.instance.guest)
        {
            StartCoroutine(LoadLevelStats(username, "TutorialLevel", TutorialBestScoreText, TutorialAttemptsText, TutorialBestSpeedText, TutorialBestStreakText));
            StartCoroutine(LoadLevelStats(username, "Level 1", Level1BestScoreText, Level1AttemptsText, Level1BestSpeedText, Level1BestStreakText));
        }
    }

    // Save user data to file
    private void SaveUserData(string username, string hashedPassword)
    {
        string userData = $"{username}:{hashedPassword}\n";
        this.username = username;
        System.IO.File.AppendAllText(userDataFilePath, userData);
    }

    // Delete user data
    private void DeleteUserData()
    {
        System.IO.File.WriteAllText(userDataFilePath, "");
        username = null;

        // Reset level statistics texts
        TutorialBestScoreText.text = "Best Score: 0";
        TutorialAttemptsText.text = "Attempts: 0";
        TutorialBestSpeedText.text = "Best Speed: 0%";
        TutorialBestStreakText.text = "Best Streak: 0";

        Level1BestScoreText.text = "Best Score: 0";
        Level1AttemptsText.text = "Attempts: 0";
        Level1BestSpeedText.text = "Best Speed: 0%";
        Level1BestStreakText.text = "Best Streak: 0";
    }

    // Coroutine to check user data during startup
    private IEnumerator CheckUserDataOnStartup(string username, string enteredPassword)
    {
        // Asynchronously retrieve user data from the database
        var userData = databaseReference.Child("Users").Child(username).Child("Authentication").GetValueAsync();

        // Set the username
        this.username = username;

        // Wait until the data retrieval task is completed
        yield return new WaitUntil(() => userData.IsCompleted);

        // Check for any exceptions during data retrieval
        if (userData.Exception != null)
        {
            // Log an error if data retrieval failed
            Debug.LogError("Failed to retrieve user data: " + userData.Exception.Message);
            yield break;
        }

        // Get the data snapshot
        DataSnapshot snapshot = userData.Result;

        // Check if the snapshot exists
        if (snapshot != null && snapshot.Exists)
        {
            // Get the stored password from the snapshot
            string storedPassword = snapshot.Child("password").Value.ToString();

            // Compare stored password with the entered password
            if (storedPassword == enteredPassword)
            {
                // Update UI and load user settings if passwords match
                loginButtonText.text = "Logout";
                Guest.instance.LoginAs.text = username;
                StartCoroutine(LoadUserSettings(username));
            }
            else
            {
                // Log a warning if passwords don't match
                Debug.LogWarning("Stored password does not match entered password for user: " + username);
            }
        }
        else
        {
            // Log a warning if user doesn't exist in the database
            Debug.LogWarning("User does not exist in the database: " + username);
        }
    }

    // Create user
    // Method to create a new user
    public void CreateUser()
    {
        // Check if all fields are valid
        if (Validations.instance.RegisterButton())
        {
            // Query the database to check if the username already exists
            var checkUsernameTask = databaseReference.Child("Users").Child(RegisterUsernameField.text).Child("Authentication").GetValueAsync();

            // Start a coroutine to wait for the task completion
            StartCoroutine(WaitForTaskCompletion(checkUsernameTask));
        }
        else
        {
            // Handle validation errors for username, email, password, and confirm password fields
            if (!Validations.instance.ValidateUsername())
            {
                // Username validation
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

            // Email validation
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

            // Password validation
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

            // Confirm password validation
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

    // Wait for task completion
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
            // If username doesn't exist, proceed with user creation
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
            username = enteredUsername;

            // Create new user in Firebase database
            User newUser = new User(RegisterUsernameField.text, RegisterEmailField.text, hashedPassword);
            string json = JsonUtility.ToJson(newUser);

            // Save user data to local file
            databaseReference.Child("Users").Child(username).Child("Authentication").SetRawJsonValueAsync(json);
            SaveUserData(RegisterUsernameField.text, PasswordHashSystem.HashPassword(RegisterPasswordField.text));

            // Return to authentication screen
            Authentication.instance.RegisterReturnButton();
            Authentication.instance.LoginReturnButton();
        }
    }

    // Method to log in a user
    public void LoginUser()
    {
        // Delete any existing user data
        DeleteUserData();

        // Get the entered username and hashed password
        string enteredUsername = loginUsername.text;
        string hashedPassword = PasswordHashSystem.HashPassword(loginPassword.text);

        // Start coroutines to get user data and load user settings
        StartCoroutine(GetUserAndPassword(enteredUsername, hashedPassword));
        StartCoroutine(LoadUserSettings(enteredUsername));
        LoadEveryLevelStats();
    }

    // Load user settings
    // Coroutine to load user settings from the database
    private IEnumerator LoadUserSettings(string username)
    {
        // Retrieve user settings asynchronously
        var userSettings = databaseReference.Child("Users").Child(username).Child("Settings").GetValueAsync();
        yield return new WaitUntil(() => userSettings.IsCompleted);

        // Check for any exceptions during data retrieval
        if (userSettings.Exception != null)
        {
            // Log an error if data retrieval failed
            Debug.LogError("Failed to retrieve user settings: " + userSettings.Exception.Message);
            yield break;
        }

        // Get the settings snapshot
        DataSnapshot settingsSnapshot = userSettings.Result;

        // Check if the settings snapshot exists
        if (settingsSnapshot != null && settingsSnapshot.Exists)
        {
            // Retrieve display settings
            var displaySnapshot = settingsSnapshot.Child("Display");
            if (displaySnapshot.Exists)
            {
                // Retrieve fullscreen setting
                var fullscreenSnapshot = displaySnapshot.Child("Fullscreen");
                bool fullscreenSetting = (bool)fullscreenSnapshot.Value;

                // Retrieve resolution setting
                var resolutionSnapshot = displaySnapshot.Child("Resolution");
                string resolutionSetting = (string)resolutionSnapshot.Value;

                // Retrieve VSync setting
                var vsyncSnapshot = displaySnapshot.Child("VSync");
                bool vsyncSetting = (bool)vsyncSnapshot.Value;

                // Loop through resolutions to find the index matching resolutionSetting
                int resolutionIndex = -1;
                for (int i = 0; i < SettingsMenu.instance.resolutions.Length; i++)
                {
                    string resolutionOption = SettingsMenu.instance.resolutions[i].width + "x" + SettingsMenu.instance.resolutions[i].height;
                    if (resolutionOption == resolutionSetting)
                    {
                        resolutionIndex = i;
                        break;
                    }
                }

                // Apply the found resolution index to the dropdown
                if (resolutionIndex != -1)
                {
                    SettingsMenu.instance.resolutionsDropdown.value = resolutionIndex;
                    SettingsMenu.instance.resolutionsDropdown.RefreshShownValue();
                }

                // Apply other display settings
                SettingsMenu.instance.fullscreenToggle.isOn = fullscreenSetting;
                SettingsMenu.instance.VSyncToggle.isOn = vsyncSetting;
            }
            SettingsMenu.instance.LoadVolumeSettings();
        }
    }

    // Method to logout a user
    public void LogoutUser()
    {
        // Clear the logged-in user's information
        Guest.instance.LoginAs.text = "Login as Guest";
        loginButtonText.text = "Login";
        Authentication.instance.animator.SetBool("login", true);

        // Delete user data and hide error messages
        DeleteUserData();
        HideErrorMessage();
    }

    void ShowErrorMessage(string message)
    {
        Authentication.instance.ErrorMessage.alpha = 1;
        Authentication.instance.ErrorMessage.interactable = true;
        Authentication.instance.ErrorMessage.blocksRaycasts = true;
        // Show the error message text in your Canvas Group
    }

    void HideErrorMessage()
    {
        // Hide the error message text in your Canvas Group
        Authentication.instance.ErrorMessage.alpha = 0;
        Authentication.instance.ErrorMessage.interactable = false;
        Authentication.instance.ErrorMessage.blocksRaycasts = false;
    }

    // Coroutine to get user data and check password during login
    private IEnumerator GetUserAndPassword(string username, string enteredPassword)
    {
        // Retrieve user data asynchronously
        var userData = databaseReference.Child("Users").Child(username).Child("Authentication").GetValueAsync();

        // Wait until the data retrieval task is completed
        yield return new WaitUntil(() => userData.IsCompleted);

        // Check for any exceptions during data retrieval
        if (userData.Exception != null)
        {
            // Log an error if data retrieval failed
            Debug.LogError("Failed to retrieve user data: " + userData.Exception.Message);
            yield break;
        }

        // Get the data snapshot
        DataSnapshot snapshot = userData.Result;

        // Check if the snapshot exists
        if (snapshot != null && snapshot.Exists)
        {
            // Get the stored password from the snapshot
            string storedPassword = snapshot.Child("password").Value.ToString();

            // Compare stored password with the entered password
            if (storedPassword == enteredPassword)
            {
                // Log in the user if passwords match
                Authentication.instance.animator.SetBool("login", false);
                Guest.instance.LoginAs.text = username;
                loginButtonText.text = "Logout";
                MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", false);
                DeleteUserData();
                SaveUserData(loginUsername.text, PasswordHashSystem.HashPassword(loginPassword.text));
            }
            else
            {
                // Show error message for incorrect username or password
                ShowErrorMessage("Incorrect username or password!");
            }
        }
        else
        {
            // Show error message for non-existing user
            ShowErrorMessage("User does not exist!");
        }
    }

    // Coroutine to load level statistics for a user
    private IEnumerator LoadLevelStats(string username, string levelName, Text bestScoreText, Text attemptsText, Text bestSpeedText, Text bestStreakText)
    {
        // Retrieve level statistics asynchronously
        var levelStats = databaseReference.Child("Users").Child(username).Child("Levels").Child(levelName).GetValueAsync();
        yield return new WaitUntil(() => levelStats.IsCompleted);

        // Check for any exceptions during data retrieval
        if (levelStats.Exception != null)
        {
            // Log an error if data retrieval failed
            Debug.LogError("Failed to retrieve level stats for " + levelName + ": " + levelStats.Exception.Message);
            yield break;
        }

        // Get the stats snapshot
        DataSnapshot statsSnapshot = levelStats.Result;

        // Check if the stats snapshot exists
        if (statsSnapshot != null && statsSnapshot.Exists)
        {
            // Retrieve best score, attempts, best speed, and best streak from the snapshot
            int bestScore = statsSnapshot.Child("BestScore").Exists ? int.Parse(statsSnapshot.Child("BestScore").Value.ToString()) : 0;
            int attempts = statsSnapshot.Child("Attempts").Exists ? int.Parse(statsSnapshot.Child("Attempts").Value.ToString()) : 0;
            float bestSpeed = statsSnapshot.Child("BestSpeed").Exists ? float.Parse(statsSnapshot.Child("BestSpeed").Value.ToString()) : 0;
            int bestStreak = statsSnapshot.Child("BestStreak").Exists ? int.Parse(statsSnapshot.Child("BestStreak").Value.ToString()) : 0;

            // Update UI with retrieved statistics
            bestScoreText.text = $"Best Score: {bestScore}";
            attemptsText.text = $"Attempts: {attempts}";
            bestSpeedText.text = $"Best Speed: {bestSpeed}%";
            bestStreakText.text = $"Best Streak: {bestStreak}";
        }
        else
        {
            // Log a warning if data for the level is not found
            Debug.LogWarning(levelName + " data not found for user: " + username);
        }
    }
}