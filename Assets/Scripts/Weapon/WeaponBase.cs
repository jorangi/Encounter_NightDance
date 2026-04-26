using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBase : MonoBehaviour
{
    public string id;
    public Character owner;
    public Character target;
    public virtual void TurnStart()
    {

    }
    public virtual void TurnEnd()
    {

    }
    public virtual void Attack()
    {

    }
    public virtual void Hit()
    {

    }
    public virtual int CalcDmg()
    {
        return 0;
    }
}
