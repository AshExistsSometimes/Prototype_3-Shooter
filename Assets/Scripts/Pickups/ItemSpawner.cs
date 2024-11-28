using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public float SpawnCooldown = 60f;
    public GameObject ItemToSpawn;
    public bool SpawnOnStart = true;

    private void Start()
    {
        if (SpawnOnStart )
        {
            GameObject NewItem = Instantiate(ItemToSpawn, transform.position, transform.rotation);
        }
            StartCoroutine(SpawnItems());
    }

    public IEnumerator SpawnItems()
    {
        yield return new WaitForSeconds(SpawnCooldown);
        GameObject NewItem = Instantiate(ItemToSpawn, transform.position, transform.rotation);
        StartCoroutine(SpawnItems());
    }
}
