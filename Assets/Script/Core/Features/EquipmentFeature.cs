using System;
using System.Collections.Generic;
using UnityEngine;
using Encounter.NightDance.Core.Datas;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Features
{
    public class EquipmentFeature : UnitFeatureBase
    {
        public Dictionary<EquipmentType, EquipmentData> _equippedItems = new();
        private IBaseStats _baseStats;
        private VitalityFeature _vitalityFeature;
        private MentalFeature _mentalFeature;
        public override void OnRegister(IUnitCore owner)
        {
            base.OnRegister(owner);
            _baseStats = owner.GetFeature<BaseStatFeature>();
            if(_baseStats == null) Debug.LogWarning("기본 스탯이 해당 유닛에게 존재하지 않습니다.");
            _vitalityFeature = owner.GetFeature<VitalityFeature>();
            if(_vitalityFeature == null) Debug.LogWarning("생존력이 해당 유닛에게 존재하지 않습니다.");
            _mentalFeature = owner.GetFeature<MentalFeature>();
            if(_mentalFeature == null) Debug.LogWarning("정신력이 해당 유닛에게 존재하지 않습니다.");
        }
        /// <summary>
        /// 장비를 착용하는 함수. 이미 해당 부위에 장비가 있다면 기존 장비를 해제한 후 새 장비를 착용한다.
        /// </summary>
        /// <param name="item"></param>
        public void Equip(EquipmentData item)
        {
            if(_equippedItems.ContainsKey(item.equipmentType))
                Unequip(item.equipmentType);
            _equippedItems[item.equipmentType] = item;
            foreach(StatModifier mod in item.statModifiers)
            {
                ApplyStatModifer(mod);
            }
            Debug.Log($"{item.equipmentType}의 장비 {item.Name}({item.MappingId})을(를) 착용했습니다.");
        }
        /// <summary>
        /// 장비의 스탯을 유닛의 Modifier에 추가하여 적용하는 함수
        /// </summary>
        /// <param name="mod"></param>
        private void ApplyStatModifer(StatModifier mod)
        {
            switch(mod.TargetStat)
            {
                case StatType.Intensity:
                    _baseStats?.Intensity.AddModifier(mod);
                    break;
                case StatType.Control:
                    _baseStats?.Control.AddModifier(mod);
                    break;
                case StatType.Speed:
                    _baseStats?.Speed.AddModifier(mod);
                    break;
                case StatType.Mobility:
                    _baseStats?.Mobility.AddModifier(mod);
                    break;
                case StatType.Vitality:
                    _vitalityFeature?.Vitality.MaxValue.AddModifier(mod);
                    break;
                case StatType.Mental:
                    _mentalFeature?.Mental.MaxValue.AddModifier(mod);
                    break;
            }
        }
        /// <summary>
        /// 장비를 해제하는 함수.
        /// </summary>
        /// <param name="type"></param>
        public void Unequip(EquipmentType type)
        {
            if(!_equippedItems.ContainsKey(type))
            {
                Debug.LogWarning($"착용중인 장비 중에 {type}의 장비가 없습니다.");
                return;
            }
            _equippedItems[type] = null;
            Debug.Log($"{type}의 장비를 해제했습니다.");
        }
    }
}