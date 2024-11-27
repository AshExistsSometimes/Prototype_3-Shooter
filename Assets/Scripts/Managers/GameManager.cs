using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // FIELDS //
    [HideInInspector]
    public float Score;
    [HideInInspector]
    public float HighScore;
    [HideInInspector]
    public float KillStreak;


    // EDITABLE FIELDS //////////////////////////////////////////////

    [Header ("Killstreak")]
    public float KSCooldown = 5f;
    public float KillcountTimer = 0f;
    public float KillcountDepletionSpeed = 1f;
    [Space]
    public float KillstreakMilestoneOne = 2;// Double Kill
    public float KillstreakMilestoneTwo = 3;// Triple Kill
    public float KillstreakMilestoneThree = 5;// Penta Kill
    public float KillstreakMilestoneFour = 10;// Killing Spree
    public float KillstreakMilestoneFive = 20;// Genocide
    [Space]
    public Slider KilltreakTimerSlider;

    [Header("Score")]
    public float ScorePerKill;
    public float KilledBossMultiplier = 100f;
    [Space]
    private float scoreMultiplier;

    private float ScoreMultiplierZero = 1f;
    [Space]
    [Space]
    public float ScoreMultiplierOne = 1.5f;
    public float ScoreMultiplierTwo = 1.5f;
    public float ScoreMultiplierThree = 2f;
    public float ScoreMultiplierFour = 5f;
    public float ScoreMultiplierFive = 10f;

    // UI References //

    [Header("UI References")]
    public TMP_Text ScoreText;
    public TMP_Text DeathScreenScoreText;
    public TMP_Text DeathScreenHighScoreText;
    [Space]
    public GameObject KillstreakPopup;
    [Space]
    public TMP_Text MiletstoneText;
    public TMP_Text KillCountText;
    public TMP_Text MultiplierText;

    /////////////////////////////////////////////////////////////////

    private void Awake()
    {
        // If there is an instance, and it's not me, it deletes itself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
            DontDestroyOnLoad(KillstreakPopup);// DESTROYED ON LOAD, BREAKS SCORE ON RESPAWN
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Score = 0f;
        Time.timeScale = 1.0f;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScoreUI();
        UpdateScoreMultiplier();
        UpdateHighScore();// High Score Logic

        DepleteCooldownTimer();

        UpdateKillstreakValues();

        // DEBUG SCORE CODE
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnKillEnemy();
        }
    }

    // SCORE LOGIC //

    public void UpdateScoreUI()
    {
        ScoreText.text = ("Score: " + Score);
        DeathScreenScoreText.text = ("Score: " + Score);
    }
    public void UpdateHighScore()
    {
        if (Score > HighScore)
        {
            Debug.Log("New High Score");
            HighScore = Score;
            DeathScreenHighScoreText.text = ("High Score: " + HighScore);
        }
    }

    public void UpdateScoreMultiplier()
    {
      MultiplierText.text = ("Multiplier x" + scoreMultiplier);

        // sets multiplier to ZERO when the killstreak hasnt reached the first milsetone
        if (KillStreak < KillstreakMilestoneOne)
        {
            scoreMultiplier = ScoreMultiplierZero;
        }

        // update multiplier for every milestone
        if (KillStreak >= KillstreakMilestoneOne && KillStreak <= KillstreakMilestoneTwo)// Double kill
        {
            MiletstoneText.text = ("DOUBLE KILL");
            scoreMultiplier = ScoreMultiplierOne;
        }

        if (KillStreak >= KillstreakMilestoneTwo && KillStreak <= KillstreakMilestoneThree)// Triple kill
        {
            MiletstoneText.text = ("TRIPLE KILL");
            scoreMultiplier = ScoreMultiplierTwo;
        }

        if (KillStreak >= KillstreakMilestoneThree && KillStreak <= KillstreakMilestoneFour)// Penta kill
        {
            MiletstoneText.text = ("PENTAKILL");
            scoreMultiplier = ScoreMultiplierThree;
        }

        if (KillStreak >= KillstreakMilestoneFour && KillStreak <= KillstreakMilestoneFive)// Killing Spree
        {
            MiletstoneText.text = ("KILLING SPREE");
            scoreMultiplier = ScoreMultiplierFour;
        }

        if (KillStreak >= KillstreakMilestoneFive)// Genocide
        {
            MiletstoneText.text = ("GENOCIDE");
            scoreMultiplier = ScoreMultiplierFive;
        }
    }

    public void DepleteCooldownTimer()
    {
        KillcountTimer -= KillcountDepletionSpeed * Time.deltaTime;
        KilltreakTimerSlider.value = KillcountTimer;

        if (KillcountTimer < 0 )
        {
            KillcountTimer = 0;
            KillstreakPopup.SetActive(false);
        }
    }

    public void OnKillEnemy()
    {
        KillStreak += 1;
        Score = Score + (ScorePerKill * scoreMultiplier);
        KillcountTimer = KSCooldown;
    }

    public void OnKillBoss()
    {
        KillStreak += 1;
        Score = Score + ((ScorePerKill * KilledBossMultiplier) * scoreMultiplier);
        KillcountTimer = KSCooldown;
    }

    // Killstreak Logic

    public void UpdateKillstreakValues()
    {
        // Disable Pop Up When Timer Runs Out
        if (KillcountTimer <= 0f)
        {
            KillStreak = 0f;
            KillstreakPopup.SetActive(false);
        }
        
        // Enable Pop Up after meeting the first milestone
        if (KillStreak >= KillstreakMilestoneOne)
        {
            KillCountText.text = ("Kills: " + KillStreak);
            KillstreakPopup.SetActive(true);
        }
    }


}
