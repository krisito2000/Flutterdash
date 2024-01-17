using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCircleTransition : MonoBehaviour
{
    public Animator animator;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            CircleTransition();
        }
    }
    public void CircleTransition()
    {
        if (!animator.GetBool("isExpanded"))
        {
            animator.SetTrigger("Clicked");
            animator.SetBool("isExpanded", true);
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
}
