using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TerrainTools;
using UnityEngine.UI;

public class TutorialGameManager : MonoBehaviour
{
    public static TutorialGameManager instance;

    //[Header("------- Sound -------")]
    //public AudioSource noteHitSound;

    [Header("------- Note manager -------")]
    public float bpm;
    public bool tryNotes;
    public bool tutorialDone;

    void Start()
    {
        instance = this;
        //music.volume = //audio mixer;
    }

    void Update()
    {
        if(tryNotes)
        {
            TutorialNoteMovement.instance.gameStart = true;
        }
        if (tutorialDone)
        {
            SceneManager.LoadScene(3);
        }
    }


    void Awake()
    {
        // Ensure there is only one instance of the GameManager script in the scene.
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            // Destroy this instance if there is already another one in the scene.
            Destroy(gameObject);
            return;
        }

        // Keep this GameObject alive throughout the entire game.
        DontDestroyOnLoad(gameObject);
    }
}