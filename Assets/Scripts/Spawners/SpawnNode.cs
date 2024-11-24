using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{
    public enum SpawnType // List of things that can be spawned
    {
        Enemy,
        Pickup,
        Ammo,
        Health
    }
    public SpawnType MySpawnType;
    public List<GameObject> PotentialPickups = new List<GameObject>();

}
