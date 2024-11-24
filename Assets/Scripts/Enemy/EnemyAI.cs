using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

public class EnemyAI : Stats, IBurnable
{

    [SerializeField] private Stats myStats;
    public GameManager ScoreManager;

    [Header("Guard Specific")]
    public GameObject Character;
    public GameObject ParticleIfHeadshot;

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

    ////////////////////////////////////

    protected override void Start()
    {
        base.Start();
        MyNav.enabled = true;

        Character = GameObject.FindGameObjectWithTag("Player");
    }

    private void Update()
    {
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