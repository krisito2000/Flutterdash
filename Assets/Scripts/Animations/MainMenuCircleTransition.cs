using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCircleTransition : MonoBehaviour
{
    public static MainMenuCircleTransition instance;
    [Tooltip("The animator for the circle trantition")]
    public Animator animator;
    [Tooltip("The text for the \"Press any key to play\"")]
    public Text AnyKeyText;

    private bool counterRunning = true;

    private void Start()
    {
        instance = this;

        // Show the "AnyKeyText" after certain time
        StartCoroutine(ShowAnyKeyTextAfterDelay(5f));
    }

    private void Update()
    {
        // Waiting for any input to start the circle transition
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Mouse0) && !PauseMenu.instance.gameIsPaused)
        {
            CircleTransition();
        }
    }

    private void CircleTransition()
    {
        // Check if it is not expanded yet
        if (!animator.GetBool("isExpanded"))
        {
            // Set the circle trigger to expand the circle
            animator.SetTrigger("Clicked");
            animator.SetBool("isExpanded", true);

            // Hide and disable the text before or after it is showed
            AnyKeyText.enabled = false;
            counterRunning = false;
        }
    }

    // Trigger animation after hover
    public void OnHover()
    {
        animator.SetTrigger("Hover");
    }

    // Trigger the normal animation after unhovering
    public void OnUnHover()
    {
        animator.SetTrigger("Normal");
    }

    // The delay for showing the "ShowAnyKey" text
    private IEnumerator ShowAnyKeyTextAfterDelay(float delay)
    {
        float timer = 0f;
        while (timer < delay && counterRunning)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (counterRunning)
        {
            AnyKeyText.gameObject.SetActive(true);
        }
    }
}