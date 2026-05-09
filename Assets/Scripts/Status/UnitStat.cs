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
    public class UnitStat : MonoBehaviour, IUnitCore
    {
        [Header("기본 정보")]
        [SerializeField] protected UnitData baseData;
        private Dictionary<Type, IUnitFeature> Features {get; set;} = new();

        public void AddFeature<T>(T feature) where T : class, IUnitFeature
        {
            Features[typeof(T)] = feature;
        }
        public T GetFeature<T>() where T : class, IUnitFeature
        {
            if(Features.TryGetValue(typeof(T), out IUnitFeature feature))
            {
                return feature as T;
            }
            Debug.LogWarning($"해당 {typeof(T).Name}특성이 대상에게 존재하지 않습니다.");
            return null;
        }
        public T[] GetFeatures<T>() where T : class, IUnitFeature
        {
            if(Features.Count == 0)
            {
                Debug.LogWarning("대상에 어떤 특성도 존재하지 않습니다.");
                return Array.Empty<T>();
            }
            return Features.Values.OfType<T>().ToArray();
        }
        public void RemoveFeature<T>() where T : class, IUnitFeature
        {
            bool removed = Features.Remove(typeof(T));
            if(!removed) Debug.LogWarning($"해당 {typeof(T).Name}특성이 대상에게 존재하지 않습니다.");
            else Debug.Log($"해당 {typeof(T).Name}특성이 대상에게서 제거되었습니다.");
        }
        public void ClearFeature()
        {
            Features.Clear();
            Debug.Log("대상의 모든 특성이 제거되었습니다.");
        }
        public virtual void Awake()
        {
            BaseStatFeature baseStat = new(baseData);
            AddFeature<IBaseStats>(baseStat);

            LevelingFeature levelingFeature = new(this);
            levelingFeature.Activate();
            AddFeature(levelingFeature);

            VitalityFeature vitalityFeature = new(new ObjectHealth(baseData.MaxVitality), new Stat(baseData.GrowthVitality));
            AddFeature(vitalityFeature);

            MentalFeature mentalFeature = new(new ObjectMental(baseData.MaxMental), new Stat(baseData.GrowthMental));
            AddFeature(mentalFeature);
        }
        public void Update()
        {
            if(Input.GetKeyDown(KeyCode.L))
            {
                int val = 30;
                // Debug.Log($"경험치 {val} 획득");
                GetFeature<LevelingFeature>()?.GainExperience(val);
            }
        }
    }
}