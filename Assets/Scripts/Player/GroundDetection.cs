using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public PlayerController player;

    private void Start()
    {
        player.IsGrounded = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        player.IsGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        player.IsGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        player.IsGrounded = false;
    }
}
