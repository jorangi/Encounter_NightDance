using System;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public class UnitPurification
    {
        public UnitPurification(Stat unitStat)
        {
            this.unitStat = unitStat;
        }
        private Stat unitStat;
        public event Action OnPure;
        private int purification = 0;
        /// <summary>
        /// 정화 함수, 정적 메서드 PurificationRule의 계산식을 사용
        /// </summary>
        /// <param name="CasterMental"></param>
        private void Pure(ObjectMental CasterMental)
        {
            purification += PurificationRule.PureCalculate(unitStat.Value, CasterMental.CurValue);
            OnPure?.Invoke();
        }
    }
}
