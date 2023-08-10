using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheNeedleOfFate : SwordBase
{
    public override void Attack()
    {
        base.Attack();
        if(owner.oriData.HP <= Mathf.RoundToInt(owner.oriData.HP * 0.25f) && CalcDmg() >= target.oriData.HP)
        {
            owner.GetBuff("Fate", 1);
        }
    }
}
