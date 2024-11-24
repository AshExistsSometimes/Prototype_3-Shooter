using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

public class Stats : MonoBehaviour
{
    protected NavMeshAgent MyNav;
    protected CapsuleCollider MyCol;
    protected Animator MyAnim;

    [Header("Stats Global")]
    public float HP;
    protected float CurHP;
    internal bool bIsDead;

    public int DEF; // for every 1 point in DEf you get X DmgReduction
    public float DmgReductionPerPt = 0.05f;

    protected virtual void Start()
    {
        TryGetComponent(out MyNav);
        TryGetComponent(out MyCol);
        TryGetComponent(out MyAnim);

        CurHP = HP;
    }

    public virtual void TakeDmg(float Dmg)
    {
        CurHP -= (Dmg * (1 - (DEF * DmgReductionPerPt)));

        if (CurHP <= 0)
        {
            DeathLogic();
        }
    }

    protected virtual void DeathLogic()
    {
        throw new NotImplementedException();
    }
}