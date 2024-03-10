using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AskScreenController : MonoBehaviour
{
    public static AskScreenController instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            // Set the instance to this GameManager if it's the first one.
            instance = this;
        }
        else
        {
            // Destroy the existing instance if a new one is detected.
            Destroy(instance.gameObject);
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);
    }
}
