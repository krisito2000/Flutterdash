using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager instance;

    [Header("------- Animaton -------")]
    public AnimationType animationtype;
    public enum AnimationType { SongShower, FadeAnimation }
    public Animator transition;

    [Header("------- Animaton time -------")]
    public float fadeTimer;
    public float songShowerTimer;

    [Header("------- Objects -------")]
    public Animator circleAnimation;

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
            switch (animationtype)
            {
                case AnimationType.SongShower:
                    StartCoroutine(AnimationTrigger(songShowerTimer));
                    break;
                case AnimationType.FadeAnimation:
                    StartCoroutine(AnimationTrigger(fadeTimer));
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

    public IEnumerator NoteAnimation()
    {
        if (!gameObject.activeSelf)
        {
            switch (NoteObject.instance.direction)
            {
                case NoteObject.SpinDirection.Left:
                    circleAnimation.SetBool("triggerLeft", true);
                    yield break;
                case NoteObject.SpinDirection.Right:
                    circleAnimation.SetBool("triggerRight", true);
                    yield break;
            }
        }
    }

    public void LoginAnimation()
    {
        if (!transition.GetBool("isTriggered"))
        {
            transition.SetBool("isTriggered", true);
        }
        else
        {
            transition.SetBool("isTriggered", false);
        }
    }
}
