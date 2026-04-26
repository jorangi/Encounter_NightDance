using System;
using System.Collections.Generic;

namespace Encounter.NightDance.Status
{
    public enum StatModifierType
    {
        Flat, //단순 값 증감 ex) +10, -3
        PercentAdd, //합연산 ex) 10퍼센트 버프2개 => 10 + 1 + 1 = 12
        PercentMul //곱연산 ex) 10퍼센트 버프2개 => (10 + 1) + 1.1 = 12.1 최종값에 연산적용
    }
    public class StatModifier
    {
        public readonly float Value;
        public readonly StatModifierType Type;
        public readonly StatType TargetStat;
        public readonly object Source;
        public StatModifier(float value, StatModifierType type, StatType target, object source)
        {
            Value = value;
            Type = type;
            TargetStat = target;
            Source = source;
        }
    }
}