using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Targets : Stats
{
    public UnityEvent OnShoot;

    private void Start()
    {
        CurHP = 1;
        base.Start();
    }
    private void Update()
    {
        if (CurHP <= 0)
        {
            DeathLogic();
        }
    }

    public void DeathLogic()
    {
        bIsDead = true;
        CurHP = 0;
        OnShoot.Invoke();
        StartCoroutine(WaitThenUnload());
    }
    public IEnumerator WaitThenUnload()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
