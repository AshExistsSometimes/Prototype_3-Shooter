using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBurnable
{
    public bool IsBurning { get; set; }

    public void StartBurning(int DamagePerSecond);

    public void StopBurning();
}

