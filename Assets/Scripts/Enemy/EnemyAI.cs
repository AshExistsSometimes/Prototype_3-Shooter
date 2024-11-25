using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyAI : Stats, IBurnable, ISlowable
{

    private NavMeshAgent myMovement;
    [SerializeField] private Stats myStats;
    public GameManager ScoreManager;

    [Header("Guard Specific")]
    public GameObject Character;
    public GameObject ParticleIfHeadshot;
    public float mySpeed = 2.7f;
    private float SaveMySpeed;

    private float SpeedMultiplier = 1;

    public Vector2 EnemyDmg;

    public float YellInterval;
    private float CurYellInterval;
    public GameObject BloodExplosion;

    public Slider MyHPSlider;

    // EFFECTS /////////////////////////

    // Burning
    private bool _isBurning;
    public bool IsBurning { get => _isBurning; set => _isBurning = value; }
    private Coroutine BurningCoroutine;

    // Slowing
    private bool _isSlowed;
    public bool IsSlowed { get => _isSlowed; set => _isSlowed = value; }
    private Coroutine SlowingCoroutine;

    ////////////////////////////////////

    protected override void Start()
    {
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
        IsSlowed = true;
        if (SlowingCoroutine != null)
        {
            StopCoroutine(SlowingCoroutine);
        }

        SlowingCoroutine = StartCoroutine(CryoGunSlowBuildup(SpeedMultiplier));
    }

    public void StopSlowing()
    {
        StartCoroutine(SlownessEndCooldown());
        StopCoroutine(SlowingCoroutine);
    }

    IEnumerator CryoGunSlowBuildup(float SpeedModifier)
    {
        if (SpeedMultiplier > 0.5f)// If slowed less than 50%
        {
            yield return new WaitForSeconds(0.5f); // wait for 0.5s
            SpeedMultiplier -= 0.1f;// Decrease speed by 10%
            mySpeed = mySpeed * SpeedMultiplier;
        }
    }
    IEnumerator SlownessEndCooldown()
    {
        yield return new WaitForSeconds(1f);
        IsSlowed = false;
        SpeedMultiplier = 1f;
        mySpeed = SaveMySpeed;
    }

    // Freezing
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- FREEZING -----------------------------------------------------

    // Poison
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- POISONING ----------------------------------------------------

    // Stunned
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- STUN ---------------------------------------------------------

    // Brainwashed
    // TO DO --------------------------------------------------------------------------------------------------------------------------------------- BRAINWASHED --------------------------------------------------

    ////////////////////////////////////////////////////////////////

    // DEATH LOGIC /////////////////////////////////////////////////
    protected override void DeathLogic()
    { 
        OnDeath();
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
        GameManager.Instance.OnKillEnemy();
        StartCoroutine(WaitThenUnload());
    }
    public IEnumerator WaitThenUnload()
    {
        yield return new WaitForSeconds(3.0f);
        Destroy(gameObject);
    }

    /////////////////////////////////////////////
}
