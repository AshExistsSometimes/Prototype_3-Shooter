using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Pool;
using UnityEngine.UI;

public class WeaponBehavior : BaseWeapon
{
	public PlayerStats playerStats;

	[Header("General Refs")]
	public TMP_Text AmmoCount;
	public int CurrentAmmo;
	public Camera PlayerCam;
	public GameObject ReloadTimerObject;
	public Slider ReloadTimer;

	[SerializeField] private SO_Gun GunData;
	[SerializeField] private Transform projectileOrigin;

	private float TimeSinceLastShot;
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
	public bool IsCrossbow;
	[Space]
	[Space]

	[Header("Weapon Class: Radius")]
	private bool _CanUseRadius = true;


	/////////////////////////////////////////////////////////////////////////////////

	PlayerController pc;

	#region Start
	private void Start()
	{
		CurrentAmmo = GunData.ClipSize;
		ReloadTimerObject.SetActive(false);

		PlayerCam = Camera.main;

		attackRadius = GetComponentInChildren<AttackRadius>();

		// InputHanderler.reloadInput += StartReload;
		IsReloading = false;

		// Hold Weapons //
		if (GunData.ShootingType == SO_Gun.EShootType.Hold)
		{
			EffectedPool = new ObjectPool<ParticleSystem>(CreateEffectedSystem);
			attackRadius.OnEnemyEnter += RadiusStartDamagingEnemy;
			attackRadius.OnEnemyExit += RadiusStopDamagingEnemy;
		}
		//////////////////



		pc = GameObject.Find("Player").GetComponent<PlayerController>();
	}
	#endregion

	#region Update
	private void Update()
	{
		ReloadTimer.maxValue = GunData.ReloadTime;
		UpdateReloadTimer();
		AmmoCount.text = ("Ammo: " + CurrentAmmo + "/" + GunData.ClipSize);
		TimeSinceLastShot += Time.deltaTime;
		Debug.DrawRay(projectileOrigin.position, projectileOrigin.forward);

		//Thrown Weapons with charge up

	}
	#endregion

	#region StartReload
	// RELOAD LOGIC //
	public void StartReload()
	{
		if (!IsReloading && (playerStats.CanReload) && (CurrentAmmo < GunData.ClipSize))//prevents reloading twice at the same time / checks if the player has mags / Only reload if gun has less than max ammo
		{
			ReloadTimerObject.SetActive(true);

			ReloadTimer.value = 0f;
			StartCoroutine(Reload());
		}
	}
	#endregion

	#region IEnumerator Reload
	private IEnumerator Reload()
	{
		IsReloading = true;

		yield return new WaitForSeconds(GunData.ReloadTime);

		CurrentAmmo = GunData.ClipSize;
		playerStats.MagsOnMe -= 1f;
		ReloadTimerObject.SetActive(false);

		IsReloading = false;
	}
	#endregion

	#region UpdateReloadTimer
	public void UpdateReloadTimer()
	{
		ReloadTimer.value += Time.deltaTime;
	}
	#endregion

	#region bool CanShoot
	// SHOOTING LOGIC //
	public bool CanShoot()
	{
		return !IsReloading && TimeSinceLastShot > GunData.FireRate && pc.IsAiming;
	}
	#endregion

	#region ShootGun
	public void ShootGun(bool state)
	{
		// INSTANT SHOT WEAPONS /////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.Instant)
		{
			if (CurrentAmmo > 0 && CanShoot() && state)
			{

				if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out RaycastHit hit, 999f, pc.PlayerMask))
				{
					hit.transform.GetComponent<Stats>()?.TakeDmg(GunData.Damage);
					SpawnAndSetUpPrefab(hit.point, projectileOrigin.position, Quaternion.LookRotation(hit.point - projectileOrigin.position));

				}
				else
				{
					SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
				}

				SFXManager.TheSFXManager.PlaySFX("Gunshot");
				CurrentAmmo -= 1;
				TimeSinceLastShot = 0;

				// if (pc.targOb != null)// if its not null
				// 	pc.targOb.GetComponent<Stats>()?.TakeDmg(GunData.Damage);// go to targets stats and damage it
			}
		}



		// CHARGE UP WEAPONS ///////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.ChargeUp)
		{
			if (CurrentAmmo > 0 && CanShoot() && state)
			{
				StartCoroutine(UseChargeWeapon());
			}
		}



		// BURST WEAPONS //////////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.MultiShot)
		{
			if (CurrentAmmo > 0 && CanShoot() && state)
			{
				// GameObject CenBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// Centre Shot

				// GameObject RBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// Right SHot

				// GameObject LBullet = Instantiate(GunData.ProjectileType, projectileOrigin.position, Quaternion.LookRotation(projectileOrigin.forward));// LEft SHot

				SFXManager.TheSFXManager.PlaySFX("Gunshot");
				CurrentAmmo -= 1;
				TimeSinceLastShot = 0;
				///
				if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), PlayerCam.transform.forward, out RaycastHit hit, 999f))// Centre shot
				{
					hit.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
					// CenBullet.GetComponent<Bullet>().targ = hit.point;
					SpawnAndSetUpPrefab(hit.point, projectileOrigin.position, Quaternion.LookRotation(hit.point - projectileOrigin.position));
				}
				else
				{
					// CenBullet.GetComponent<Bullet>().targ = PlayerCam.transform.forward * 999f;
					SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
				}

				// (PlayerCam.transform.forward * 3) this adds a offset forward to the gun's barrel. It does not deal with the roation.
				if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(45, PlayerCam.transform.up) * PlayerCam.transform.forward, out RaycastHit hit1, 999f)) // Right Shot
				{
					hit1.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
					// RBullet.GetComponent<Bullet>().targ = hit1.point;
					SpawnAndSetUpPrefab(hit1.point, projectileOrigin.position, Quaternion.LookRotation(hit1.point - projectileOrigin.position));
				}
				else
				{
					// RBullet.GetComponent<Bullet>().targ = Quaternion.AngleAxis(45, transform.up) * PlayerCam.transform.forward * 999f;
					SpawnAndSetUpPrefab(PlayerCam.transform.position + (Quaternion.AngleAxis(45, transform.up) * (PlayerCam.transform.forward * 999f)), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
					// SpawnAndSetUpPrefab(Quaternion.AngleAxis(45, transform.up) * PlayerCam.transform.forward * 999f);
				}


				if (Physics.Raycast(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(-45, PlayerCam.transform.up) * PlayerCam.transform.forward, out RaycastHit hit2, 999f)) // Left Shot
				{
					hit2.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
					// LBullet.GetComponent<Bullet>().targ = hit2.point;
					// SpawnAndSetUpPrefab(hit2.point);
					SpawnAndSetUpPrefab(hit2.point, projectileOrigin.position, Quaternion.LookRotation(hit2.point - projectileOrigin.position));

				}
				else
				{
					// LBullet.GetComponent<Bullet>().targ = Quaternion.AngleAxis(-45, transform.up) * PlayerCam.transform.forward * 999f;
					// SpawnAndSetUpPrefab(Quaternion.AngleAxis(-45, transform.up) * PlayerCam.transform.forward * 999f);
					SpawnAndSetUpPrefab(PlayerCam.transform.position + (Quaternion.AngleAxis(-45, transform.up) * (PlayerCam.transform.forward * 999f)), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));

				}

				// Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), PlayerCam.transform.forward * 1000, Color.red, 10);
				// Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(45, PlayerCam.transform.up) * PlayerCam.transform.forward * 1000, Color.red, 10);
				// Debug.DrawRay(PlayerCam.transform.position + (PlayerCam.transform.forward * 3), Quaternion.AngleAxis(-45, PlayerCam.transform.up) * PlayerCam.transform.forward * 1000, Color.red, 10);

				//if (pc.targOb != null)// if its not null
				//    pc.targOb.GetComponent<Stats>().TakeDmg(GunData.Damage);// go to targets stats and damage it
			}
		}



		// PROJECTILE WEAPONS ////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.Projectile)
		{
			if (CurrentAmmo > 0 && CanShoot() && state)
			{

				GameObject go = SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
				go.GetComponent<Rigidbody>().AddForce(PlayerCam.transform.forward * GunData.ProjectileSpeed, ForceMode.Impulse);

				CurrentAmmo -= 1;
				TimeSinceLastShot = 0;

			}
			// Charge Up Projectiles
			else if (CurrentAmmo > 0 && CanShoot() && ThrowReady)
			{
				GameObject NewProjectile = SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
				NewProjectile.GetComponent<Rigidbody>().AddForce(PlayerCam.transform.forward * Mathf.Lerp(MinThrowForce, MaxThrowForce, ThrowForce), ForceMode.Impulse);
				CurrentAmmo -= 1;
				TimeSinceLastShot = 0;
				ThrowForce = 0f;

			}
		}



		// HOLD WEAPONS //////////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.Hold)
		{
			if (CurrentAmmo > 0 && CanShoot() && state)
			{
				if (ParticleShootingSystem != null) ParticleShootingSystem.gameObject.SetActive(true);
				if (attackRadius != null) attackRadius.gameObject.SetActive(true);
				StartCoroutine(HoldWeaponAmmoDepletion());
			}
			else
			{
				if (ParticleShootingSystem != null) ParticleShootingSystem.gameObject.SetActive(false);
				if (attackRadius != null) attackRadius.gameObject.SetActive(false);
			}
		}

		// RADIUS WEAPONS ///////////////////////////////////////////////////////////
		if (GunData.ShootingType == SO_Gun.EShootType.Radius)
		{
			if (CanShoot() && _CanUseRadius)
			{
				StartCoroutine(SpawnOneRadius());
			}
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
	#endregion

	#region SpawnAndSetUpPrefab
	private GameObject SpawnAndSetUpPrefab(Vector3 target, Vector3? spawnPos = null, Quaternion? spawnRotation = null)
	{
		if (!spawnPos.HasValue)
		{
			spawnPos = projectileOrigin.position;
		}

		if (!spawnRotation.HasValue)
		{
			spawnRotation = Quaternion.LookRotation(projectileOrigin.forward);
		}

		GameObject NewProjectile = Instantiate(GunData.ProjectileType, spawnPos.Value, spawnRotation.Value);
		NewProjectile.GetComponent<BaseProjectile>().SetUpProjectile(pc.targ);
		return NewProjectile;
	}
	#endregion

	#region ChargeProjectileForce
	private void ChargeProjectileForce(bool state)
	{
		if ((GunData.ShootingType == SO_Gun.EShootType.Projectile && HasChargeUp) && state)
		{
			ThrowReady = true;
			ThrowForce += Time.deltaTime * (1 / ThrowForceBuildUpTime);
		}
	}
	#endregion

	#region IEnumerator HoldWeaponAmmoDepletion
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
	#endregion

	#region FireAndSpawn
	/// <summary>
	/// Waiting for imp.
	/// </summary>
	/// <returns></returns>
	[Obsolete("Not sure of use case, dont use.")]
	public GameObject FireRaycastAndSpawn()
	{
		if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out RaycastHit hit, 999f, pc.PlayerMask))
		{
			return SpawnAndSetUpPrefab(hit.point, projectileOrigin.position, Quaternion.LookRotation(hit.point - projectileOrigin.position));
		}
		else
		{
			return SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
		}
	}
	#endregion

	#region IEnumerator UseChargeWeapon
	IEnumerator UseChargeWeapon()// For some reason doing bursts at higher fire rates
	{
		yield return new WaitForSeconds(GunData.ChargeTime);

		while (CurrentAmmo > 0 && CanShoot() && Input.GetKey(KeyCode.Mouse0))
		{
			if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out RaycastHit hit, 999f, pc.PlayerMask))
			{
				hit.transform.GetComponent<Stats>()?.TakeDmg(GunData.Damage);
				SpawnAndSetUpPrefab(hit.point, projectileOrigin.position, Quaternion.LookRotation(hit.point - projectileOrigin.position));
			}
			else
			{
				SpawnAndSetUpPrefab(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f), projectileOrigin.position, Quaternion.LookRotation(PlayerCam.transform.position + (PlayerCam.transform.forward * 999f) - projectileOrigin.position));
			}

			SFXManager.TheSFXManager.PlaySFX("Gunshot");
			CurrentAmmo -= 1;
			TimeSinceLastShot = 0;

			// if (pc.targOb != null)// if its not null
			// 	pc.targOb.GetComponent<Stats>().TakeDmg(GunData.Damage);// go to targets stats and damage it
			yield return null;
		}
	}
	#endregion

	#region ParticleSystem CreateEffectedSystem
	// Hold - Radius Weapons //
	private ParticleSystem CreateEffectedSystem()
	{
		return Instantiate(EffectAppliedSystemPrefab);
	}
	#endregion

	#region RadiusStartDamagingEnemy
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
	#endregion

	#region IEnumerator DelayedDisableEffect
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
				RadiusStopDamagingEnemy(enemy);
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
	#endregion

	#region IEnumerator SpawnOneRadius
	private IEnumerator SpawnOneRadius()
	{
		_CanUseRadius = false;
		GameObject NewRadius = Instantiate(GunData.ProjectileType, projectileOrigin.position, projectileOrigin.rotation);
		yield return new WaitForSeconds(3f);
		_CanUseRadius = true;
	}
	#endregion

	#region RadiusStopDamagingEnemy
	private void RadiusStopDamagingEnemy(EnemyAI enemy)
	{
		if (EnemyParticleSystems.ContainsKey(enemy))
		{
			StartCoroutine(DelayedDisableEffect(enemy, EnemyParticleSystems[enemy], EffectDuration));
			EnemyParticleSystems.Remove(enemy);
		}
	}
	#endregion

	#region Base Class Calls
	public override void Mouse0(bool status)
	{
		ChargeProjectileForce(status);
		ShootGun(status);
	}

	public override void Mouse1(bool status)
	{

	}

	public override void RKeyDown()
	{

		StartReload();
	}
	#endregion

	#region  OnEnable
	void OnEnable()
	{
		IsReloading = false;
		StopAllCoroutines();
	}
	#endregion
}
