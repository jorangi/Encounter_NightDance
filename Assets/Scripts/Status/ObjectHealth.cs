using System;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI;
using R3;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    /// <summary>
    /// 유닛 생존 수치 스냅샷
    /// </summary>
    [Serializable]
    public class ObjectHealth: ResourceStat
    {
        private readonly ReactiveProperty<Unit> _onDamageSubject = new();
        private readonly ReactiveProperty<Unit> _onHealSubject = new();
        private readonly ReactiveProperty<Unit> _onDeathSubject = new();
        private readonly ReactiveProperty<Unit> _onAliveSubject = new();
        public Observable<Unit> OnDamagedAsObservable() => _onDamageSubject;
        public Observable<Unit> OnHealedAsObservable() => _onHealSubject;
        public Observable<Unit> OnDeathAsObservable() => _onDeathSubject;
        public Observable<Unit> OnAliveAsObservable() => _onAliveSubject;
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

            if(damage > 0) _onDamageSubject.OnNext(Unit.Default);
            else if(damage < 0) _onHealSubject.OnNext(Unit.Default);
            if(IsDead) _onDeathSubject.OnNext(Unit.Default);
            else _onAliveSubject.OnNext(Unit.Default);
        }
        public override void Dispose()
        {
            _onDamageSubject.Dispose();
            _onHealSubject.Dispose();
            _onDeathSubject.Dispose();
            _onAliveSubject.Dispose();
        }
    }
}