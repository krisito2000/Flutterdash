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
    private string userDataFilePath = "userdata.txt";

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

    [Header("------- Level stats -------")]
    [Header("------- Tutorial level -------")]
    public Text TutorialBestScoreText;
    public Text TutorialBestSpeedText;
    public Text TutorialBestStreakText;

    [Header("------- Level 1 -------")]
    public Text Level1BestScoreText;
    public Text Level1BestSpeedText;
    public Text Level1BestStreakText;

    public DatabaseReference databaseReference;

    private void Start()
    {
        instance = this;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

        if (!System.IO.File.Exists(userDataFilePath))
        {
            System.IO.File.WriteAllText(userDataFilePath, "");
        }
        else
        {
            string userData = System.IO.File.ReadAllText(userDataFilePath);
            string[] userDataLines = userData.Split('\n');

            foreach (string userDataLine in userDataLines)
            {
                string[] userDataParts = userDataLine.Split(':');
                if (userDataParts.Length == 2)
                {
                    string username = userDataParts[0];
                    string hashedPassword = userDataParts[1].Trim();

                    StartCoroutine(CheckUserDataOnStartup(username, hashedPassword));
                }
            }
        }

        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            // Initialize the Firebase Realtime Database
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

            // Retrieve data for the tutorial level
            RetrieveLevelData("TutorialLevel", (tutorialDataSnapshot) =>
            {
                // Handle the retrieved data for the tutorial level
                if (tutorialDataSnapshot != null && tutorialDataSnapshot.Exists)
                {
                    // Retrieve best score, best speed, and best streak from the snapshot
                    int tutorialBestScore = tutorialDataSnapshot.Child("BestScore").Exists ? int.Parse(tutorialDataSnapshot.Child("BestScore").Value.ToString()) : 0;
                    float tutorialBestSpeed = tutorialDataSnapshot.Child("BestSpeed").Exists ? float.Parse(tutorialDataSnapshot.Child("BestSpeed").Value.ToString()) : 0;
                    int tutorialBestStreak = tutorialDataSnapshot.Child("BestStreak").Exists ? int.Parse(tutorialDataSnapshot.Child("BestStreak").Value.ToString()) : 0;

                    // Update UI or perform other actions with the retrieved data
                    // For example:
                    TutorialBestScoreText.text = tutorialBestScore.ToString();
                    TutorialBestSpeedText.text = tutorialBestSpeed.ToString();
                    TutorialBestStreakText.text = tutorialBestStreak.ToString();
                }
                else
                {
                    Debug.LogWarning("Tutorial level data not found.");
                }
            });

            // Retrieve data for Level 1
            RetrieveLevelData("Level1", (level1DataSnapshot) =>
            {
                // Handle the retrieved data for Level 1
                if (level1DataSnapshot != null && level1DataSnapshot.Exists)
                {
                    // Retrieve best score, best speed, and best streak from the snapshot
                    int level1BestScore = level1DataSnapshot.Child("BestScore").Exists ? int.Parse(level1DataSnapshot.Child("BestScore").Value.ToString()) : 0;
                    float level1BestSpeed = level1DataSnapshot.Child("BestSpeed").Exists ? float.Parse(level1DataSnapshot.Child("BestSpeed").Value.ToString()) : 0;
                    int level1BestStreak = level1DataSnapshot.Child("BestStreak").Exists ? int.Parse(level1DataSnapshot.Child("BestStreak").Value.ToString()) : 0;

                    // Update UI or perform other actions with the retrieved data
                    // For example:
                    Level1BestScoreText.text = level1BestScore.ToString();
                    Level1BestSpeedText.text = level1BestSpeed.ToString();
                    Level1BestStreakText.text = level1BestStreak.ToString();
                }
                else
                {
                    Debug.LogWarning("Level 1 data not found.");
                }
            });
        });
    }
    private void Update()
    {
        
    }

    private void SaveUserData(string username, string hashedPassword)
    {
        string userData = $"{username}:{hashedPassword}\n";
        System.IO.File.AppendAllText(userDataFilePath, userData);
    }

    private void DeleteUserData()
    {
        System.IO.File.WriteAllText(userDataFilePath, "");
    }
    private IEnumerator CheckUserDataOnStartup(string username, string enteredPassword)
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
                loginButtonText.text = "Logout";
                Guest.instance.LoginAs.text = username;
                StartCoroutine(LoadUserSettings(username));
            }
            else
            {
                // Password doesn't match
                Debug.LogWarning("Stored password does not match entered password for user: " + username);
            }
        }
        else
        {
            // User does not exist in the database
            Debug.LogWarning("User does not exist in the database: " + username);
        }
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

            // Email
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

            // Password
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
            SaveUserData(RegisterUsernameField.text, PasswordHashSystem.HashPassword(RegisterPasswordField.text));

            Authentication.instance.RegisterReturnButton();
            Authentication.instance.LoginReturnButton();
        }
    }


    public void LoginUser()
    {
        DeleteUserData();
        string enteredUsername = loginUsername.text;
        string hashedPassword = PasswordHashSystem.HashPassword(loginPassword.text);

        StartCoroutine(GetUserAndPassword(enteredUsername, hashedPassword));
        StartCoroutine(LoadUserSettings(enteredUsername));
    }

    private IEnumerator LoadUserSettings(string username)
    {
        var userSettings = databaseReference.Child("Users").Child(username).Child("Settings").GetValueAsync();
        yield return new WaitUntil(() => userSettings.IsCompleted);

        if (userSettings.Exception != null)
        {
            Debug.LogError("Failed to retrieve user settings: " + userSettings.Exception.Message);
            yield break;
        }

        DataSnapshot settingsSnapshot = userSettings.Result;

        if (settingsSnapshot != null && settingsSnapshot.Exists)
        {
            var displaySnapshot = settingsSnapshot.Child("Display");
            if (displaySnapshot.Exists)
            {
                var fullscreenSnapshot = displaySnapshot.Child("Fullscreen");
                bool fullscreenSetting = (bool)fullscreenSnapshot.Value;

                var resolutionSnapshot = displaySnapshot.Child("Resolution");
                string resolutionSetting = (string)resolutionSnapshot.Value;

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

                if (resolutionIndex != -1)
                {
                    // Apply the found resolution index to the dropdown
                    SettingsMenu.instance.resolutionsDropdown.value = resolutionIndex;
                    SettingsMenu.instance.resolutionsDropdown.RefreshShownValue();
                }

                // Apply other display settings as needed
                SettingsMenu.instance.fullscreenToggle.isOn = fullscreenSetting;
                SettingsMenu.instance.VSyncToggle.isOn = vsyncSetting;
            }

            // Volume setting
            var volumeSnapshot = settingsSnapshot.Child("Volume");
            if (volumeSnapshot.Exists)
            {
                var masterSnapshot = volumeSnapshot.Child("Master");
                var musicSnapshot = volumeSnapshot.Child("Music");
                var hitSoundSnapshot = volumeSnapshot.Child("HitSound");

                // Extract values from the snapshots
                float masterVolumeSetting = float.Parse(masterSnapshot.Value.ToString());
                float musicVolumeSetting = float.Parse(musicSnapshot.Value.ToString());
                float hitSoundVolumeSetting = float.Parse(hitSoundSnapshot.Value.ToString());

                // Check if SettingsMenu.instance is not null before accessing members
                if (SettingsMenu.instance != null)
                {
                    // Check if masterSlider is not null before accessing its members
                    if (SettingsMenu.instance.masterSlider != null)
                    {
                        // Apply the loaded volume setting to the master slider
                        SettingsMenu.instance.masterSlider.value = masterVolumeSetting;

                        // Call the respective method to set the volume in SettingsMenu
                        SettingsMenu.instance.SetMasterVolume(masterVolumeSetting);
                    }
                    else
                    {
                        Debug.LogError("masterSlider is null in SettingsMenu.");
                    }

                    // Check if musicSlider is not null before accessing its members
                    if (SettingsMenu.instance.musicSlider != null)
                    {
                        // Apply the loaded volume setting to the music slider
                        SettingsMenu.instance.musicSlider.value = musicVolumeSetting;

                        // Call the respective method to set the volume in SettingsMenu
                        SettingsMenu.instance.SetMusicVolume(musicVolumeSetting);
                    }
                    else
                    {
                        Debug.LogError("musicSlider is null in SettingsMenu.");
                    }

                    // Check if hitSoundSlider is not null before accessing its members
                    if (SettingsMenu.instance.hitSoundSlider != null)
                    {
                        // Apply the loaded volume setting to the hitSound slider
                        SettingsMenu.instance.hitSoundSlider.value = hitSoundVolumeSetting;

                        // Call the respective method to set the volume in SettingsMenu
                        SettingsMenu.instance.SetHitSoundVolume(hitSoundVolumeSetting);
                    }
                    else
                    {
                        Debug.LogError("hitSoundSlider is null in SettingsMenu.");
                    }
                }
                else
                {
                    Debug.LogError("SettingsMenu.instance is null.");
                }
            }
        }
    }


    public void LogoutUser()
    {
        // Clear the logged-in user's information
        Guest.instance.LoginAs.text = "Login as Guest";
        loginButtonText.text = "Login";
        Authentication.instance.animator.SetBool("login", true);

        DeleteUserData();
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
                loginButtonText.text = "Logout";
                MainMenuTransition.instance.animator.SetBool("AuthenticationTrigger", false);
                DeleteUserData();
                SaveUserData(loginUsername.text, PasswordHashSystem.HashPassword(loginPassword.text));
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

    void RetrieveLevelData(string levelName, System.Action<DataSnapshot> callback)
    {
        // Get a reference to the location in the database where level data is stored
        DatabaseReference levelRef = databaseReference.Child("Users")
                                                     .Child(Guest.instance.LoginAs.text)
                                                     .Child("Levels")
                                                     .Child(levelName);

        // Retrieve the level data asynchronously
        levelRef.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsFaulted)
            {
                // Handle the error
                Debug.LogError("Failed to retrieve data for " + levelName + ": " + task.Exception);
                callback(null);
                return;
            }

            // Retrieve the data snapshot
            DataSnapshot snapshot = task.Result;

            // Pass the data snapshot to the callback function
            callback(snapshot);
        });
    }
}