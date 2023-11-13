using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public AudioSource music;
    public AudioSource noteHitSound;

    public float bpm;
    public bool startMusic;
    public NoteMovement NoteMovement;
    public Text pressAnyKey;

    public Text scoreText;
    private int currentScore;
    private int scorePerNote;
    // Purfect            light blue  500
    // EPurfect/LPurfect  green       350
    // Early/Late         yellow      200
    // Missed             red         0
    private int scorePerfect;
    private int scoreELPurfect;
    private int scoreEL;

    public int purfectCounter;
    public int eLPurfecyCounter;
    public int eLCounter;
    public int missedCounter;

    public Text multiplierText;
    public SpriteRenderer multiplierBackground;
    private int currentMultiplier;
    private int multiplierTracker;
    public int[] multiplierThresholds;

    public GameObject Hearth1;
    public GameObject Hearth2;
    public GameObject Hearth3;
    public GameObject Hearth4;
    private int currentDamageTaken;
    private float damageTracker;
    public GameObject Notes;
    public int[] damageThresholds;

    void Start()
    {
        instance = this;
        music.enabled = false;
        //music.volume = //audio mixer;

        scoreText.text = "0";
        scorePerfect = 500;
        scoreELPurfect = 350;
        scoreEL = 200;
        currentMultiplier = 1;
    }

    void Update()
    {
        MultiplyerBackground();

        if (!startMusic)
        {
            if (Input.anyKeyDown)
            {
                NoteMovement.gameStart = true;
                music.enabled = true;
                startMusic = true;
                if (pressAnyKey != null)
                {
                    pressAnyKey.text = string.Empty;
                }
            }
        }
    }

    public void DamageTake()
    {
        //// Death indicator
        //if (currentDamageTaken <= damageThresholds.Length)
        //{
        //    currentDamageTaken++;

        //    //Removing hearths
        //    if (currentDamageTaken == 1)
        //    {
        //        Hearth1.SetActive(false);
        //    }
        //    else if (currentDamageTaken == 2)
        //    {
        //        Hearth2.SetActive(false);
        //    }
        //    else if (currentDamageTaken == 3)
        //    {
        //        Hearth3.SetActive(false);
        //    }
        //    else if (currentDamageTaken == 4)
        //    {
        //        Hearth4.SetActive(false);
        //        Notes.SetActive(false);

        //        currentDamageTaken = 0;

        //        pressAnyKey.text = "You're Dead";
        //    }
        //}
        //else
        //{
        //    Debug.Log("Something went wrong with the damage tracker");
        //}
    }
    public void DamageHeal()
    {
        //Get hearths
        if (currentDamageTaken == 0)
        {
            Hearth1.SetActive(true);
        }
        else if (currentDamageTaken == 1)
        {
            Hearth2.SetActive(true);
        }
        else if (currentDamageTaken == 2)
        {
            Hearth3.SetActive(true);
        }
    }
    public void MultiplyerBackground()
    {
        //Multiplier background color change
        if (currentMultiplier == 1)
        {
            multiplierBackground.color = Color.gray;
            multiplierText.color = Color.white;
        }
        else if (currentMultiplier == 2)
        {
            multiplierBackground.color = Color.green;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier == 4)
        {
            multiplierBackground.color = Color.blue;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier == 8)
        {
            multiplierBackground.color = Color.red;
            multiplierText.color = Color.black;
        }
        else if (currentMultiplier >= 16)
        {
            multiplierBackground.color = Color.yellow;
            multiplierText.color = Color.black;
        }
    }

    public void NoteHit()
    {
        if (currentMultiplier / 2 < multiplierThresholds.Length)
        {
            multiplierTracker++;

            if (multiplierThresholds[currentMultiplier / 2] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier *= 2;
            }
        }

        multiplierText.text = "x" + currentMultiplier;

        currentScore += scorePerNote * currentMultiplier;
        scoreText.text = $"{currentScore}";
        noteHitSound.Play();
    }

    public void NotePurfect()
    {
        purfectCounter++;

        currentScore += scorePerfect * currentMultiplier;
        NoteHit();

        // 2 notes heal 1 hearth
        // but for now is 1 for 1
        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Purfect");
    }

    public void NoteELPurfect()
    {
        eLPurfecyCounter++;

        currentScore += scoreELPurfect * currentMultiplier;
        NoteHit();


        Debug.Log("ELPurfect");
    }

    public void NoteEL()
    {
        eLCounter++;

        currentScore += scoreEL * currentMultiplier;
        NoteHit();

        Debug.Log("EL");
    }

    public void NoteMissed()
    {
        missedCounter++;
        DamageTake();

        currentMultiplier = 1;
        multiplierTracker = 0;

        multiplierText.text = "x" + currentMultiplier;
        Debug.Log("Missed");
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