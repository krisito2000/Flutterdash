using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuCircleTransition : MonoBehaviour
{
    public static MainMenuCircleTransition instance;
    public Animator animator;
    public Text AnyKeyText;

    private bool counterRunning = true;

    private void Start()
    {
        instance = this;
        StartCoroutine(ShowAnyKeyTextAfterDelay(2f));
    }

    private void Update()
    {
        if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Mouse0))
        {
            CircleTransition();
        }
    }

    private void CircleTransition()
    {
        if (!animator.GetBool("isExpanded"))
        {
            animator.SetTrigger("Clicked");
            animator.SetBool("isExpanded", true);
            AnyKeyText.gameObject.SetActive(false);
            counterRunning = false;
        }
    }
    public void OnHover()
    {
        animator.SetTrigger("Hover");
    }

    public void OnUnHover()
    {
        animator.SetTrigger("Normal");
    }

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