using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerStats : Stats
{
	public Slider HPSlider;

	[Header("Invincibility Frames")]
	public bool InIFrames = false;
	public float InvulnerabilityCooldown = 0.5f;

	[Header("On Death")]
	public GameObject OilExplosion;
	public GameObject DeathScreenUI;

	[Header("Health")]
	public float HealthPerPickup = 200;

	[Header("Ammo")]
	public float MagsOnMe;
	public TMP_Text MagCounterUI;

	public bool PlayerIsDead = false;
	public bool CanReload = true;

	protected override void Start()
	{
		base.Start();
		MagsOnMe = 10f;
		PlayerIsDead = false;
		UpdateUI();
		CurHP = HP;
	}

	private void Update()
	{
		if (CurHP > HP)// Locks player to Max HP
		{
			CurHP = HP;
		}
		UpdateUI();

		if (MagsOnMe > 0)
		{
			CanReload = true;
		}
		else if (MagsOnMe <= 0)
		{
			CanReload = false;
		}
	}

	protected override void DeathLogic()
	{
		OnDeath();
	}

	public override void TakeDmg(float Dmg)
	{
		if (bIsDead || InIFrames) return;
		base.TakeDmg(Dmg);

		UpdateUI();

		StartCoroutine(IFrameCD());
	}

	public void IncreaseMags(float amount)
	{
		MagsOnMe += amount;
	}


	private void OnTriggerEnter(Collider other)// All triggers attatched // When an enemy touches me
	{
		// EnemyAI ENEMY = other.GetComponentInParent<EnemyAI>();
		// if (ENEMY != null && !InIFrames)
		// {
		// 	print(other.gameObject.name);
		// 	TakeDmg(UnityEngine.Random.Range(ENEMY.EnemyDmg.x, ENEMY.EnemyDmg.y + 1));// take random damage between 2 values of enemy damage
		// 	StartCoroutine(IFrameCD());
		// }
	}

	private void OnTriggerStay(Collider other)
	{
		// EnemyAI ENEMY = other.GetComponentInParent<EnemyAI>();
		// if (ENEMY != null && !InIFrames)
		// {
		// 	print(other.gameObject.name);
		// 	TakeDmg(UnityEngine.Random.Range(ENEMY.EnemyDmg.x, ENEMY.EnemyDmg.y + 1));// take random damage between 2 values of enemy damage
		// 	StartCoroutine(IFrameCD());
		// }
	}

	public IEnumerator IFrameCD()
	{
		InIFrames = true;
		yield return new WaitForSeconds(InvulnerabilityCooldown);
		InIFrames = false;
	}


	private void UpdateUI()
	{
		HPSlider.value = CurHP;
		MagCounterUI.text = ("Mags: " + MagsOnMe);
	}

	// Death Logic //
	public void OnDeath()
	{
		if (bIsDead) return;

		CurHP = 0;
		UpdateUI();

		// GetComponent<PlayerGunStats>().enabled = false;//Current Gun Stats
		GetComponent<PlayerController>().enabled = false;

		// Disable all mesh renderers
		foreach (var SkinMeshRenderer in GetComponentsInChildren<SkinnedMeshRenderer>())
		{
			SkinMeshRenderer.enabled = false;
		}
		foreach (var MeshRenderer in GetComponentsInChildren<MeshRenderer>())
		{
			MeshRenderer.enabled = false;
		}
		StartCoroutine(ExplosionThenDeathScreen());
		bIsDead = true;

	}

	public IEnumerator ExplosionThenDeathScreen()
	{
		OilExplosion.SetActive(true);
		yield return new WaitForSeconds(1.5f);
		DeathScreenUI.SetActive(true);
		yield return new WaitForSeconds(0.01f);
		PlayerIsDead = true;
	}

	public void Respawn()
	{
		//Temporary
		PlayerIsDead = false;
		SceneManager.LoadScene(1);
	}
}

