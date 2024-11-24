using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPrefabScript : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(WaitAndCull());
    }

    private IEnumerator WaitAndCull()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
}
