using System;
using Encounter.NightDance.Character;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    [Serializable]
    public class ObjectMental: ResourceStat
    {
        public event Action OnDamaged;
        public event Action OnHealed;
        public event Action OnAlived;
        public event Action OnContamination;
        public bool IsAlive => CurValue > 0;
        public bool ProceedContamination => CurValue <= 0;
        
        public ObjectMental(int baseValue): base(baseValue){}
        /// <summary>
        /// 정신 피해 함수
        /// </summary>
        /// <param name="damage"></param>
        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
        }
        /// <summary>
        /// 정신 피해 값 변화 함수
        /// </summary>
        /// <param name="damage"></param>
        public override void OnValueCheck(int damage)
        {
            if(damage > 0)OnDamaged?.Invoke();
            else if(damage < 0) OnHealed?.Invoke();
            if(ProceedContamination) OnContamination?.Invoke();
            else OnAlived?.Invoke();
        }
    }
}