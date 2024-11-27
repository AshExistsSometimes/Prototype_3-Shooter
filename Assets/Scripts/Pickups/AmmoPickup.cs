using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public float MagsGain = 5f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            IncreaseMagAmount(collision);
            Destroy(gameObject);
        }
    }

    public void IncreaseMagAmount(Collision Target)
    {
        {
            Target.transform.GetComponent<PlayerStats>()?.IncreaseMags(MagsGain);
        }
    }
}
