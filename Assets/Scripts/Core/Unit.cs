using System;
using System.Collections.Generic;
using System.Linq;
using Encounter.NightDance.Character;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.ScriptableObjects;
using Encounter.NightDance.Status;
using Encounter.NightDance.UI;
using Encounter.NightDance.UI.Presenter;
using Encounter.NightDance.UI.View;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Encounter.NightDance.Core
{
    [RequireComponent(typeof(UnitController))]
    [RequireComponent(typeof(UnitStat))]
    public class Unit : Prototype_TileObject, IUnitCore
    {
        [SerializeField] private MovementStrategySO _terrainCostSO;
        [SerializeField] private UnitController _unitController;
        [SerializeField] private UnitStat _stat;
        private Dictionary<Type, IUnitFeature> Features { get; set; } = new();
        [SerializeField] private UnitGaugeBarView _unitHealthView;
        [SerializeField] private UnitGaugeBarView _unitMentalView;
        private UnitVitalityPresenter _unitHealthPresenter;
        private UnitMentalPresenter _unitMentalPresenter;
        private MainAction _mainAction;
        private bool _isInitialized = false;
        private void Awake()
        {
            _mainAction = new();
        }
        public void AddFeature<T>(T feature) where T : class, IUnitFeature
        {
            Features[typeof(T)] = feature;
        }
        public T GetFeature<T>() where T : class, IUnitFeature
        {
            if (Features.TryGetValue(typeof(T), out IUnitFeature feature))
            {
                return feature as T;
            }
            Debug.LogWarning($"{name}에게 해당 {typeof(T).Name}특성이 대상에게 존재하지 않습니다.");
            return null;
        }
        public T[] GetFeatures<T>() where T : class, IUnitFeature
        {
            if (Features.Count == 0)
            {
                Debug.LogWarning("대상에 어떤 특성도 존재하지 않습니다.");
                return Array.Empty<T>();
            }
            return Features.Values.OfType<T>().ToArray();
        }
        public void RemoveFeature<T>() where T : class, IUnitFeature
        {
            bool removed = Features.Remove(typeof(T));
            if (!removed) Debug.LogWarning($"해당 {typeof(T).Name}특성이 대상에게 존재하지 않습니다.");
            else Debug.Log($"해당 {typeof(T).Name}특성이 대상에게서 제거되었습니다.");
        }
        public void ClearFeature()
        {
            Features.Clear();
            Debug.Log("대상의 모든 특성이 제거되었습니다.");
        }
        /// <summary>
        /// 유닛을 초기화하는 메서드
        /// </summary>
        /// <param name="data">유닛 데이터</param>
        public void Initialize(UnitData data)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            _stat ??= GetComponent<UnitStat>();
            _stat?.Initialize(data);

            _unitController ??= GetComponent<UnitController>();
            WalkingFeature walkingFeature = new(_unitController, new WalkingStrategy(MovementStrategyContainer.GetStrategySO(MovementType.Walking)));
            AddFeature(walkingFeature);

            BaseStatFeature baseStat = new(data);
            AddFeature<IBaseStats>(baseStat);

            LevelingFeature levelingFeature = new(this);
            levelingFeature.Activate();
            AddFeature(levelingFeature);

            VitalityFeature vitalityFeature = new(new ObjectHealth(data.MaxVitality), new Stat(data.GrowthVitality));
            AddFeature(vitalityFeature);

            MentalFeature mentalFeature = new(new ObjectMental(data.MaxMental), new Stat(data.GrowthMental));
            AddFeature(mentalFeature);

            FocusUnitService.SetFocus(this);
        }
        private void Start()
        {
            VitalityFeature vitalityFeature = GetFeature<VitalityFeature>();
            vitalityFeature.Activate();
            _unitHealthPresenter = new UnitVitalityPresenter(_unitHealthView, vitalityFeature);

            MentalFeature mentalFeature = GetFeature<MentalFeature>();
            mentalFeature.Activate();
            _unitMentalPresenter = new UnitMentalPresenter(_unitMentalView, mentalFeature);
        }
        private void OnEnable()
        {
            _mainAction?.UnitControl.Enable();
            _mainAction.UnitControl.L.performed += TestExperienceGet;
            _mainAction.UnitControl.H.performed += TestVitalityChange;
        }
        private void OnDisable()
        {
            _mainAction?.UnitControl.Disable();
            _mainAction.UnitControl.L.performed -= TestExperienceGet;
            _mainAction.UnitControl.H.performed -= TestVitalityChange;
        }
        private void TestVitalityChange(InputAction.CallbackContext context)
        {
            GetFeature<VitalityFeature>().TakeDamage(new Core.Datas.DamageData(null, UnityEngine.Random.Range(-101, 100), Core.Datas.DamageType.Vitality, false));
        }
        private void TestExperienceGet(InputAction.CallbackContext context)
        {
            int val = 30;
            // Debug.Log($"경험치 {val} 획득");
            GetFeature<LevelingFeature>()?.GainExperience(val);
        }
        private void OnDestroy()
        {
            _unitHealthPresenter.Dispose();
            _unitMentalPresenter.Dispose();
            _mainAction?.Dispose();
        }
        /// <summary>
        /// 유닛 이동 메서드, transform의 위치를 업데이트
        /// </summary>
        /// <param name="newPos"></param>
        public void MoveTo(Vector3 newPos)
        {
            _unitController.MoveTo(newPos);
        }
        private void SelectedUnit()
        {
        }
        public void OnRegister(IUnitCore owner)
        {
        }
        public void OnUnregister(IUnitCore owner)
        {
        }
    }
}