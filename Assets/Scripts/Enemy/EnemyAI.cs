using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyAI : Stats, IBurnable, ISlowable
{
    private NavMeshAgent myMovement;
    public GameManager ScoreManager;

    [Space]
    public float MyScoreValue = 20f;
    [Space]

    [Header("Guard Specific")]
    public GameObject Character;
    public float mySpeed = 2.7f;
    private float SaveMySpeed;
    public bool IsRegularGuard = true;
    public bool IsBossEnemy = false;

    public GameObject MyFrozenParticle;
    public GameObject MyPoisonParticle;

    public UnityEvent WhenIDie;

    private float SpeedMultiplier = 1;

    public Vector2 EnemyDmg;

    public float YellInterval;
    private float CurYellInterval;
    public GameObject BloodExplosion;

    public Slider MyHPSlider;

    private int SaveMyDefence;

    // EFFECTS /////////////////////////

    // Burning
    private bool _isBurning;
    public bool IsBurning { get => _isBurning; set => _isBurning = value; }
    private Coroutine BurningCoroutine;

    // Slowing
    private bool _isSlowed;
    public bool IsSlowed { get => _isSlowed; set => _isSlowed = value; }
    private Coroutine SlowingCoroutine;

    private bool _slowing = false;
    public bool isFrozen = false;

    private float _slownessTimer;

    // Poison
    public int PoisonCounter = 0;
    private bool _isPoisoned = false;

    [Space]
    public float PoisonDmg = 3f;
    public float PoisonTime = 5;

    ////////////////////////////////////

    protected override void Start()
    {
        SaveMyDefence = DEF;
        MyHPSlider.maxValue = HP;

        base.Start();
        MyNav.enabled = true;
        myMovement = GetComponent<NavMeshAgent>();
        SaveMySpeed = mySpeed;
        

        Character = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
        myMovement.speed = mySpeed;

        if (bIsDead) return;

        MyNav.SetDestination(Character.transform.position);

        CurYellInterval += Time.deltaTime;
        if (CurYellInterval > YellInterval + UnityEngine.Random.Range(-.1f, .1f))
        {
            CurYellInterval = 0;
            SFXManager.TheSFXManager.PlaySFX("Yelling");
        }

        MyHPSlider.value = CurHP;

        if (_slownessTimer > 0f)
        {
            _slownessTimer -= Time.deltaTime;
        }
        else if (_isSlowed)
        {
            StopSlowing();
        }

        // Kills enemy if health is 0 or lower
        if (CurHP <= 0)
        {          
            DeathLogic();
        }
    }
    // EFFECTS LOGIC ///////////////////////////////////////////////

    // Burning //
    public void StartBurning(int DamagePerSecond)
    {
        IsBurning = true;
        if (BurningCoroutine != null)
        {
            StopCoroutine(BurningCoroutine);
        }

        BurningCoroutine = StartCoroutine(Burn(DamagePerSecond));
    }

    public void StopBurning()
    {
        IsBurning = false;
        if (BurningCoroutine != null)
        {
            StopCoroutine(BurningCoroutine);
        }
    }
    private IEnumerator Burn(int DamagePerSecond)
    {
        float minTimeToDmg = 1f / DamagePerSecond;
        WaitForSeconds wait = new WaitForSeconds(minTimeToDmg);
        int damagePerTick = Mathf.FloorToInt(minTimeToDmg) + 1;

        TakeDmg(damagePerTick);
        while (IsBurning)
        {
            yield return wait;
            TakeDmg(damagePerTick);
        }
    }


    // Slowing //
    public void StartSlowing(float SpeedModifier)
    {
        _slownessTimer = 1f;

        if (_slowing)
        {
            //StopCoroutine(SlowingCoroutine);
            return;
        }
        IsSlowed = true;
        SlowingCoroutine = StartCoroutine(CryoGunSlowBuildup());
    }

    public void StopSlowing()
    {
        //StartCoroutine(SlownessEndCooldown());
        //StopCoroutine(SlowingCoroutine);
    }

    public void FreezeMe()
    {
        if (isFrozen)
        {
            StartCoroutine(Freezing());
        }
    }

    IEnumerator CryoGunSlowBuildup()
    {
        if (!isFrozen)
        {
            _slowing = true;
            if (SpeedMultiplier > 0.5f)// If slowed less than 50%
            {
                yield return new WaitForSeconds(0.5f); // wait for 0.5s
                SpeedMultiplier -= 0.1f;// Decrease speed by 10%
                mySpeed = SaveMySpeed * SpeedMultiplier;
                myMovement.speed = mySpeed;
            }
            _slowing = false;
        }
    }
    IEnumerator SlownessEndCooldown()
    {
        yield return new WaitForSeconds(1f);
        IsSlowed = false;
        SpeedMultiplier = 1f;
        mySpeed = SaveMySpeed;
        myMovement.speed = mySpeed;
    }

    IEnumerator Freezing()
    {
        mySpeed = 0f;
        if (IsRegularGuard)
        {
            DEF = -1000;// Makes regular enemies get instakilled
        }
        else if (IsBossEnemy)
        {
            DEF = -5;// Makes Boss enemy take more damage
        }
        MyAnim.speed = 0;
        MyFrozenParticle.SetActive(true);
        yield return new WaitForSeconds(3f);
        mySpeed = SaveMySpeed;
        DEF = SaveMyDefence;
        MyAnim.speed = 1;
        MyFrozenParticle.SetActive(false);
        isFrozen = false;
    }

    // Freezing
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- FREEZING -----------------------------------------------------

    // Poison
    public void HitWithPoison()
    {
        if (PoisonCounter < 5)
        {
            PoisonCounter += 1;
        }
        else if (PoisonCounter >= 3)//makes it poison on shot 5, idk why
        {
            _isPoisoned = true;
            Poisoned();
        }
    }

    public void Poisoned()
    {
        if (_isPoisoned)
        {
            StartCoroutine(PoisonTick());
        }
    }

    private IEnumerator PoisonTick()
    {
        MyPoisonParticle.SetActive(true);
        for (int i = 0; i < PoisonTime; i++)
        {
            if (_isPoisoned)
            {
                yield return new WaitForSeconds(1f);
                TakeDmg(PoisonDmg);
            }
        }
        _isPoisoned = false;
        MyPoisonParticle.SetActive(false);
    }

    // Stunned
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- STUN ---------------------------------------------------------

    // Brainwashed
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- BRAINWASHED --------------------------------------------------

    ////////////////////////////////////////////////////////////////

    // DEATH LOGIC /////////////////////////////////////////////////
    protected override void DeathLogic()
    { 
        OnDeath();
        WhenIDie.Invoke();
        StopBurning();
    }
            
    public void OnDeath()
    {
        bIsDead = true;
        CurHP = 0;
        MyNav.enabled = false;


        foreach (var enemyHPUI in GetComponentsInChildren<Canvas>())
        {
            enemyHPUI.enabled = false;
        }
        foreach (var collider in GetComponentsInChildren<Collider>())
        {
            collider.enabled = false;
        }
        foreach (var SkinMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            SkinMeshRenderer.enabled = false;
        }
        foreach (var MeshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            MeshRenderer.enabled = false;
        }
        BloodExplosion.SetActive(true);
       
        GameManager.Instance.OnKillEnemy(MyScoreValue);
        
        StartCoroutine(WaitThenUnload());
    }
    public IEnumerator WaitThenUnload()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    /////////////////////////////////////////////
}
