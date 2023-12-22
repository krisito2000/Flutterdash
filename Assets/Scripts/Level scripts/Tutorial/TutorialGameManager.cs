using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing.Printing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TerrainTools;
using UnityEngine.UI;

public class TutorialGameManager : MonoBehaviour
{
    public static TutorialGameManager instance;

    [Header("------- Sound -------")]
    public AudioSource music;
    //public AudioSource noteHitSound;

    [Header("------- Note manager -------")]
    public float bpm;
    public bool readyToPlay;
    public GameObject Notes;
    public NoteMovement NoteMovement;
    public Text pressAnyKey;

    [Header("------- Score -------")]
    public Text scoreText;
    public Text resultsScoreText;
    private int currentScore;
    private int scorePerNote;
    // Perfect            light blue  350
    // EPerfect/LPerfect  green       150
    // Early/Late         yellow      75
    // Missed             red         0
    private int scoreEarly;
    private int scoreEarlyPerfect;
    private int scorePerfect;
    private int scoreLatePerfect;
    private int scoreLate;

    [Header("------- Multiplier -------")]
    public Text multiplierText;
    public SpriteRenderer multiplierBackground;
    private int currentMultiplier;
    private int multiplierTracker;
    public int[] multiplierThresholds;

    [Header("------- Results -------")]
    public Animator resultsAnimation;
    private int earlyCounter;
    private int earlyPerfectCounter;
    private int perfectCounter;
    private int latePerfectCounter;
    private int lateCounter;
    private int missedCounter;
    public Text earlyText;
    public Text earlyPerfectText;
    public Text perfectText;
    public Text latePerfectText;
    public Text lateText;
    public Text missedText;

    [Header("------- Hearths -------")]
    public GameObject Hearth1;
    public GameObject Hearth2;
    public GameObject Hearth3;
    public GameObject Hearth4;

    [Header("------- Hearths animation -------")]
    public Animator Hearth1Fade;
    public Animator Hearth2Fade;
    public Animator Hearth3Fade;
    public Animator Hearth4Fade;

    [Header("------- Damage -------")]
    private int currentDamageTaken;
    public int[] damageThresholds;

    void Start()
    {
        instance = this;
        music.enabled = false;
        readyToPlay = false;
        //music.volume = //audio mixer;

        scoreText.text = "0";
        scoreEarly = 75;
        scoreEarlyPerfect = 150;
        scorePerfect = 350;
        scoreLatePerfect = 150;
        scoreLate = 75;
        currentMultiplier = 1;
    }

    void Update()
    {
        if (readyToPlay)
        {
            if (Input.anyKeyDown)
            {
                NoteMovement.gameStart = true;
                music.enabled = true;
                readyToPlay = false;
                if (pressAnyKey != null)
                {
                    pressAnyKey.text = string.Empty;
                }
            }
        }
        else
        {
            //if (!music.isPlaying)
            //{
            //    Statistics();
            //    resultsAnimation.SetBool("isTriggered", true);
            //}
        }
    }

    public void Statistics()
    {
        earlyText.text = earlyCounter.ToString();
        earlyPerfectText.text = earlyPerfectCounter.ToString();
        perfectText.text = perfectCounter.ToString();
        latePerfectText.text = latePerfectCounter.ToString();
        lateText.text = lateCounter.ToString();
        missedText.text = missedCounter.ToString();
        resultsScoreText.text = currentScore.ToString();
    }

    public void MultiplierBackground()
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
        MultiplierBackground();
    }

    public void EarlyHit()
    {
        earlyCounter++;

        currentScore += scoreEarly * currentMultiplier;
        NoteHit();

        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Early Hit");
    }

    public void EarlyPerfectHit()
    {
        earlyPerfectCounter++;

        currentScore += scoreEarlyPerfect * currentMultiplier;
        NoteHit();

        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Early Perfect Hit");
    }

    public void PerfectHit()
    {
        perfectCounter++;

        currentScore += scorePerfect * currentMultiplier;
        NoteHit();

        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Perfect Hit");
    }

    public void LatePerfectHit()
    {
        latePerfectCounter++;

        currentScore += scoreLatePerfect * currentMultiplier;
        NoteHit();

        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Late Perfect Hit");
    }
    public void LateHit()
    {
        lateCounter++;

        currentScore += scoreLate * currentMultiplier;
        NoteHit();

        if (currentDamageTaken != 0)
        {
            currentDamageTaken--;
            DamageHeal();
        }

        Debug.Log("Late Hit");
    }

    public void NoteMissed()
    {
        missedCounter++;
        DamageTake();

        currentMultiplier = 1;
        multiplierTracker = 0;
        multiplierBackground.color = Color.gray;
        multiplierText.color = Color.white;

        multiplierText.text = "x" + currentMultiplier;
        Debug.Log("Missed");
    }

    public void DamageTake()
    {
        // Death indicator
        if (currentDamageTaken <= damageThresholds.Length)
        {
            currentDamageTaken++;

            //Removing hearths
            if (currentDamageTaken == 1)
            {
                Hearth1.SetActive(false);
            }
            else if (currentDamageTaken == 2)
            {
                Hearth2.SetActive(false);
            }
            else if (currentDamageTaken == 3)
            {
                Hearth3.SetActive(false);
            }
            else if (currentDamageTaken == 4)
            {
                Hearth4.SetActive(false);
                Notes.SetActive(false);
                music.Stop();

                currentDamageTaken = 0;

                pressAnyKey.text = "You're Dead";
            }
        }
        else
        {
            Debug.Log("Something went wrong with the damage tracker");
        }
    }
    public void DamageHeal()
    {
        // Get hearths
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