using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnerActivationTrigger : MonoBehaviour
{
    public UnityEvent StartGame;

    private void OnTriggerEnter(Collider other)
    {
        StartGame.Invoke();
    }
}
