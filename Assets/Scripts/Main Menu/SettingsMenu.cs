using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("------- Audio -------")]
    public AudioMixer audioMixer;

    [Header("------- Settings -------")]
    public Toggle fullscreenToggle;
    public Toggle vSyncToggle;

    [Header("------- Resolution -------")]
    public TMP_Dropdown resolutionsDropdown;
    public Resolution[] resolutions;

    [Header("------- Canvas Groups -------")]
    public CanvasGroup GeneralCanvasGroup;
    public CanvasGroup InputCanvasGroup;
    public CanvasGroup AudioCanvasGroup;
    public CanvasGroup DisplayCanvasGroup;

    [Header("------- Animations -------")]
    public Animator ButtonAnimator;

    [System.Obsolete]
    public void Start()
    {
        // Resoluton
        resolutions = Screen.resolutions;
        resolutionsDropdown.ClearOptions();
        // Turning the array in to a list
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
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();

        // Buttons
        ButtonAnimator.SetBool("General", true);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", false);
        ButtonAnimator.SetBool("Display", false);
        // Settigs
        fullscreenToggle.isOn = true;
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
    // Audio
    public void AudioButton()
    {
        ButtonAnimator.SetBool("General", false);
        ButtonAnimator.SetBool("Input", false);
        ButtonAnimator.SetBool("Audio", true);
        ButtonAnimator.SetBool("Display", false);
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
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
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    [System.Obsolete]
    public void VSync()
    {
        if (vSyncToggle.isOn)
        {
            var refreshRate = Screen.currentResolution.refreshRate;
            Application.targetFrameRate = refreshRate;
        }
        else
        {
            Application.targetFrameRate = -1;
        }
    }
}
