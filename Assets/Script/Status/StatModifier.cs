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
        public readonly object Source;
        public StatModifier(float value, StatModifierType type, object source)
        {
            Value = value;
            Type = type;
            Source = source;
        }
    }
    // public class StatModifier
    // {
    //     public int BaseValue;
    //     private int value;
    //     private bool isDirty;

    //     private readonly List<StatModifier> statModifiers = new();

    //     public int Value
    //     {
    //         get
    //         {
    //             if (isDirty)
    //             {
    //                 value = Calc();
    //                 isDirty = false;
    //             }
    //             return value;
    //         }
    //     }
    //     public void AddModifier(StatModifier mod)
    //     {
    //         statModifiers.Add(mod);
    //         isDirty = true;
    //     }
    //     public bool RemoveAllModifiersFromSource(object source)
    //     {
    //         foreach (StatModifier mod in statModifiers)
    //         {
    //             if(mod.)
    //         }
    //     }
    // }
}