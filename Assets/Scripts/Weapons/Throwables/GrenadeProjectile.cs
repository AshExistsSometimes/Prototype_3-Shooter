using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeProjectile : BaseProjectile
{
	public Vector3 targ;

	public float FuseTime = 3f;

	private Rigidbody rb;

	public GameObject Explosion;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	public void GrenadeThrown(float force)
	{

		// rb.AddForce(transform.forward * force, ForceMode.Impulse);

	}

	private IEnumerator ExplodeAfterFuse()
	{
		yield return new WaitForSeconds(FuseTime);

		GameObject myExplosion = Instantiate(Explosion, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	public override void SetUpProjectile(Vector3 target)
	{
		targ = target;
		transform.LookAt(targ);
		StartCoroutine(ExplodeAfterFuse());
	}
}
