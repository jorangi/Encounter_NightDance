using System;
using Encounter.NightDance.Character;
using UnityEngine;

namespace Encounter.NightDance.Core
{
    public class ObjectHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private int maxHp = 100;
        private int curHp;
        public event Action OnDamaged;
        public event Action OnAlived;
        public event Action OnDied;
        public void TakeDamage(int damage)
        {
            curHp -= damage;
            curHp = Mathf.Max(curHp, 0);
            OnDamaged?.Invoke();
            if(IsDead) OnDied?.Invoke();
            else OnAlived?.Invoke();
        }
        public bool IsAlive => curHp > 0;
        public bool IsDead => curHp <= 0;
    }
}