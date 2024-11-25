using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceRadiusAtk : MonoBehaviour
{
    public float MinScale = 0.5f;
    public float MaxScale = 10f;
    public float ExpansionSpeed = 1f;

    public float SpeedMultiplier = 0f;

    public float ExplosionDamage = 40f;

    private float _time = 0f;

    private List<EnemyAI> EnemiesInRadius = new List<EnemyAI>();

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
        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            EnemiesInRadius.Add(enemy);
            print(enemy.transform.name);
            enemy.TakeDmg(ExplosionDamage);
        }
    }

    private IEnumerator Waittodestroy()
    {
        yield return new WaitForSeconds(SpeedMultiplier);
        Destroy(gameObject);
    }

}
