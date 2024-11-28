using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpawningLogic : MonoBehaviour
{

    public float SpawnInterval;
    private float CurSpawnInterval;

    public float SpawnIntervalWeap;
    private float CurSpawnIntervalWeap;

    private List<GameObject> SpawningPoints = new List<GameObject>();
    private List<GameObject> SpawningPointsWeaps = new List<GameObject>();

    public GameObject EnemyPrefab;

    private void Start()
    {
        foreach (var t in transform.GetComponentsInChildren<SpawnNode>())
        {
            if (t.gameObject.GetComponent<SpawnNode>().MySpawnType == SpawnNode.SpawnType.Enemy)
            {
                SpawningPoints.Add(t.gameObject);
            }
            else
            {
                SpawningPointsWeaps.Add(t.gameObject);
            }
        }


    }

    void Update()
    {
        CurSpawnInterval += Time.deltaTime;
        if (CurSpawnInterval > SpawnInterval)
        {
            CurSpawnInterval = 0;

            //get rand spawning point
            Vector3 SpawnLoc = Vector3.zero;
            int RandPoint = UnityEngine.Random.Range(0, SpawningPoints.Count);
            SpawnLoc = SpawningPoints[RandPoint].transform.position;

            //spawning
            GameObject NewEnemy = Instantiate(EnemyPrefab, SpawnLoc, Quaternion.identity);
        }


        CurSpawnIntervalWeap += Time.deltaTime;
        if (CurSpawnIntervalWeap > SpawnIntervalWeap)
        {
            CurSpawnIntervalWeap = 0;

            //get rand spawning point
            Vector3 SpawnLoc = Vector3.zero;
            int RandPoint = UnityEngine.Random.Range(0, SpawningPoints.Count);
            SpawnLoc = SpawningPoints[RandPoint].transform.position;

            //spawning

            int RandPickup = UnityEngine.Random.Range(0,
                SpawningPoints[RandPoint].GetComponent<SpawnNode>().PotentialPickups.Count);

            GameObject NewZombie = Instantiate(
                SpawningPoints[RandPoint].GetComponent<SpawnNode>().PotentialPickups[RandPickup]
                , SpawnLoc, Quaternion.identity);
        }
    }
}
