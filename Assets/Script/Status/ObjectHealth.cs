using System;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    [Serializable]
    public class ObjectHealth: ResourceStat
    {
        public event Action OnDamaged;
        public event Action OnHealed;
        public event Action OnAlived;
        public event Action OnDied;
        public bool IsAlive => CurValue > 0;
        public bool IsDead => CurValue <= 0;
        public ObjectHealth(int baseValue): base(baseValue){}
        /// <summary>
        /// 체력 피해 함수
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
        /// <summary>
        /// 체력 피해 값 변화 함수
        /// </summary>
        /// <param name="damage"></param>
        public override void OnValueCheck(int damage)
        {
            if(damage > 0) OnDamaged?.Invoke();
            else if(damage < 0) OnHealed?.Invoke();
            if(IsDead) OnDied?.Invoke();
            else OnAlived?.Invoke();
        }
    }
}