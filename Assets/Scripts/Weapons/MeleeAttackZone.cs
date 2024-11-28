using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttackZone : MonoBehaviour
{
    public float MeleeDamage;
    public float FireRate;
    public bool AttackConstantly;

    private bool running = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            if (!AttackConstantly && !running)
            {
                StartCoroutine(SingleAttack(enemy));
            }
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (AttackConstantly)
        {
            if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy) && !running)
            {
                StartCoroutine(DealConstantDamage(enemy));
                
            }
        }
    }
    private IEnumerator DealConstantDamage(EnemyAI enemy)
    {
        running = true;
        enemy.mySpeed = 1f;
        enemy.TakeDmg(MeleeDamage);
        yield return new WaitForSeconds(FireRate);
        running = false;
        enemy.mySpeed = enemy.SaveMySpeed;
    }

    private IEnumerator SingleAttack(EnemyAI enemy)
    {
        running = true;
        enemy.TakeDmg(MeleeDamage);
        yield return new WaitForSeconds(0.5f);
        gameObject.SetActive(false);
        running = false;
    }



}
