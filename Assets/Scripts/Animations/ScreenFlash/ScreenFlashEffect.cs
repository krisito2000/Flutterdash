using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFlashEffect : MonoBehaviour
{
    public void StartFlash(float duration, Color flashColor)
    {
        StartCoroutine(FlashCoroutine(duration, flashColor));
    }

    private IEnumerator FlashCoroutine(float duration, Color flashColor)
    {
        yield return new WaitForSeconds(duration);
    }

}
