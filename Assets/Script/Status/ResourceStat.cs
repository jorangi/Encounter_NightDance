using System;
using System.Collections.Generic;
using Encounter.NightDance.Core;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    [Serializable]
    public abstract class ResourceStat
    {
        private Percentage _cachedPercentage;
        public event Action<Percentage> OnPercentageChanged;
        [field: SerializeField]public Stat MaxValue {get; private set;}
        [field: SerializeField]public int CurValue {get; private set;}
        public ResourceStat(int baseValue)
        {
            MaxValue = new(baseValue);
            CurValue = MaxValue.Value;
            MaxValue.OnChanged += () => NotifyPercentageChange();
        }
        protected void NotifyPercentageChange()
        {
            int currentRaw = MaxValue.Value > 0 ? CurValue * 100 / MaxValue.Value : 0;
            Percentage newPercentage = new(currentRaw);
            if(!_cachedPercentage.Equals(newPercentage))
            {
                OnPercentageChanged?.Invoke(new Percentage(currentRaw));
                _cachedPercentage = newPercentage;
            }
        }
        public virtual void TakeDamage(int value)
        {
            CurValue = Mathf.Clamp(CurValue - value, 0, MaxValue.Value);
            OnValueCheck(value);
        }
        public void ApplySnapshot(int curValue)
        {
            CurValue = Mathf.Clamp(curValue, 0, MaxValue.Value);
            int delta = CurValue - curValue;
            OnValueCheck(delta, isSlient: true);
        }
        /// <summary>
        /// 값 변화량 체크 함수, 기본적으로 NotifyPercentageChange를 호출하여 퍼센트 변화 이벤트를 발생시켜 UI 대응
        /// </summary>
        /// <param name="value"></param>
        public virtual void OnValueCheck(int value, bool isSlient = false)
        {
            NotifyPercentageChange();
        }
    }
}
