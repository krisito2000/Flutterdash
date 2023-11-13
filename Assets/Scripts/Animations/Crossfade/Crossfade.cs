using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crossfade : MonoBehaviour
{
    public Animator transition;
    // Start is called before the first frame update
    void Start()
    {
        CrossfadeAnimation();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerable CrossfadeAnimation()
    {

        //transition.SetTrigger("Start");
        yield return new WaitForSeconds(1);
        //transition.SetTrigger("End");


    }
}
