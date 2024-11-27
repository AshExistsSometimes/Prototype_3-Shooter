using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class FloatingDamageLookAtPlayer : MonoBehaviour
{
    public GameObject playerCamera;

    private void Start()
    {
        playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }
    private void Update()
    {
        transform.LookAt(-playerCamera.transform.position);
    }
}
