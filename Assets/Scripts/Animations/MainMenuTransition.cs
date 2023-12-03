using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuTransition : MonoBehaviour
{
    // Reference to the Animator component
    public Animator animator;

    // Reference to your AnimatorController
    public AnimatorController movementAnimatorController;

    public CanvasGroup mainMenuCanvas;

    void Start()
    {
        // Access the Animator component from this GameObject
        animator = GetComponent<Animator>();

        // Assign the AnimatorController to the Animator component
        animator.runtimeAnimatorController = movementAnimatorController as RuntimeAnimatorController;

    }

    void Update()
    {
        if (mainMenuCanvas.alpha == 1)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (!animator.GetBool("SettingsTrigger") && !animator.GetBool("SyncTrigger") && !animator.GetBool("PlayTrigger"))
                {
                    PlayButton();
                }
                if (animator.GetBool("LevelSelectionXSettingsTrigger"))
                {
                    if (animator.GetBool("SettingsTrigger"))
                    {
                        BackMainMenuSettings();
                        PlayButton();
                    }
                }
                if (animator.GetBool("SettingsTrigger"))
                {
                    animator.SetBool("LevelSelectionXSettingsTrigger", true);
                    BackMainMenuSettings();
                    PlayButton();
                }
                if (animator.GetBool("LevelSelectionXSyncTrigger"))
                {
                    if (animator.GetBool("SyncTrigger"))
                    {
                        BackMainMenuSync();
                        PlayButton();
                    }
                }
                if (animator.GetBool("SyncTrigger"))
                {
                    animator.SetBool("LevelSelectionXSyncTrigger", true);
                    BackMainMenuSync();
                    PlayButton();
                }
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                if (!animator.GetBool("SettingsTrigger") && !animator.GetBool("SyncTrigger") && !animator.GetBool("PlayTrigger"))
                {
                    SettingsButton();
                }
                else if (animator.GetBool("SyncTrigger"))
                {
                    BackMainMenuSync();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
                if (animator.GetBool("LevelSelectionXSettingsTrigger"))
                {
                    if (animator.GetBool("PlayTrigger"))
                    {
                        BackMainMenuPlay();
                        animator.SetBool("LevelSelectionXSettingsTrigger", true);
                        SettingsButton();
                    }
                }
                else
                {
                    if (animator.GetBool("PlayTrigger"))
                    {
                        BackMainMenuPlay();
                        animator.SetBool("LevelSelectionXSettingsTrigger", true);
                        SettingsButton();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                if (!animator.GetBool("SyncTrigger") && !animator.GetBool("SettingsTrigger"))
                {
                    SyncButton();
                    BackMainMenuPlay();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
                else if (animator.GetBool("SettingsTrigger"))
                {
                    BackMainMenuSettings();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
                if (animator.GetBool("LevelSelectionXSyncTrigger"))
                {
                    if (animator.GetBool("PlayTrigger"))
                    {
                        BackMainMenuPlay();
                        animator.SetBool("LevelSelectionXSyncTrigger", true);
                        SyncButton();
                    }
                }
                else
                {
                    if (animator.GetBool("PlayTrigger"))
                    {
                        BackMainMenuPlay();
                        animator.SetBool("LevelSelectionXSettingsTrigger", true);
                        SettingsButton();
                    }
                }
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (animator.GetBool("PlayTrigger"))
                {
                    BackMainMenuPlay();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
            }
        }
    }

    // Level Selection
    public void PlayButton()
    {
        animator.SetBool("PlayTrigger", true);
    }

    public void BackMainMenuPlay()
    {
        animator.SetBool("PlayTrigger", false);
    }

    // Settings
    public void SettingsButton()
    {
        animator.SetBool("SettingsTrigger", true);
        animator.SetBool("SyncTrigger", false);
        animator.SetBool("PlayTrigger", false);
    }

    public void BackMainMenuSettings()
    {
        animator.SetBool("SettingsTrigger", false);
    }

    // Sync
    public void SyncButton()
    {
        animator.SetBool("SyncTrigger", true);
        animator.SetBool("SettingsTrigger", false);
        animator.SetBool("PlayTrigger", false);
    }

    public void BackMainMenuSync()
    {
        animator.SetBool("SyncTrigger", false);
    }
}