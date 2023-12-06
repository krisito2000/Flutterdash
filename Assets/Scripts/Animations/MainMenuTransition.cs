using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransition : MonoBehaviour
{
    public static MainMenuTransition instance;

    [Header("------- Animaton -------")]
    // Reference to the Animator component
    public Animator animator;

    // Reference to your AnimatorController
    public AnimatorController movementAnimatorController;

    [Header("------- Canvases -------")]
    public CanvasGroup mainMenuCanvas;

    void Start()
    {
        instance = this;

        // Access the Animator component from this GameObject
        animator = GetComponent<Animator>();

        // Assign the AnimatorController to the Animator component
        animator.runtimeAnimatorController = movementAnimatorController as RuntimeAnimatorController;

    }

    void Update()
    {
        if (mainMenuCanvas.alpha == 1 && !animator.GetBool("GuestPlayTrigger"))
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!GetSettings() && !GetSync() && !GetPlay())
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
                    }
                }
                if (GetSettings())
                {
                    SetTransitionSettings(true);
                    BackMainMenuSettings();
                    PlayButton();
                }
                if (GetTransitionSync())
                {
                    if (GetSync())
                    {
                        BackMainMenuSync();
                        PlayButton();
                    }
                }
                if (GetSync())
                {
                    SetTransitionSync(true);
                    BackMainMenuSync();
                    PlayButton();
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!GetSettings() && !GetSync() && !GetPlay())
                {
                    SettingsButton();
                }
                else if (GetSync())
                {
                    BackMainMenuSync();
                    SetTransitionSync(false);
                    SetTransitionSettings(false);
                }
                if (GetTransitionSettings())
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionSettings(true);
                        SettingsButton();
                    }
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
                if (!GetSync() && !GetSettings() && !GetPlay())
                {
                    SyncButton();
                    BackMainMenuPlay();
                    SetTransitionSync(false);
                    SetTransitionSettings(false);
                }
                else if (GetSettings())
                {
                    BackMainMenuSettings();
                    SetTransitionSync(false);
                    SetTransitionSettings(false);
                }
                else if (GetTransitionSync())
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionSync(true);
                        SyncButton();
                    }
                }
                else
                {
                    if (GetPlay())
                    {
                        BackMainMenuPlay();
                        SetTransitionSync(true);
                        SetSync(true);
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (GetPlay())
                {
                    BackMainMenuPlay();
                    SetTransitionSync(false);
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
        SetSync(false);
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

    // Settings
    public void SettingsButton()
    {
        SetPlay(false);
        SetSettings(true);
        SetSync(false);
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

    // Sync
    public void SyncButton()
    {
        SetPlay(false);
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

    public void BackMainMenuSync()
    {
        SetSync(false);
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

    public bool GetTransitionSync()
    {
        return animator.GetBool("LevelSelectionXSyncTrigger");
    }
    public void SetTransitionSync(bool trigger)
    {
        animator.SetBool("LevelSelectionXSyncTrigger", trigger);
    }
}