using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HealthPickup : MonoBehaviour
{
    public float HealthGain = 100f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HealPlayer(collision);
            Destroy(gameObject);
        }
    }

    public void HealPlayer(Collision Target)
    {
        {
            Target.transform.GetComponent<PlayerStats>()?.TakeDmg(-HealthGain);
        }
    }
}
