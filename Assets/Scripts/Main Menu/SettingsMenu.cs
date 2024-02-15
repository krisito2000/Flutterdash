using Firebase.Database;
using Google.MiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu instance;

    [Header("------- Canvas Groups -------")]
    public CanvasGroup GeneralCanvasGroup;
    public CanvasGroup InputCanvasGroup;
    public CanvasGroup AudioCanvasGroup;
    public CanvasGroup DisplayCanvasGroup;

    [Header("------- Animations -------")]
    public Animator ButtonAnimator;

    [Header("------- Settings -------")]

    [Header("------- Input -------")]
    public bool InputLockMode;
    public GameObject lockButton;
    public Text lockButtonText;
    private bool waitingForInput;

    public CanvasGroup UpCircleCanvasGroup;
    public Text UpCircleText;
    public string UpCircleKeyCode;

    public CanvasGroup LeftCircleCanvasGroup;
    public Text LeftCircleText;
    public string LeftCircleKeyCode;

    public CanvasGroup DownCircleCanvasGroup;
    public Text DownCircleText;
    public string DownCircleKeyCode;

    public CanvasGroup RightCircleCanvasGroup;
    public Text RightCircleText;
    public string RightCircleKeyCode;

    [Header("------- Audio -------")]
    public AudioMixer audioMixer;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider hitSoundSlider;
    public Text syncText;
    public Slider syncSlider;
    public UnityEvent<float> onSliderValueChanged = new UnityEvent<float>();

    [Header("------- Display -------")]
    public Toggle fullscreenToggle;

    public TMP_Dropdown resolutionsDropdown;
    public Resolution[] resolutions;

    public Toggle VSyncToggle;

    [System.Obsolete]
    public void Start()
    {
        instance = this;

        // Input
        InputLockMode = false;

        UpCircleKeyCode = "W";
        LeftCircleKeyCode = "A";
        DownCircleKeyCode = "S";
        RightCircleKeyCode = "D";

        // Volume

        onSliderValueChanged.AddListener(SetMasterVolume);
        onSliderValueChanged.AddListener(SetMusicVolume);
        onSliderValueChanged.AddListener(SetHitSoundVolume);

        // Resolution
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        // Turning the array into a list
        List<string> resolutionOptions = new List<string>();

        int currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            string resolutionOption = resolutions[i].width + " x " + resolutions[i].height;
            resolutionOptions.Add(resolutionOption);

            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionsDropdown.AddOptions(resolutionOptions);

        //resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        // Buttons
        ButtonAnimator.SetBool("General", true);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", false);

        VSync();
    }
    public void Update()
    {
        
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

    public void LockButton()
    {
        if (InputLockMode == true)
        {
            InputLockMode = false;
            ButtonAnimator.SetBool("LockMode", false);

            lockButtonText.text = "Lock";
            StartCoroutine(MainMenuTransition.instance.LoadKeybindsCoroutine());
        }
        else
        {
            if (!Guest.instance.guest)
            {
                InputLockMode = true;
                ButtonAnimator.SetBool("LockMode", true);

                lockButtonText.text = "Unlock";
            }
            else
            {
                lockButtonText.text = "Need to login";
            }
        }
    }

    public void UpCircle()
    {
        UpCircleText.text = "?";
        lockButton.SetActive(false);

        LeftCircleCanvasGroup.interactable = false;
        DownCircleCanvasGroup.interactable = false;
        RightCircleCanvasGroup.interactable = false;

        if (!waitingForInput)
        {
            StartCoroutine(WaitForButtonPress(UpCircleKeyCode, UpCircleText));
        }
    }

    public void LeftCircle()
    {
        LeftCircleText.text = "?";
        lockButton.SetActive(false);

        UpCircleCanvasGroup.interactable = false;
        DownCircleCanvasGroup.interactable = false;
        RightCircleCanvasGroup.interactable = false;

        if (!waitingForInput)
        {
            StartCoroutine(WaitForButtonPress(LeftCircleKeyCode, LeftCircleText));
        }
    }

    public void DownCircle()
    {
        DownCircleText.text = "?";
        lockButton.SetActive(false);

        UpCircleCanvasGroup.interactable = false;
        LeftCircleCanvasGroup.interactable = false;
        RightCircleCanvasGroup.interactable = false;

        if (!waitingForInput)
        {
            StartCoroutine(WaitForButtonPress(DownCircleKeyCode, DownCircleText));
        }
    }

    public void RightCircle()
    {
        RightCircleText.text = "?";
        lockButton.SetActive(false);

        UpCircleCanvasGroup.interactable = false;
        LeftCircleCanvasGroup.interactable = false;
        DownCircleCanvasGroup.interactable = false;

        if (!waitingForInput)
        {
            StartCoroutine(WaitForButtonPress(RightCircleKeyCode, RightCircleText));
        }
    }

    private IEnumerator WaitForButtonPress(string circleKeyCode, Text circleTextChange)
    {
        var keybindPath = DatabaseManager.instance.databaseReference
            .Child("Users")
            .Child(Guest.instance.LoginAs.text)
            .Child("Settings")
            .Child("Input")
            .Child(circleKeyCode);

        waitingForInput = true;

        while (true)
        {
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Mouse0))
            {
                circleKeyCode = Input.inputString.ToUpper();
                Debug.Log("User Input: " + circleKeyCode);

                UpCircleCanvasGroup.interactable = true;
                LeftCircleCanvasGroup.interactable = true;
                DownCircleCanvasGroup.interactable = true;
                RightCircleCanvasGroup.interactable = true;

                circleTextChange.text = circleKeyCode.ToUpper();
                lockButton.SetActive(true);

                // Save the updated keybind to the database
                keybindPath.SetValueAsync(circleKeyCode.ToUpper());

                break;
            }

            yield return null;
        }

        waitingForInput = false;
    }

    public void ResetButton()
    {
        // Define default keybind values
        string defaultUpKeyCode = "W";
        string defaultLeftKeyCode = "A";
        string defaultDownKeyCode = "S";
        string defaultRightKeyCode = "D";

        // Update the UI text fields with default values
        UpCircleText.text = defaultUpKeyCode;
        LeftCircleText.text = defaultLeftKeyCode;
        DownCircleText.text = defaultDownKeyCode;
        RightCircleText.text = defaultRightKeyCode;

        // Update the database with default keybind values
        DatabaseManager databaseManager = DatabaseManager.instance;
        var databaseReference = databaseManager.databaseReference;
        string playerUsername = Guest.instance.LoginAs.text;

        databaseReference
            .Child("Users")
            .Child(playerUsername)
            .Child("Settings")
            .Child("Input")
            .Child("W")
            .SetValueAsync(defaultUpKeyCode);

        databaseReference
            .Child("Users")
            .Child(playerUsername)
            .Child("Settings")
            .Child("Input")
            .Child("A")
            .SetValueAsync(defaultLeftKeyCode);

        databaseReference
            .Child("Users")
            .Child(playerUsername)
            .Child("Settings")
            .Child("Input")
            .Child("S")
            .SetValueAsync(defaultDownKeyCode);

        databaseReference
            .Child("Users")
            .Child(playerUsername)
            .Child("Settings")
            .Child("Input")
            .Child("D")
            .SetValueAsync(defaultRightKeyCode);

        Debug.Log("Keybinds reset to default values in the database.");
    }


    // Audio
    public void AudioButton()
    {
        ButtonAnimator.SetBool("General", false);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", true);
        ButtonAnimator.SetBool("Display", false);
    }
    public void SetMasterVolume(float volume)
    {
        volume = masterSlider.value;
        audioMixer.SetFloat("Master", volume);
        SaveMasterVolumeSetting(volume);
    }
    public void SaveMasterVolumeSetting(float masterVolume)
    {
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("Master").SetValueAsync(masterVolume);

            Debug.Log("Master volume setting saved to Firebase!");
        }
    }
    public void SetMusicVolume(float volume)
    {
        volume = musicSlider.value;
        audioMixer.SetFloat("Music", volume);
        SaveMusicVolumeSetting(volume);
    }
    public void SaveMusicVolumeSetting(float musicVolume)
    {
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("Music").SetValueAsync(musicVolume);

            Debug.Log("Music volume setting saved to Firebase!");
        }
    }
    public void SetHitSoundVolume(float volume)
    {
        volume = hitSoundSlider.value;
        audioMixer.SetFloat("HitSound", volume);
        SaveHitSoundVolumeSetting(volume);
    }
    public void SaveHitSoundVolumeSetting(float hitSoundVolume)
    {
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Volume").Child("HitSound").SetValueAsync(hitSoundVolume);

            Debug.Log("HitSound volume setting saved to Firebase!");
        }
    }
    public void SetSync(float ms)
    {
        ms = syncSlider.value;
        ms = (float)Math.Floor(ms);
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
    public void SetFullscreen()
    {
        Screen.fullScreen = fullscreenToggle.isOn;
        SaveFullscreenSetting(fullscreenToggle.isOn);
    }
    public void SaveFullscreenSetting(bool isFullscreen)
    {
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("Fullscreen").SetValueAsync(isFullscreen);

            Debug.Log("Fullscreen setting saved to Firebase!");
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        string resolutionData = $"{resolution.width}x{resolution.height}";
        SaveResolutionSetting(resolutionData);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }
    public void SaveResolutionSetting(string resolutionData)
    {
        if (Guest.instance.LoginAs.text == "Login as Guest")
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("Resolution").SetValueAsync(resolutionData);

            Debug.Log("Resolution setting saved to Firebase!");
        }
    }

    [System.Obsolete]
    public void VSync()
    {
        if (VSyncToggle.isOn)
        {
            var refreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = refreshRate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
        SaveVSyncSetting(VSyncToggle.isOn);
    }
    public void SaveVSyncSetting(bool isVSyncOn)
    {
        if (Guest.instance.guest)
        {
            Debug.Log("User not logged in");
        }
        else
        {
            string playerUsername = Guest.instance.LoginAs.text;
            DatabaseManager.instance.databaseReference
                .Child("Users").Child(playerUsername).Child("Settings").Child("Display").Child("VSync").SetValueAsync(isVSyncOn);

            Debug.Log("VSync setting saved to Firebase!");
        }
    }
}
