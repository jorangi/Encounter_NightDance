using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI;
using UnityEngine;

namespace Encounter.NightDance.Status
{
    public enum StatType
    {
        Vitality,
        Mental,
        Intensity,
        Control,
        Speed,
        Mobility
    }
    /// <summary>
    /// 특성들을 컴포넌트 형식으로 관리하는 유닛 클래스(아마 최종?)
    /// </summary>
    public class UnitStat : MonoBehaviour
    {
        [Header("기본 정보")]
        [SerializeField] protected UnitData baseData;
        public UnitData BaseData => baseData;
    }
}