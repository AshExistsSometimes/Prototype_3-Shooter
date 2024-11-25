using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPGProjectile : MonoBehaviour
{
    [SerializeField] private SO_Gun GunData;

    public Vector3 targ;

    private Rigidbody rb;

    public GameObject Explosion;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        RocketFired();
        StartCoroutine(WaitGravThenCull());
    }

    private void Update()
    {
        // if the modle is not facing the Z direction add a offset
        transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
    private void OnCollisionEnter(Collision collision)
    {
        GameObject myExplosion = Instantiate(Explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }


    public void RocketFired()
    {
        transform.LookAt(targ);
        rb.AddForce(transform.forward * GunData.ProjectileSpeed, ForceMode.Impulse);
    }

    private IEnumerator WaitGravThenCull()
    {
        yield return new WaitForSeconds(5f);
        rb.useGravity = true;
    }
}
