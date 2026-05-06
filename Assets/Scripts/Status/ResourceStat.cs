using System;
using System.Collections.Generic;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    [Serializable]
    public abstract class ResourceStat: IDisposable
    {
        private Percentage _cachedPercentage = new(100);
        private DisposableBag _statDisposables = new();
        private readonly ReactiveProperty<Percentage> _onPercentageChangedSubject = new();
        public Observable<Percentage> OnPercentageChangedAsObservable() => _onPercentageChangedSubject;
        [field: SerializeField]public Stat MaxValue {get; private set;}
        [field: SerializeField]public int CurValue {get; private set;}
        public ResourceStat(int baseValue)
        {
            _statDisposables = new();
            MaxValue = new(baseValue);
            CurValue = MaxValue.Value;
            _onPercentageChangedSubject = new(new Percentage(MaxValue.Value > 0 ? CurValue * 100 / MaxValue.Value : 0));
            MaxValue.OnChangedAsObservable()
                .Subscribe(this, (s, state) => state.NotifyPercentageChange(s))
                .AddTo(ref _statDisposables);
        }
        protected void NotifyPercentageChange(IModifiableStat stat)
        {
            int currentRaw = MaxValue.Value > 0 ? CurValue * 100 / MaxValue.Value : 0;
            Percentage newPercentage = new(currentRaw);
            if(!_cachedPercentage.Equals(newPercentage))
            {
                _onPercentageChangedSubject.OnNext(newPercentage);
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
            int oldValue = CurValue;
            CurValue = Mathf.Clamp(curValue, 0, MaxValue.Value);
            int delta = oldValue - CurValue;
            OnValueCheck(delta, isSilent: true);
        }
        /// <summary>
        /// 값 변화량 체크 함수, 기본적으로 NotifyPercentageChange를 호출하여 퍼센트 변화 이벤트를 발생시켜 UI 대응
        /// </summary>
        /// <param name="value"></param>
        public virtual void OnValueCheck(int value, bool isSilent = false)
        {
            NotifyPercentageChange(MaxValue);
        }
        /// <summary>
        /// ResourceStat은 구독 해제시 Dispose하여 이벤트 정리
        /// </summary>
        public virtual void Dispose()
        {
            _statDisposables.Dispose();
        }
    }
}
