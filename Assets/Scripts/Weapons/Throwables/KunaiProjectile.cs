using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KunaiProjectile : MonoBehaviour
{
    [SerializeField] private SO_Gun GunData;

    public Vector3 targ;

    private Rigidbody rb;

    private float RicochetCount = 0f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        KunaiThrown();
        StartCoroutine(WaitGravThenCull());
    }

    private void Update()
    {
        // if the modle is not facing the Z direction add a offset
        transform.rotation = Quaternion.LookRotation(rb.velocity.normalized);
    }
    private void OnCollisionEnter(Collision collision)
    {
        RicochetCount += 1f;

        rb.velocity = Vector3.Reflect(transform.forward * rb.velocity.magnitude, collision.GetContact(0).normal);  

        if (collision.gameObject.CompareTag("Shootable"))
        {
            KunaiHit(collision);
        }

        if (RicochetCount >= 5f)
        {
            Destroy(gameObject);
        }
    }


    public void KunaiThrown()
    {
        transform.LookAt(targ);
        rb.AddForce(transform.forward * GunData.ProjectileSpeed, ForceMode.Impulse);
    }

    private IEnumerator WaitGravThenCull()
    {
        yield return new WaitForSeconds(0.75f);
        rb.useGravity = true;
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }

    public void KunaiHit(Collision Target)
    {
        Target.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);        
    }
}
