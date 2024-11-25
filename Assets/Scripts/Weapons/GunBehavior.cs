using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GunBehavior : MonoBehaviour
{
    private PlayerStats playerStats;

    [Header("General Refs")]
    [SerializeField] private SO_Gun GunData;
    [SerializeField] private Transform projectileOrigin;
    [Space]
    public TMP_Text AmmoCount;
    public int CurrentAmmo;
    private float TimeSinceLastShot;
    [Space]
    public Camera PlayerCam;

    public GameObject ReloadTimerObject;
    public Slider ReloadTimer;

    private bool IsReloading = false;

    [Space]
    [Space]

    [Header("Effects")]
    public bool ApplyBurn = false;
    public bool ApplySlowed = false;
    public bool ApplyFreeze = false;
    public bool ApplyPoison = false;
    public bool ApplyStun = false;
    public bool ApplyBrainwash = false;

    [Space]
    [Space]

    // HOLD WEAPONS //

    [Header("Weapon Class: Hold")]

    [Space]
    public ParticleSystem ParticleShootingSystem;
    public ParticleSystem EffectAppliedSystemPrefab;
    [Space]
    private AttackRadius attackRadius;
    [Space]
    public int EffectDPS = 5;
    public float EffectDuration = 3f;
    public float SpeedModifier = 0f;

    private bool ReducingAmmo = false;

    private ObjectPool<ParticleSystem> EffectedPool;
    private Dictionary<EnemyAI, ParticleSystem> EnemyParticleSystems = new();

    [Space]
    [Space]

    // Projectile Weapons //

    [Header("Weapon Class: Projectile")]
    [Space]
    public bool HasChargeUp = false;
    public float MinThrowForce = 5f;
    public float MaxThrowForce = 100f;
    public float ThrowForceBuildUpTime = 3f;
    private bool ThrowReady = false;

    private float ThrowForce = 0f;

    [Space]
    [Space]
    public bool IsKunai;
    public bool IsGrenade;
    public bool IsMissile;
    public bool IsFlameOrb;
    public bool IsGrapple;


    /////////////////////////////////////////////////////////////////////////////////

    PlayerController pc;

    private void Start()
    {
        CurrentAmmo = GunData.ClipSize;
        ReloadTimerObject.SetActive(false);

        PlayerCam = Camera.main;

        attackRadius = GetComponentInChildren<AttackRadius>();

        PlayerAttack.reloadInput += StartReload;
        IsReloading = false;

        // Hold Weapons //
        if (GunData.ShootingType == SO_Gun.EShootType.Hold || GunData.ShootingType == SO_Gun.EShootType.Radius)
        {
            EffectedPool = new ObjectPool<ParticleSystem>(CreateEffectedSystem);
            attackRadius.OnEnemyEnter += RadiusStartDamagingEnemy; // ERROR
            attackRadius.OnEnemyExit += RadiusStopDamagingEnemy;
        }
        //////////////////



        pc = GameObject.Find("Player").GetComponent<PlayerController>();
    }

    private void Update()
    {
        ReloadTimer.maxValue = GunData.ReloadTime;
        UpdateReloadTimer();
        AmmoCount.text = ("Ammo: " + CurrentAmmo + "/" + GunData.ClipSize);
        TimeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(projectileOrigin.position, projectileOrigin.forward);

        //Thrown Weapons with charge up
        ChargeProjectileForce();

        ShootGun();
    }



    // RELOAD LOGIC //
    public void StartReload()
    {
        Debug.Log("Trying Reload");
        if (!IsReloading)//prevents reloading twice at the same time
        {
            ReloadTimerObject.SetActive(true);

            ReloadTimer.value = 0f;
            Debug.Log("Reloading");
            StartCoroutine(Reload());
        }
    }

    private IEnumerator Reload()
    {
        IsReloading = true;

        yield return new WaitForSeconds(GunData.ReloadTime);

        CurrentAmmo = GunData.ClipSize;
        ReloadTimerObject.SetActive(false);

        IsReloading = false;
    }

    public void UpdateReloadTimer()
    {
        ReloadTimer.value += Time.deltaTime;
    }


    // SHOOTING LOGIC //
    public bool CanShoot() => // Can Shoot if
        !IsReloading // Gun isn't currently reloading
        && TimeSinceLastShot > GunData.FireRate // Enough time has passed since last shot depending on guns fire rate
        && pc.IsAiming;


    public void ShootGun()
    {
        Debug.Log("Shoot Gun Is being Called");
        // INSTANT SHOT WEAPONS /////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Instant)
        {
            if (CurrentAmmo > 0 && CanShoot() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameObject NewBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));
                //NewBullet.transform.rotation = Quaternion.LookRotation(pc.targ);
                NewBullet.GetComponent<Bullet>().targ = pc.targ;
                SFXManager.TheSFXManager.PlaySFX("Gunshot");
                CurrentAmmo -= 1;
                TimeSinceLastShot = 0;

                if (pc.targOb != null)// if its not null
                    pc.targOb.GetComponent<Stats>()?.TakeDmg(GunData.Damage);// go to targets stats and damage it
            }
        }



        // CHARGE UP WEAPONS ///////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.ChargeUp)
        {
            if (CurrentAmmo > 0 && CanShoot() && Input.GetKey(KeyCode.Mouse0))
            {
                StartCoroutine(UseChargeWeapon());
            }
        }



        // BURST WEAPONS //////////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Burst)
        {
            if (CurrentAmmo > 0 && CanShoot() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameObject CenBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// Centre Shot

                GameObject RBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// Right SHot

                GameObject LBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// LEft SHot

                SFXManager.TheSFXManager.PlaySFX("Gunshot");
                CurrentAmmo -= 1;
                TimeSinceLastShot = 0;
                ///
                if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), PlayerCam.transform.forward, out RaycastHit hit, 999f))// Centre shot
                {
                    hit.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
                    CenBullet.GetComponent<Bullet>().targ = hit.point;
                }
                else
                {
                    CenBullet.GetComponent<Bullet>().targ = PlayerCam.transform.forward * 999f;
                }
                ///
                if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(45, PlayerCam.transform.up) * PlayerCam.transform.forward, out RaycastHit hit1, 999f)) // Right Shot
                {
                    hit1.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
                    RBullet.GetComponent<Bullet>().targ = hit1.point;
                }
                else
                {
                    RBullet.GetComponent<Bullet>().targ = Quaternion.AngleAxis(45, transform.up) * PlayerCam.transform.forward * 999f;
                }
                ///
                if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(-45, PlayerCam.transform.up) * PlayerCam.transform.forward, out RaycastHit hit2, 999f)) // Left Shot
                {
                    hit2.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
                    LBullet.GetComponent<Bullet>().targ = hit2.point;
                }
                else
                {
                    LBullet.GetComponent<Bullet>().targ = Quaternion.AngleAxis(-45, transform.up) * PlayerCam.transform.forward * 999f;
                }

                Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), PlayerCam.transform.forward * 1000, Color.red, 10);
                Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(45, PlayerCam.transform.up) * PlayerCam.transform.forward * 1000, Color.red, 10);
                Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(-45, PlayerCam.transform.up) * PlayerCam.transform.forward * 1000, Color.red, 10);

                //if (pc.targOb != null)// if its not null
                //    pc.targOb.GetComponent<Stats>().TakeDmg(GunData.Damage);// go to targets stats and damage it
            }
        }



        // PROJECTILE WEAPONS ////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Projectile)
        {
            if (CurrentAmmo > 0 && CanShoot() && Input.GetKeyDown(KeyCode.Mouse0))
            {
                if (IsKunai)
                {
                    GameObject NewProjectile = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));
                    NewProjectile.GetComponent<KunaiProjectile>().targ = pc.targ;
                    CurrentAmmo -= 1;
                    TimeSinceLastShot = 0;
                }

                if (IsMissile)
                {
                    GameObject NewProjectile = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));
                    NewProjectile.GetComponent<RPGProjectile>().targ = pc.targ;
                    CurrentAmmo -= 1;
                    TimeSinceLastShot = 0;
                }
            }
            // Charge Up Projectiles
            else if (CurrentAmmo > 0 && CanShoot() && Input.GetKeyUp(KeyCode.Mouse0))
            {
                if (IsGrenade && ThrowReady)
                {
                    GameObject NewProjectile = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));
                    NewProjectile.GetComponent<GrenadeProjectile>().targ = pc.targ;
                    NewProjectile.GetComponent<GrenadeProjectile>().GrenadeThrown(Mathf.Lerp(MinThrowForce, MaxThrowForce, ThrowForce));
                    CurrentAmmo -= 1;
                    TimeSinceLastShot = 0;
                    ThrowForce = 0f;
                }
            }
        }



        // HOLD WEAPONS //////////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Hold)
        {
            if (CurrentAmmo > 0 && CanShoot() && Input.GetKey(KeyCode.Mouse0))
            {
                print("Shooting");
                ParticleShootingSystem.gameObject.SetActive(true);
                attackRadius.gameObject.SetActive(true);
                StartCoroutine(HoldWeaponAmmoDepletion());
            }

            else
            {
                print("NOT shooting");
                ParticleShootingSystem.gameObject.SetActive(false);
                attackRadius.gameObject.SetActive(false);
            }
        }

        // RADIUS WEAPONS ///////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Radius)
        {

        }



        // MELEE WEAPONS /////////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Melee)
        {

        }



        // DASH WEAPONS /////////////////////////////////////////////////////////////
        if (GunData.ShootingType == SO_Gun.EShootType.Dash)
        {

        }



        ////////////////////////////

    }
    private void ChargeProjectileForce()
    {
        if ((GunData.ShootingType == SO_Gun.EShootType.Projectile && HasChargeUp) && Input.GetKey(KeyCode.Mouse0))
        {
            ThrowReady = true;
            ThrowForce += Time.deltaTime * (1 / ThrowForceBuildUpTime);

        }
    }

    private IEnumerator HoldWeaponAmmoDepletion()
    {
        if (CurrentAmmo > 0 && !ReducingAmmo)
        {
            ReducingAmmo = true;
            yield return new WaitForSeconds(0.25f);
            CurrentAmmo -= 1;
            ReducingAmmo = false;
        }
    }
    IEnumerator UseChargeWeapon()// For some reason doing bursts at higher fire rates
        {
            yield return new WaitForSeconds(GunData.ChargeTime);

            while (CurrentAmmo > 0 && CanShoot() && Input.GetKey(KeyCode.Mouse0))
            {
                GameObject NewBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));
                NewBullet.transform.rotation = Quaternion.LookRotation(pc.targ);
                SFXManager.TheSFXManager.PlaySFX("Gunshot");
                NewBullet.GetComponent<Bullet>().targ = pc.targ;
                CurrentAmmo -= 1;
                TimeSinceLastShot = 0;

                if (pc.targOb != null)// if its not null
                    pc.targOb.GetComponent<Stats>().TakeDmg(GunData.Damage);// go to targets stats and damage it
                yield return null;
            }
        }

    // Hold - Radius Weapons //
    private ParticleSystem CreateEffectedSystem()
    {
        return Instantiate(EffectAppliedSystemPrefab);
    }

    private void RadiusStartDamagingEnemy(EnemyAI enemy)
    {
        if (ApplyBurn)
        {
            if (enemy.TryGetComponent<IBurnable>(out IBurnable burnable))
            {
                burnable.StartBurning(EffectDPS);
                // Particles
                ParticleSystem effectAppliedSystem = EffectedPool.Get();
                effectAppliedSystem.transform.SetParent(enemy.transform, false);
                effectAppliedSystem.transform.localPosition = Vector3.zero;
                ParticleSystem.MainModule main = effectAppliedSystem.main;
                main.loop = true;
                EnemyParticleSystems.Add(enemy, effectAppliedSystem);
            }
        }

        else if (ApplySlowed)
        {
            SpeedModifier = 0f;
            if (enemy.TryGetComponent<ISlowable>(out ISlowable slowable))
            {
                slowable.StartSlowing(SpeedModifier);
                // Particles
                ParticleSystem effectAppliedSystem = EffectedPool.Get();
                effectAppliedSystem.transform.SetParent(enemy.transform, false);
                effectAppliedSystem.transform.localPosition = Vector3.zero;
                ParticleSystem.MainModule main = effectAppliedSystem.main;
                main.loop = true;
                EnemyParticleSystems.Add(enemy, effectAppliedSystem);
            }
        }

        else if (ApplyFreeze)
        {
            ///
        }

        else if (ApplyPoison)
        {
            ///
        }

        else if (ApplyStun)
        {
            ///
        }

        else if (ApplyBrainwash)
        {
            ///
        }
    }

    private IEnumerator DelayedDisableEffect(EnemyAI enemy, ParticleSystem Instance, float EffectDuration)
    {
        ParticleSystem.MainModule main = Instance.main;
        main.loop = false;
        yield return new WaitForSeconds(EffectDuration);
        Instance.gameObject.SetActive(false);
        if (ApplyBurn)
        {
            if (enemy.TryGetComponent<IBurnable>(out IBurnable burnable))
            {
                burnable.StopBurning();
                RadiusStopDamagingEnemy(enemy);// UNKNOWN IF WORKS OR NOT, CHECK HERE IF BROKEN
            }
        }
        else if (ApplySlowed)
        {
            if (enemy.TryGetComponent<ISlowable>(out ISlowable slowable))
            {
                slowable.StopSlowing();
                RadiusStopDamagingEnemy(enemy);// UNKNOWN IF WORKS OR NOT, CHECK HERE IF BROKEN
            }
        }
        else if (ApplyFreeze)
        {
            yield return null;
        }

        else if (ApplyPoison)
        {
            yield return null;
        }

        else if (ApplyStun)
        {
            yield return null;
        }

        else if (ApplyBrainwash)
        {
            yield return null;
        }
    }

    private void RadiusStopDamagingEnemy(EnemyAI enemy)
    {
        if (EnemyParticleSystems.ContainsKey(enemy))
        {
            StartCoroutine(DelayedDisableEffect(enemy, EnemyParticleSystems[enemy], EffectDuration));
            EnemyParticleSystems.Remove(enemy);
        }
    }
}
