using System;
using System.Collections.Generic;
using Encounter.NightDance.Status;
using R3;
using UnityEngine;


namespace Encounter.NightDance.Status
{
    [Serializable]
    public class Stat: IModifiableStat
    {
        public readonly ReactiveProperty<IModifiableStat> _onChangedSubject = new();
        public readonly ReactiveProperty<IModifiableStat> _onSyncSubject = new();
        public Observable<IModifiableStat> OnChangedAsObservable() => _onChangedSubject;// 단순 데이터 동기화용(스냅샷 적용 등)
        public Observable<IModifiableStat> OnSyncAsObservable() => _onSyncSubject;// 값의 변화마다 호출(로직에 의한 변화)
        private bool isDirty;
        private readonly List<StatModifier> statModifiers = new();
        public IReadOnlyList<StatModifier> Modifiers => statModifiers.AsReadOnly();
        [field: SerializeField]public int BaseValue{get; private set;}
        [field: SerializeField]private int value;
        /// <summary>
        /// 더티 플래그 true시 다시 계산
        /// </summary>
        [SerializeField] public int Value
        {
            get
            {
                if (isDirty)
                {
                    value = Calc();
                    isDirty = false;
                }
                return value;
            }
        }

        /// <summary>
        /// 모디파이어 추가
        /// </summary>
        /// <param name="mod"></param>
        public void AddModifier(StatModifier mod)
        {
            statModifiers.Add(mod);
            isDirty = true;
            _onChangedSubject.OnNext(this);
        }
        /// <summary>
        /// 특정 소스 제거할 때 연관된 모디파이어 제거
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool RemoveAllModifiersFromSource(object source)
        {
            int removeCount = statModifiers.RemoveAll(m => m.Source == source);
            if (removeCount > 0)
            {
                isDirty = true;
                _onChangedSubject.OnNext(this);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 스탯 + 모디파이어 계산
        /// </summary>
        /// <returns></returns>
        public int Calc()
        {
            float finalValue = BaseValue;
            float sumPercentAdd = 0;
            
            foreach (StatModifier mod in statModifiers)
            {
                if(mod.Type == StatModifierType.Flat) finalValue += mod.Value;
                else if(mod.Type == StatModifierType.PercentAdd) sumPercentAdd += mod.Value;
            }
            finalValue *= (1+sumPercentAdd);

            foreach(StatModifier mod in statModifiers)
            {
                if(mod.Type == StatModifierType.PercentMul) finalValue *= 1+mod.Value;
            }
            return Mathf.FloorToInt(finalValue);
        }
        public Stat(int baseValue)
        {
            this.BaseValue = baseValue;
            isDirty = true;
            _onSyncSubject.OnNext(this);
        }
        public void RestoreFromSnapshot(int baseVal, IEnumerable<StatModifier> mods)
        {
            this.BaseValue = baseVal;
            statModifiers.Clear();
            statModifiers.AddRange(mods);
            isDirty = true;
            _onSyncSubject.OnNext(this);
        }
        public void IncreaseBaseValue(int amount)
        {
            BaseValue += amount;
            isDirty = true;
            _onSyncSubject.OnNext(this);
            _onChangedSubject.OnNext(this);
        }
    }
}