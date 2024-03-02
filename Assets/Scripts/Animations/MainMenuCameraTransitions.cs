using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransition : MonoBehaviour
{
    public static MainMenuTransition instance;

    [Header("------- Animaton -------")]
    public Animator animator;
    public CanvasGroup mainMenuCanvas;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (mainMenuCanvas == null) return;

        if (mainMenuCanvas.alpha == 1 && !animator.GetBool("GuestPlayTrigger") && MainMenuCircleTransition.instance.animator.GetBool("isExpanded") && !PauseMenu.instance.gameIsPaused && !SettingsMenu.instance.InputLockMode)
        {
            if (InputSystemController.instance.DownCircleClicked)
            {
                SetTutorial(false);
                if (!GetSettings() && !GetCustomSong() && !GetPlay() && !GetAuthentication() && !GetSync())
                {
                    if (Guest.instance.guest)
                    {
                        animator.SetBool("GuestPlayTrigger", true);
                        Guest.instance.guestCanvas.alpha = 1;
                    }
                    PlayButton();
                }
                if (GetAuthentication())
                {
                    SetAuthentication(false);
                }
                if (GetTransitionSettings())
                {
                    if (GetSettings())
                    {
                        BackMainMenuSettings();
                        PlayButton();
                        animator.SetBool("GuestPlayTrigger", false);
                    }
                }
                if (GetSettings())
                {
                    SetTransitionSettings(true);
                    BackMainMenuSettings();
                    PlayButton();
                    animator.SetBool("GuestPlayTrigger", false);
                }
                if (GetTransitionCustom())
                {
                    if (GetCustomSong())
                    {
                        BackMainMenuCustomSong();
                        PlayButton();
                        animator.SetBool("GuestPlayTrigger", false);
                    }
                }
                if (GetCustomSong())
                {
                    SetTransitionCustom(true);
                    BackMainMenuCustomSong();
                    PlayButton();
                    animator.SetBool("GuestPlayTrigger", false);
                }
            }
            else if (InputSystemController.instance.RightCircleClicked)
            {
                SetTutorial(false);
                if (GetAuthentication())
                {

                }
                else if (!GetSettings() && !GetCustomSong() && !GetPlay() && !GetSync())
                {
                    SettingsButton();
                }
                else if (GetCustomSong())
                {
                    BackMainMenuCustomSong();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
                }
                else if (GetTransitionSettings())
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionSettings(true);
                        SettingsButton();
                    }
                }
                else if (GetSettings() && !GetSync())
                {
                    SetSync(true);
                    SetSettings(false);
                }
                else
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionSettings(true);
                        SettingsButton();
                    }
                }
            }
            else if (InputSystemController.instance.LeftCircleClicked)
            {
                SetTutorial(false);
                if (GetAuthentication())
                {

                }
                else if (!GetCustomSong() && !GetSettings() && !GetPlay() && !GetSync())
                {
                    CustomSongButton();
                    BackMainMenuPlay();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
                }
                else if (GetAuthentication())
                {
                    SetCustomSong(false);
                }
                else if (GetSettings())
                {
                    BackMainMenuSettings();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
                }
                else if (GetTransitionCustom())
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionCustom(true);
                        CustomSongButton();
                    }
                }
                else if (GetSync() && !GetSettings())
                {
                    SetSync(false);
                    SetSettings(true);
                }
                else
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionCustom(true);
                        SetCustomSong(true);
                    }
                }
            }
            else if (InputSystemController.instance.DownCircleClicked)
            {
                SetTutorial(false);
                Guest.instance.guestCanvas.alpha = 0;
                if (!GetPlay() && !GetSync() && !GetSettings())
                {
                    SetAuthentication(true);
                }
                if (GetPlay())
                {
                    BackMainMenuPlay();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
                }
            }
        }
    }

    // Method to convert string key codes to Unity KeyCode
    KeyCode GetKeyCode(string key)
    {
        KeyCode keyCode;
        if (Enum.TryParse(key, out keyCode))
        {
            return keyCode;
        }
        else
        {
            Debug.LogWarning("Invalid key code: " + key);
            return KeyCode.None;
        }
    }

    // Level Selection
    public void PlayButton()
    {
        SetPlay(true);
        SetSettings(false);
        SetCustomSong(false);
        DatabaseManager.instance.LoadEveryLevelStats();

        if (animator.GetBool("isGuest") && !GetCustomSong() && !GetSettings())
        {
            animator.SetBool("GuestPlayTrigger", true);
            Guest.instance.guestCanvas.alpha = 1;
        }
    }
    public bool GetPlay()
    {
        return animator.GetBool("PlayTrigger");
    }
    public void SetPlay(bool trigger)
    {
        animator.SetBool("PlayTrigger", trigger);
    }

    public void BackMainMenuPlay()
    {
        SetPlay(false);
    }
    // Tutorial
    public void TutorialButton()
    {
        SetTutorial(true);
        DatabaseManager.instance.LoadEveryLevelStats();
    }
    public bool GetTutorial()
    {
        return animator.GetBool("TutorialTrigger");
    }
    public void SetTutorial(bool trigger)
    {
        animator.SetBool("TutorialTrigger", trigger);
    }

    // Settings
    public void SettingsButton()
    {
        SetPlay(false);
        SetSettings(true);
        SetCustomSong(false);
    }
    public bool GetSettings()
    {
        return animator.GetBool("SettingsTrigger");
    }
    public void SetSettings(bool trigger)
    {
        animator.SetBool("SettingsTrigger", trigger);
    }
    public void BackMainMenuSettings()
    {
        SetSettings(false);
    }

    // Custom Song creation
    public void CustomSongButton()
    {
        SetPlay(false);
        SetCustomSong(true);
        SetSettings(false);
    }
    public bool GetCustomSong()
    {
        return animator.GetBool("CustomTrigger");
    }
    public void SetCustomSong(bool trigger)
    {
        animator.SetBool("CustomTrigger", trigger);
    }

    public void BackMainMenuCustomSong()
    {
        SetCustomSong(false);
    }
    // Authentication
    public void AuthenticationButton()
    {
        SetPlay(false);
        SetCustomSong(false);
        SetSettings(false);
    }
    public bool GetAuthentication()
    {
        return animator.GetBool("AuthenticationTrigger");
    }
    public void SetAuthentication(bool trigger)
    {
        animator.SetBool("AuthenticationTrigger", trigger);
    }
    public void BackMainMenuAuthentication()
    {
        SetCustomSong(false);
    }

    // Song synchronization
    public void SyncButton()
    {
        SetSync(true);
        SetSettings(false);
    }
    public bool GetSync()
    {
        return animator.GetBool("SyncTrigger");
    }
    public void SetSync(bool trigger)
    {
        animator.SetBool("SyncTrigger", trigger);
    }

    // Transitions
    public bool GetTransitionSettings()
    {
        return animator.GetBool("LevelSelectionXSettingsTrigger");
    }
    public void SetTransitionSettings(bool trigger)
    {
        animator.SetBool("LevelSelectionXSettingsTrigger", trigger);
    }

    public bool GetTransitionCustom()
    {
        return animator.GetBool("LevelSelectionXCustomTrigger");
    }
    public void SetTransitionCustom(bool trigger)
    {
        animator.SetBool("LevelSelectionXCustomTrigger", trigger);
    }
}
