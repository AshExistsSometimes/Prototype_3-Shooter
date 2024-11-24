using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public PlayerStats player;

    public float healthgain;

    private void Awake()
    {
        player = GetComponent<PlayerStats>();
    }
    //public void OnPickup()
    //{

    //}

}
