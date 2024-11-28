using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FloatingDamageLookAtPlayer : MonoBehaviour
{
    public Transform playerCamera;

    private void Start()
    {
        playerCamera = Camera.main.transform;
    }
    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(-(playerCamera.position - transform.position));
    }
}
