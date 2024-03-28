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
        audioMixer.SetFloat("Master", masterSlider.value);
        audioMixer.SetFloat("Music", musicSlider.value);
        audioMixer.SetFloat("NoteHit", hitSoundSlider.value);

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
    // Method to load volume settings from Firebase
    public async void LoadVolumeSettings()
    {
        // Check user login status
        if (Guest.instance.guest)
        {
            Debug.Log("User not logged in");
            return; // Exit the function early if user is a guest
        }

        string playerUsername = DatabaseManager.instance.username;

        try
        {
            // Build Firebase reference with null check
            DatabaseReference volumeSettingsRef = DatabaseManager.instance.databaseReference?.Child("Users")
                                                    .Child(playerUsername)
                                                    .Child("Settings")
                                                    .Child("Volume");

            if (volumeSettingsRef == null)
            {
                Debug.LogError("Failed to create Firebase reference for volume settings");
                return; // Exit the function if reference creation fails
            }

            // Read volume settings asynchronously
            DataSnapshot snapshot = await volumeSettingsRef.GetValueAsync();

            // Check snapshot existence and children
            if (snapshot != null && snapshot.HasChildren)
            {
                try
                {
                    masterValue = float.Parse(snapshot.Child("Master").Value.ToString());
                    musicValue = float.Parse(snapshot.Child("Music").Value.ToString());
                    hitSoundValue = float.Parse(snapshot.Child("HitSound").Value.ToString());
                }
                catch (FormatException e)
                {
                    Debug.LogError($"Error parsing volume data from Firebase: {e.Message}");
                    // Consider providing default values or informative errors here
                }

                // Update slider values
                CheckDatabaseAudioVolume(masterValue, masterSlider, "Master");
                CheckDatabaseAudioVolume(musicValue, musicSlider, "Music");
                CheckDatabaseAudioVolume(hitSoundValue, hitSoundSlider, "HitSound");

                if (!RandomMusicPlayer.instance.audioSource.isPlaying)
                {
                    RandomMusicPlayer.instance.audioSource.Play();
                }
            }
            else
            {
                Debug.Log("Volume settings not found in Firebase");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Unexpected error loading volume settings: {e.Message}");
        }
    }


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
        SaveAudioVolumeSettings(setValue, "Master");
    }

    // Method to set the music volume level
    public void SetMusicVolume()
    {
        // Retrieve the volume value from the music slider
        float setValue = musicSlider.value;

        // Set the music volume level in the audio mixer
        audioMixer.SetFloat("Music", setValue);

        // Save the music volume setting to Firebase
        SaveAudioVolumeSettings(setValue, "Music");
    }

    // Method to set the hit sound volume level
    public void SetHitSoundVolume()
    {
        // Retrieve the volume value from the hit sound slider
        float setValue = hitSoundSlider.value;

        // Set the hit sound volume level in the audio mixer
        audioMixer.SetFloat("NoteHit", setValue);

        // Save the hit sound volume setting to Firebase
        SaveAudioVolumeSettings(setValue, "HitSound");
    }

    public void CheckDatabaseAudioVolume(float volume, Slider slider, string soundType)
    {
        slider.value = volume;
        audioMixer.SetFloat(soundType, volume);
    }

    public void SaveAudioVolumeSettings(float soundVolume, string soundType)
    {
        // Check if the user is logged in as a guest
        if (Guest.instance.guest)
        {
            // If the user is a guest, log a message indicating they are not logged in
            Debug.Log("User not logged in");
        }
        else
        {
            // Set the hit sound volume setting in the Firebase database under the user's settings
            DatabaseManager.instance.databaseReference
                .Child("Users")
                .Child(DatabaseManager.instance.username)
                .Child("Settings")
                .Child("Volume")
                .Child(soundType)
                .SetValueAsync(soundVolume);

            // Log a message indicating the hit sound volume setting was saved to Firebase
            Debug.Log("Volume setting saved to Firebase!");
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
        if (Guest.instance.guest)
        {
            // If the user is not logged in, log a message indicating so
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is logged in, retrieve the player's username
            string playerUsername = DatabaseManager.instance.username;

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
        if (Guest.instance.guest)
        {
            // If the user is not logged in, log a message indicating so
            Debug.Log("User not logged in");
        }
        else
        {
            // If the user is logged in, retrieve the player's username
            string playerUsername = DatabaseManager.instance.username;

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
            string playerUsername = DatabaseManager.instance.username;

            // Set the VSync setting in the Firebase database under the player's settings
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("VSync").SetValueAsync(isVSyncOn);

            // Log a message indicating that the VSync setting has been saved to Firebase
            Debug.Log("VSync setting saved to Firebase!");
        }
    }
}
