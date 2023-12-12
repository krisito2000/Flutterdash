using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SettingsMenu : MonoBehaviour
{
    [Header("------- Audio -------")]
    public AudioMixer audioMixer;

    [Header("------- CheckBox -------")]
    public GameObject CheckBox;

    // General
    public void GeneralButton()
    {

    }
    // Input
    public void InputButton()
    {

    }
    // Audio
    public void AudioButton()
    {

    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }
    // Display
    public void DisplayButton()
    {

    }
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }
}
