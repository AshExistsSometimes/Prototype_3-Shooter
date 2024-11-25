using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlowable
{
    public bool IsSlowed { get; set; }

    public void StartSlowing(float SpeedModifier);

    public void StopSlowing();
}
