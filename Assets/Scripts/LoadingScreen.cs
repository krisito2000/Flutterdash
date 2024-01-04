using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingScreen : MonoBehaviour
{
    public static LoadingScreen instance;
    public GameObject LoadingScreenCanvas;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    //void Awake()
    //{
    //    // Ensure there is only one instance of the GameManager script in the scene.
    //    if (instance == null)
    //    {
    //        instance = this;
    //    }
    //    else
    //    {
    //        // Destroy this instance if there is already another one in the scene.
    //        Destroy(gameObject);
    //        return;
    //    }

    //    // Keep this GameObject alive throughout the entire game.
    //    DontDestroyOnLoad(gameObject);
    //}
}
