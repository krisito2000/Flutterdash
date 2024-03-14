using Firebase.Database;
using Google.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance = null;

    [Header("------- Canvas Groups -------")]
    [Tooltip("The canvas group for the general menu")]
    public CanvasGroup GeneralCanvasGroup;

    [Tooltip("The canvas group for the input menu")]
    public CanvasGroup InputCanvasGroup;

    [Tooltip("The canvas group for the audio menu")]
    public CanvasGroup AudioCanvasGroup;

    [Tooltip("The canvas group for the display menu")]
    public CanvasGroup DisplayCanvasGroup;

    [Header("------- Animations -------")]
    [Tooltip("The animator for the General, Input, Audio, and Display buttons")]
    public Animator ButtonAnimator;

    [Header("------- Settings -------")]
    [Header("------- Input -------")]
    [Tooltip("Mode indicating whether the camera is locked or not")]
    public bool InputLockMode;
    [Tooltip("The lock button in input section in the settings menu")]
    public GameObject lockButton;
    [Tooltip("The text for the lock button")]
    public Text lockButtonText;
    public TextMeshProUGUI duplicateBindingText;

    [Tooltip("The canvas group for the up circle")]
    public CanvasGroup UpCircleCanvasGroup;
    [Tooltip("The canvas group for the down circle")]
    public CanvasGroup DownCircleCanvasGroup;
    [Tooltip("The canvas group for the left circle")]
    public CanvasGroup LeftCircleCanvasGroup;
    [Tooltip("The canvas group for the right circle")]
    public CanvasGroup RightCircleCanvasGroup;

    [Header("------- Audio -------")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public float masterValue;
    public Slider musicSlider;
    public float musicValue;
    public Slider hitSoundSlider;
    public float hitSoundValue;

    public Text syncText;
    public Slider syncSlider;
    public UnityEvent<float> onSliderValueChanged = new UnityEvent<float>();

    [Header("------- Display -------")]
    [Tooltip("Toggles fullscreen mode")]
    public Toggle fullscreenToggle;
    [Tooltip("Dropdown menu for selecting resolutions")]
    public TMP_Dropdown resolutionsDropdown;
    public Resolution[] resolutions;
    [Tooltip("Toggles VSync mode. VSync (Vertical Synchronization) synchronizes the frame rate of the game with the refresh rate of your monitor to prevent screen tearing.")]
    public Toggle VSyncToggle;

    [System.Obsolete]
    public void Start()
    {
        // Input
        InputLockMode = false;

        // Volume

        // Display
        // Resolution
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        // Turning the array of available resolutions into a list of strings to display in the dropdown menu
        List<string> resolutionOptions = new List<string>();

        // Tracking the index of the current resolution
        int currentResolutionIndex = 0;

        // Looping through each resolution in the array
        for (int i = 0; i < resolutions.Length; i++)
        {
            // Constructing a string representation of the resolution (e.g., "1920 x 1080")
            string resolutionOption = resolutions[i].width + " x " + resolutions[i].height;

            // Adding the string to the list of resolution options
            resolutionOptions.Add(resolutionOption);

            // Checking if the current resolution matches the screen's current resolution
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                // If it matches, set the currentResolutionIndex to the index of this resolution in the array
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);

        // Set the current value of the dropdown to the current resolution index
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        // Buttons
        ButtonAnimator.SetBool("General", true);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", false);

        VSync();
    }

    // Method to load volume settings from Firebase
    public void LoadVolumeSettings()
    {
        // Check if the user is logged in
        if (Guest.instance.guest)
        {
            // If the user is a guest, log a message indicating they are not logged in
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is not a guest, retrieve the player's username
            string playerUsername = DatabaseManager.instance.username;

            // Get a reference to the volume settings node in the Firebase database
            DatabaseReference volumeSettingsRef = DatabaseManager.instance.databaseReference
                .Child("Users")
                .Child(playerUsername)
                .Child("Settings")
                .Child("Volume");

            if (!Guest.instance.guest)
            {
                // Read the volume settings from Firebase
                volumeSettingsRef.GetValueAsync().ContinueWith(task =>
                {
                    if (task.IsCompleted)
                    {
                        DataSnapshot snapshot = task.Result;

                        // Check if the snapshot exists and has children
                        if (snapshot != null && snapshot.HasChildren)
                        {
                            // Get the values of master, music, and hit sound values from the snapshot
                            masterValue = float.Parse(snapshot.Child("Master").Value.ToString());
                            musicValue = float.Parse(snapshot.Child("Music").Value.ToString());
                            hitSoundValue = float.Parse(snapshot.Child("HitSound").Value.ToString());

                            // Update the values of the sliders with the loaded values
                            CheckDatabaseMasterVolume(masterValue);
                            CheckDatabaseMusicVolume(musicValue);
                            CheckDatabaseHitSoundVolume(hitSoundValue);
                        }
                        else
                        {
                            Debug.Log("Volume settings not found in Firebase");
                        }
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.LogError("Failed to load volume settings from Firebase: " + task.Exception);
                    }
                });
            }
        }
    }

    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // General
    public void GeneralButton()
    {
        ButtonAnimator.SetBool("General", true);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", false);
    }

    // Input
    public void InputButton()
    {
        ButtonAnimator.SetBool("General", false);
        ButtonAnimator.SetBool("Input", true);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", false);
    }

    // Method to toggle the input lock mode
    public void LockButton()
    {
        // If input lock mode is currently enabled
        if (InputLockMode == true)
        {
            // Disable input lock mode
            InputLockMode = false;

            // Update the animator parameter to reflect the change in lock mode
            ButtonAnimator.SetBool("LockMode", false);

            // Update the text on the lock button to indicate it can be locked
            lockButtonText.text = "Lock";
        }
        else
        {
            // If input lock mode is currently disabled

            // Enable input lock mode
            InputLockMode = true;

            // Update the animator parameter to reflect the change in lock mode
            ButtonAnimator.SetBool("LockMode", true);

            // Update the text on the lock button to indicate it can be unlocked
            lockButtonText.text = "Unlock";
        }
    }

    // Audio
    public void AudioButton()
    {
        ButtonAnimator.SetBool("General", false);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", true);
        ButtonAnimator.SetBool("Display", false);
    }

    // Method to set the master volume level
    public void SetMasterVolume()
    {
        // Retrieve the volume value from the master slider
        float setValue = masterSlider.value;

        // Set the master volume level in the audio mixer
        audioMixer.SetFloat("Master", setValue);

        // Save the master volume setting to Firebase
        SaveMasterVolumeSetting(setValue);
    }

    public void CheckDatabaseMasterVolume(float volume)
    {
        masterSlider.value = volume;
        audioMixer.SetFloat("Master", volume);
    }

    // Method to save the master volume setting to Firebase
    public void SaveMasterVolumeSetting(float masterVolume)
    {
        // Check if the user is logged in as a guest
        if (Guest.instance.guest)
        {
            // If the user is a guest, log a message indicating they are not logged in
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is not a guest, retrieve the player's username
            string playerUsername = DatabaseManager.instance.username;

            // Set the master volume setting in the Firebase database under the user's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("Master").SetValueAsync(masterVolume);

            // Log a message indicating the master volume setting was saved to Firebase
            Debug.Log("Master volume setting saved to Firebase!");
        }
    }

    // Method to set the music volume level
    public void SetMusicVolume(float volume)
    {
        // Retrieve the volume value from the music slider
        volume = musicSlider.value;

        // Set the music volume level in the audio mixer
        audioMixer.SetFloat("Music", volume);

        // Save the music volume setting to Firebase
        SaveMusicVolumeSetting(volume);
    }

    public void CheckDatabaseMusicVolume(float volume)
    {
        musicSlider.value = volume;
        audioMixer.SetFloat("Music", volume);
    }

    // Method to save the music volume setting to Firebase
    public void SaveMusicVolumeSetting(float musicVolume)
    {
        // Check if the user is logged in as a guest
        if (Guest.instance.guest)
        {
            // If the user is a guest, log a message indicating they are not logged in
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is not a guest, retrieve the player's username
            string playerUsername = Guest.instance.LoginAs.text;

            // Set the music volume setting in the Firebase database under the user's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("Music").SetValueAsync(musicVolume);

            // Log a message indicating the music volume setting was saved to Firebase
            Debug.Log("Music volume setting saved to Firebase!");
        }
    }

    // Method to set the hit sound volume level
    public void SetHitSoundVolume(float volume)
    {
        // Retrieve the volume value from the hit sound slider
        volume = hitSoundSlider.value;

        // Set the hit sound volume level in the audio mixer
        audioMixer.SetFloat("NoteHit", volume);

        // Save the hit sound volume setting to Firebase
        SaveHitSoundVolumeSetting(volume);
    }

    public void CheckDatabaseHitSoundVolume(float volume)
    {
        hitSoundSlider.value = volume;
        audioMixer.SetFloat("NoteHit", volume);
    }

    // Method to save the hit sound volume setting to Firebase
    public void SaveHitSoundVolumeSetting(float hitSoundVolume)
    {
        // Check if the user is logged in as a guest
        if (Guest.instance.guest)
        {
            // If the user is a guest, log a message indicating they are not logged in
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is not a guest, retrieve the player's username
            string playerUsername = Guest.instance.LoginAs.text;

            // Set the hit sound volume setting in the Firebase database under the user's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("HitSound").SetValueAsync(hitSoundVolume);

            // Log a message indicating the hit sound volume setting was saved to Firebase
            Debug.Log("HitSound volume setting saved to Firebase!");
        }
    }

    // Method to set the synchronization delay
    public void SetSync(float ms)
    {
        // Retrieve the synchronization delay value from the sync slider
        ms = syncSlider.value;

        // Round down the synchronization delay value to the nearest integer
        ms = (float)Math.Floor(ms);

        // Update the synchronization text to display the synchronization delay in milliseconds
        syncText.text = $"{ms} ms";
    }


    // Display
    public void DisplayButton()
    {
        ButtonAnimator.SetBool("General", false);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", true);
    }

    // Method to set fullscreen mode
    public void SetFullscreen()
    {
        // Set the fullscreen mode based on the state of the fullscreen toggle
        Screen.fullScreen = fullscreenToggle.isOn;

        // Save the fullscreen setting to Firebase
        SaveFullscreenSetting(fullscreenToggle.isOn);
    }

    // Method to save fullscreen setting to Firebase
    public void SaveFullscreenSetting(bool isFullscreen)
    {
        // Check if the user is logged in
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            // If the user is not logged in, log a message indicating so
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is logged in, retrieve the player's username
            string playerUsername = Guest.instance.LoginAs.text;

            // Set the fullscreen setting in the Firebase database under the player's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("Fullscreen").SetValueAsync(isFullscreen);

            // Log a message indicating that the fullscreen setting has been saved to Firebase
            Debug.Log("Fullscreen setting saved to Firebase!");
        }
    }

    // Method to set resolution
    public void SetResolution(int resolutionIndex)
    {
        // Retrieve the selected resolution from the resolutions array
        Resolution resolution = resolutions[resolutionIndex];

        // Set the screen resolution using the selected resolution's width, height, and fullscreen state
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Generate a string representing the resolution data (width x height)
        string resolutionData = $"{resolution.width}x{resolution.height}";

        // Save the resolution setting to Firebase
        SaveResolutionSetting(resolutionData);
    }

    // Method to set quality
    public void SetQuality(int qualityIndex)
    {
        // Set the quality level based on the selected index
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Method to save resolution setting to Firebase
    public void SaveResolutionSetting(string resolutionData)
    {
        // Check if the user is logged in
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            // If the user is not logged in, log a message indicating so
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is logged in, retrieve the player's username
            string playerUsername = Guest.instance.LoginAs.text;

            // Set the resolution setting in the Firebase database under the player's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("Resolution").SetValueAsync(resolutionData);

            // Log a message indicating that the resolution setting has been saved to Firebase
            Debug.Log("Resolution setting saved to Firebase!");
        }
    }

    // Method to handle VSync setting
    [System.Obsolete]
    public void VSync()
    {
        // Check if VSync toggle is on
        if (VSyncToggle.isOn)
        {
            // If VSync is on, set the target frame rate to the current screen refresh rate
            var refreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = refreshRate;
        }
        else
        {
            // If VSync is off, set the target frame rate to -1 to use the platform default
            Application.targetFrameRate = -1;
        }

        // Save the VSync setting to Firebase
        SaveVSyncSetting(VSyncToggle.isOn);
    }

    // Method to save VSync setting to Firebase
    public void SaveVSyncSetting(bool isVSyncOn)
    {
        // Check if the user is logged in
        if (Guest.instance.guest)
        {
            // If the user is not logged in, log a message indicating so
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is logged in, retrieve the player's username
            string playerUsername = Guest.instance.LoginAs.text;

            // Set the VSync setting in the Firebase database under the player's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("VSync").SetValueAsync(isVSyncOn);

            // Log a message indicating that the VSync setting has been saved to Firebase
            Debug.Log("VSync setting saved to Firebase!");
        }
    }
}
