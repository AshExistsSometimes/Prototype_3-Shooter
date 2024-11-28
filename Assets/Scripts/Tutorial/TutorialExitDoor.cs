using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialExitDoor : MonoBehaviour
{
    public float KillsRequired = 6f;
    public float NumberOfEnemiesKilled = 0f;
    public UnityEvent SpawnBoss;

    public void EnemyKilled()
    {
        NumberOfEnemiesKilled += 1f;
        OpenDoorOnTargetReached();
    }

    public void OpenDoorOnTargetReached()
    {
        if (NumberOfEnemiesKilled >= KillsRequired)
        {
            SpawnBoss.Invoke();
        }
    }
}
