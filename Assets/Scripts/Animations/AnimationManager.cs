using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [Header("------- Animaton -------")]
    [Tooltip("The type animation you want to manage")]
    public AnimationType animationtype;
    public enum AnimationType { SongShower, FadeAnimation, TutorialAnimation }

    [Tooltip("The animation")]
    public Animator transition;

    [Header("------- Animaton time -------")]
    [Tooltip("The time where the Up, Down, Left and Right circle text will fade")]
    public float fadeTimer;
    [Tooltip("The time after the song shower will show the song")]
    public float songShowerTimer;

    void Start()
    {
        instance = this;
        transition = GetComponent<Animator>();
    }

    void Update()
    {
        // Wait for the music to start
        if (GameManager.instance.music.enabled == true)
        {
            // Checking what is the animation type
            switch (animationtype)
            {
                // Song shower animation
                case AnimationType.SongShower:
                    StartCoroutine(AnimationTrigger(songShowerTimer));
                    break;
                // Circle text fade animation
                case AnimationType.FadeAnimation:
                    StartCoroutine(AnimationTrigger(fadeTimer));
                    break;
                case AnimationType.TutorialAnimation:
                    if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !PauseMenu.instance.gameIsPaused)
                        transition.SetBool("isTriggered", true);
                    break;
            }
        }
    }

    public IEnumerator AnimationTrigger(float timer)
    {
        // Wait for the specified fadeTime
        yield return new WaitForSeconds(timer);

        // Set the transition trigger after the delay
        transition.SetBool("isTriggered", true);
    }
}
