using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public enum AnimationType { SongShower, FadeAnimation }
    public AnimationType animationtype;
    public Animator transition;
    public float fadeTimer;
    public float songShowerTimer;

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
}
