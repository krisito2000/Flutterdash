using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCircleTransition : MonoBehaviour
{
    public Animator animator;
    public void CircleTransition()
    {
        animator.SetBool("isTriggered", true);
    }
}
