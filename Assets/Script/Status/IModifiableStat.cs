using System;
using System.Collections.Generic;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Status
{
    public interface IModifiableStat
    {
        /// <summary>
        /// 모디파이어가 적용된 최종 스탯 값
        /// </summary>
        public int Value {get;}
        /// <summary>
        /// 모디파이어가 적용되기 전의 기본 스탯 값
        /// </summary>
        public int BaseValue {get;}
        /// <summary>
        /// 스탯 값이 변경될 때 이벤트
        /// </summary>
        public event Action<IModifiableStat> OnChanged;
        /// <summary>
        /// 스탯 값이 동기화될 때 이벤트(예: 스냅샷 적용 등)
        /// </summary>
        public event Action<IModifiableStat> OnSync;
        /// <summary>
        /// 스탯 모디파이어 추가, 모디파이어는 스탯 계산에 영향을 주며, 추가 시 스탯 값이 다시 계산됨
        /// </summary>
        /// <param name="mod"></param>
        public void AddModifier(StatModifier mod);
        /// <summary>
        /// 특정 소스에서 추가된 모든 모디파이어 제거, 제거 시 스탯 값이 다시 계산됨
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool RemoveAllModifiersFromSource(object source);
        public IReadOnlyList<StatModifier> Modifiers {get;}
    }
}