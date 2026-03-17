using System;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    /// <summary>
    /// 유닛 생존 수치 스냅샷
    /// </summary>
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
        /// 체력 값 변화 함수
        /// </summary>
        /// <param name="damage"></param>
        public override void OnValueCheck(int damage, bool isSilent = false)
        {
            base.OnValueCheck(damage, isSilent);
            if(isSilent) return; // isSilent이 true인 경우 이벤트 발생 없이 조용히 값만 변경

            if(damage > 0) OnDamaged?.Invoke();
            else if(damage < 0) OnHealed?.Invoke();
            if(IsDead) OnDied?.Invoke();
            else OnAlived?.Invoke();
        }
    }
}