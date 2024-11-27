using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{
    public Vector3 targ;

    [SerializeField] private SO_Gun GunData;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision collision)
    {
        rb.velocity = Vector3.zero;
        if (collision.gameObject.CompareTag("Shootable"))
        {
            BoltHit(collision);
        }
    }

    public void BoltFired(float force)
    {
        transform.LookAt(targ);
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        StartCoroutine(Cull());
    }

    public void BoltHit(Collision Target)
    {
        Target.transform.GetComponent<EnemyAI>()?.TakeDmg(GunData.Damage);
        Target.transform.GetComponent<EnemyAI>()?.HitWithPoison();
    }

    private IEnumerator Cull()
    {
        yield return new WaitForSeconds(10);
        Destroy(gameObject);
    }
}
