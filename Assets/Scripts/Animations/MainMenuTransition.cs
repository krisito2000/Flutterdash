using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;

public class MainMenuTransition : MonoBehaviour
{
    // Reference to the Animator component
    public Animator animator; 

    // Reference to your AnimatorController
    public AnimatorController movementAnimatorController;

    void Start()
    {
        // Access the Animator component from this GameObject
        animator = GetComponent<Animator>();

        // Assign the AnimatorController to the Animator component
        animator.runtimeAnimatorController = movementAnimatorController as RuntimeAnimatorController;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (!animator.GetBool("SettingsTrigger") && !animator.GetBool("SyncTrigger"))
            {
                if (!animator.GetBool("PlayTrigger"))
                {
                    PlayButton();
                }
            }
            if (animator.GetBool("SyncTrigger"))
            {
                animator.SetBool("LevelSelectionXSyncTrigger", true);
                BackMainMenuSync();
                PlayButton();
            }
            if (animator.GetBool("SettingsTrigger"))
            {
                animator.SetBool("LevelSelectionXSettingsTrigger", true);
                BackMainMenuSettings();
                PlayButton();
            }
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (!animator.GetBool("SettingsTrigger"))
            {
                if (!animator.GetBool("SyncTrigger"))
                {
                    SettingsButton();
                    BackMainMenuLevelSelection();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
            }
            if (animator.GetBool("SyncTrigger"))
            {
                BackMainMenuSync();
            }
        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (!animator.GetBool("SyncTrigger"))
            {
                if (!animator.GetBool("SettingsTrigger"))
                {
                    SyncButton();
                    BackMainMenuLevelSelection();
                    animator.SetBool("LevelSelectionXSyncTrigger", false);
                    animator.SetBool("LevelSelectionXSettingsTrigger", false);
                }
            }
            if (animator.GetBool("SettingsTrigger"))
            {
                BackMainMenuSettings();
            }
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (animator.GetBool("PlayTrigger"))
            {
                BackMainMenuLevelSelection();
                animator.SetBool("LevelSelectionXSyncTrigger", false);
                animator.SetBool("LevelSelectionXSettingsTrigger", false);
            }
        }
    }

    // Level Selection
    public void PlayButton()
    {
        animator.SetBool("PlayTrigger", true);
        animator.SetBool("SettingsTrigger", false);
        animator.SetBool("SyncTrigger", false);
    }

    public void BackMainMenuLevelSelection()
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
