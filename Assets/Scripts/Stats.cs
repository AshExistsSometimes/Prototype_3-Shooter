using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class Stats : MonoBehaviour
{
    protected NavMeshAgent MyNav;
    protected CapsuleCollider MyCol;
    protected Animator MyAnim;

    public TMP_Text FloatingDamageText;


    [Header("Stats Global")]
    public float HP;
    protected float CurHP;
    internal bool bIsDead;

    public bool NeedsFloatingDamage = true;

    public int DEF; // for every 1 point in DEF you get X DmgReduction
    public float DmgReductionPerPt = 0.05f;

    protected virtual void Start()
    {
        TryGetComponent(out MyNav);
        TryGetComponent(out MyCol);
        TryGetComponent(out MyAnim);

        CurHP = HP;
        if (FloatingDamageText != null)
        {
            FloatingDamageText.text = ("");
        }

    }

    public virtual void TakeDmg(float Dmg)
    {
        CurHP -= (Dmg * (1 - (DEF * DmgReductionPerPt)));

        if (NeedsFloatingDamage && FloatingDamageText != null)
        {
            StartCoroutine(FloatingDamageAppearDissapear(Dmg));
        }

        if (CurHP <= 0)
        {
            DeathLogic();
        }
    }

    protected virtual void DeathLogic() { }

    private IEnumerator FloatingDamageAppearDissapear(float Dmg)
    {
        FloatingDamageText.text = ("");
        FloatingDamageText.text = ("-" + (Dmg * (1 - (DEF * DmgReductionPerPt))));
        yield return new WaitForSeconds(0.5f);
        FloatingDamageText.text = ("");
    }
}