using System;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public class ObjectHealth : IDamageable
    {
        public Stat MaxHP {get; private set;}
        public int CurHP{get; private set;}
        public event Action OnDamaged;
        public event Action OnHeal;
        public event Action OnAlived;
        public event Action OnDied;
        /// <summary>
        /// 피해를 입거나 회복하는 함수
        /// </summary>
        /// <param name="damage"></param>
        public void TakeDamage(int damage)
        {
            CurHP -= damage;
            CurHP = Mathf.Clamp(CurHP, 0, MaxHP.Value);
            if(damage > 0) OnDamaged?.Invoke();
            else if(damage < 0) OnHeal?.Invoke();
            if(IsDead) OnDied?.Invoke();
            else OnAlived?.Invoke();
        }
        public bool IsAlive => CurHP > 0;
        public bool IsDead => CurHP <= 0;
        public ObjectHealth(int baseValue)
        {
            MaxHP = new(baseValue);
            CurHP = MaxHP.Value;
        }
    }
}