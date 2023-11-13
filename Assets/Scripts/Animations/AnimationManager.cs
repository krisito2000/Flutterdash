using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimationType { SongShower, FadeAnimation }
    public AnimationType animationtype;
    public Animator transition;
    public float fadeTime;
    public float songShowerTime;

    void Start()
    {
        switch (animationtype)
        {
            case AnimationType.SongShower:
                StartCoroutine(SongShower(songShowerTime));
                break;
            case AnimationType.FadeAnimation:
                StartCoroutine(FadeAnimation(fadeTime));
                break;
        }
    }

    public IEnumerator FadeAnimation(float fadeTime)
    {
        // Wait for the specified fadeTime
        yield return new WaitForSeconds(fadeTime);

        // Set the transition trigger after the delay
        transition.SetBool("isTriggered", true);
    }
    public IEnumerator SongShower(float songShowerTime)
    {
        // Wait for the specified songShowerTime
        yield return new WaitForSeconds(songShowerTime);

        // Set the transition trigger after the delay
        transition.SetBool("isTriggered", true);
    }
}
