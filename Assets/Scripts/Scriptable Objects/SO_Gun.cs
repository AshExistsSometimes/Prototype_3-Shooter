using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun",
    menuName = "Scriptable Objects/Guns", order = 1)]
public class SO_Gun : ScriptableObject
{
    public string GunName;

    public enum EShootType
    {
        Instant,// Shoot Once On Click
        ChargeUp,// Hold For time before firing repeated shots
        MultiShot,// Fire Multiple Shots with one click
        Projectile,// Projectile is the damaging portion instead of a ray, IE Kunai or Crossbow
        Hold,// Fires Constantly when attack button held
        Radius,// Creates Sphere around player
        Melee,// No Projectile
        Dash,// Propels Player forward as projectile, uses melee stats
        Lightning
    }

    public enum EStatusGiven
    {
        None,
        Poisoning,
        Burning,
        Freeze,
        Stunned,
        Brainwashed
    }

    public EShootType ShootingType;

    public int GunID;


    [Header("Damage Stats")]
    [Range(0f, 100f)]

    public float Damage;// Damage Per Shot // Only Neccesary in raycast weapons

    public float FireRate;// Fire Rate
    public float Recoil;// Recoil

    public float Knockback; // Knockback done to enemies

    [Header("Radius Attack Stats")]
    public float AttackDiameter;

    [Header("Charge Up Attack Stats")]
    public float ChargeTime;

    [Header("Ammo Stats")]
    [Tooltip("If zero, then you don't reload. Else, you do.")]

    public int ClipSize;// Max Ammo

    public Vector2 ClipSize_OnPickup;

    public float ReloadTime;

    public GameObject ProjectileType;

    public float ProjectileMaxDistance;

    public float ProjectileSpeed = 10f;// set to 10m/s by default

    public EStatusGiven StatusGiven;

    [Header("Model Prefab")]
    public GameObject GunPrefab;
}
