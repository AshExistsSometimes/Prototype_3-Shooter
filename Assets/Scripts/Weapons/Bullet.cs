using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : BaseProjectile
{
    public Vector3 targ;

    [Header("Refs")]
    [SerializeField] private SO_Gun GunData;// Needs to be current gun, for debug purposes just revolver
    private Rigidbody rb;

    private float myMaxDistance;
    private float mySpeed;

    private float myDamage;
    private float myKnockback;
    private SO_Gun.EStatusGiven myEffect;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        // How far the bullet goes at what speed
        myMaxDistance = GunData.ProjectileMaxDistance;
        mySpeed = GunData.ProjectileSpeed;

        // How much ouchie the bullet does
        myDamage = GunData.Damage;
        myKnockback = GunData.Knockback;
        myEffect = GunData.StatusGiven;


        // then throw myself forward at speed for the distance and apply the ouchies to the enemy 

    }

    private void Start()
    {
        BulletShot();
        StartCoroutine(WaitThenCull());
    }

    private void FixedUpdate()
    {

        rb.velocity = transform.forward * rb.velocity.magnitude;
    }

    // on collision with object, if its shootable, apply ouchies
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Shootable"))
        {
            BulletHit();
        }
        Destroy(gameObject);
    }

    private IEnumerator WaitThenCull()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    public void BulletShot()
    {
        rb.AddForce(transform.forward * mySpeed, ForceMode.Impulse);

        // transform.LookAt(targ);
    }

    public void BulletHit()
    {
        // AS ENEMY TakeDmg(myDamage);
        // AS ENEMY Velocity = myKnockback * Bullet.forward
        // AS ENEMY Set status to MyEffect
    }


    public override void SetUpProjectile(Vector3 target)
    {

    }
}
