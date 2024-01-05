using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransition : MonoBehaviour
{
    public static MainMenuTransition instance;

    [Header("------- Animaton -------")]
    public Animator animator;

    [Header("------- Canvases -------")]
    public CanvasGroup mainMenuCanvas;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        if (mainMenuCanvas == null) return;

        if (mainMenuCanvas.alpha == 1 && !animator.GetBool("GuestPlayTrigger"))
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                SetTutorial(false);
                if (!GetSettings() && !GetCustomSong() && !GetPlay())
                {
                    if (Guest.instance.guest)
                    {
                        animator.SetBool("GuestPlayTrigger", true);
                        Guest.instance.guestCanvas.alpha = 1;
                    }
                    PlayButton();
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
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                SetTutorial(false);
                if (!GetSettings() && !GetCustomSong() && !GetPlay())
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
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                SetTutorial(false);
                if (!GetCustomSong() && !GetSettings() && !GetPlay() && !GetSync())
                {
                    CustomSongButton();
                    BackMainMenuPlay();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
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
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                SetTutorial(false);
                Guest.instance.guestCanvas.alpha = 0;
                if (GetPlay())
                {
                    BackMainMenuPlay();
                    SetTransitionCustom(false);
                    SetTransitionSettings(false);
                    
                }
            }
        }
    }
    // Level Selection
    public void PlayButton()
    {
        SetPlay(true);
        SetSettings(false);
        SetCustomSong(false);

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
