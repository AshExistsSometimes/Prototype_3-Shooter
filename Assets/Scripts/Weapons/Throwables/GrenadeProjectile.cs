using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public Vector3 targ;

    public float FuseTime = 3f;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void GrenadeThrown(float force)
    {

        transform.LookAt(targ);
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }
}
