using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class AttackRadius : MonoBehaviour
{
    public delegate void EnemyEnteredEvent(EnemyAI Enemy);
    public delegate void EnemyExitedEvent(EnemyAI Enemy);

    public EnemyEnteredEvent OnEnemyEnter;
    public EnemyExitedEvent OnEnemyExit;

    private List<EnemyAI> EnemiesInRadius = new List<EnemyAI>();


    /////
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy)) 
        {
            EnemiesInRadius.Add(enemy);
            OnEnemyEnter?.Invoke(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            EnemiesInRadius.Remove(enemy);
            OnEnemyExit?.Invoke(enemy);
        }
    }

    private void OnDisable()
    {
        foreach(EnemyAI enemy in EnemiesInRadius)
        {
            OnEnemyExit?.Invoke(enemy);
        }
        EnemiesInRadius.Clear();
    }
}
