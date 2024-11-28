using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float MinScale = 0.5f;
    public float MaxScale = 10f;
    public float ExpansionSpeed = 1f;

    public float ExplosionHoldTime = 0.25f;

    public float ExplosionDamage = 40f;

    private float _time = 0f;

    private void Start()
    {
        transform.localScale = Vector3.Lerp(new Vector3(MinScale, MinScale, MinScale), new Vector3(MaxScale, MaxScale, MaxScale), _time);
    }

    private void Update()
    {
        _time += Time.deltaTime * (1 / ExpansionSpeed);
        transform.localScale = Vector3.Lerp(new Vector3(MinScale, MinScale, MinScale), new Vector3(MaxScale, MaxScale, MaxScale), _time);
        if (_time >= 1)
        {
            StartCoroutine(Waittodestroy());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Stats>(out Stats target))
        {
            print(target.transform.name);
            target.TakeDmg(ExplosionDamage);
        }
    }

    private IEnumerator Waittodestroy()
    {
        yield return new WaitForSeconds(ExplosionHoldTime);
        Destroy(gameObject);
    }

}
