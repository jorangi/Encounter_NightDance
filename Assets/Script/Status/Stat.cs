using System.Collections.Generic;
using UnityEngine;


namespace Encounter.NightDance.Status
{
    public class Stat
    {
        public int BaseValue{get; private set;}
        private int value;
        /// <summary>
        /// 더티 플래그 true시 다시 계산
        /// </summary>
        public int Value
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
            set => this.value = value;
        }
        private bool isDirty;
        private readonly List<StatModifier> statModifiers = new();
        /// <summary>
        /// 모디파이어 추가
        /// </summary>
        /// <param name="mod"></param>
        public void AddModifier(StatModifier mod)
        {
            statModifiers.Add(mod);
            isDirty = true;
        }
        /// <summary>
        /// 특정 소스 제거할 때 연관된 모디파이어 제거
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public bool RemoveAllModifiersFromSource(object source)
        {
            int removeCount = statModifiers.RemoveAll(m => m.Source == source);
            isDirty = removeCount > 0;
            return removeCount > 0;
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
                if(mod.Type == StatModifierType.PercentMul) finalValue *= (1+mod.Value);
            }
            return Mathf.FloorToInt(finalValue);
        }
    }
}