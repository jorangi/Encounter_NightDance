using System;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Core.Status;
using Encounter.NightDance.Status;
using UnityEngine;

namespace Encounter.NightDance.UI.Presenter
{
    public class FocusUnitPresenter : IDisposable
    {
        private readonly FocusUnitView _view;
        private IUnitCore _unit;
        private IBaseStats _baseStats;
        private VitalityFeature _vitalityFeature;
        private MentalFeature _mentalFeature;
        private LevelingFeature _levelingFeature;
        public FocusUnitPresenter(FocusUnitView view)
        {
            _view = view;
            FocusUnitService.OnFocusChanged += BindUnit;
        }
        /// <summary>
        /// 이벤트들을 바인딩하는 메서드, FocusUnitService에서 포커스가 변경될 때마다 호출됨
        /// </summary>
        /// <param name="unit"></param>
        private void BindUnit(IUnitCore unit)
        {
            UnbindUnit();
            _unit = unit;
            if(_unit == null) return;
            _baseStats = _unit.GetFeature<IBaseStats>();
            _levelingFeature = _unit.GetFeature<LevelingFeature>();
            _vitalityFeature = _unit.GetFeature<VitalityFeature>();
            _mentalFeature = _unit.GetFeature<MentalFeature>();
            if (_baseStats != null)
            {
                _baseStats.Intensity.OnSync += UpdateIntensity;
                _baseStats.Control.OnSync += UpdateControl;
                _baseStats.Speed.OnSync += UpdateSpeed;
                _baseStats.Mobility.OnSync += UpdateMobility;
            }
            if (_levelingFeature.IsActive)
            {
                _levelingFeature.OnExperienceChanged += OnExperienceChanged;
                _levelingFeature.OnLevelChange += OnLevelChange;
            }
            if (_vitalityFeature?.Vitality != null)
            {
                _vitalityFeature.Vitality.OnPercentageChanged += OnVitalityChanged;
            }
            if (_mentalFeature?.Mental != null)
            {
                _mentalFeature.Mental.OnPercentageChanged += OnMentalChanged;
            }
            
            RefreshAll();
        }
        /// <summary>
        /// 이벤트들을 언바인딩하는 메서드, 새로운 유닛이 포커스될 때 기존 유닛에서 이벤트를 해제하기 위해 호출됨
        /// </summary>
        private void UnbindUnit()
        {
            if (_unit == null) return;

            if (_baseStats != null)
            {
                _baseStats.Intensity.OnSync -= UpdateIntensity;
                _baseStats.Control.OnSync -= UpdateControl;
                _baseStats.Speed.OnSync -= UpdateSpeed;
                _baseStats.Mobility.OnSync -= UpdateMobility;
            }

            if (_levelingFeature != null && _levelingFeature.IsActive)
            {
                _levelingFeature.OnExperienceChanged -= OnExperienceChanged;
                _levelingFeature.OnLevelChange -= OnLevelChange;
            }
            if (_vitalityFeature?.Vitality != null)
            {
                _vitalityFeature.Vitality.OnPercentageChanged -= OnVitalityChanged;
            }

            if (_mentalFeature?.Mental != null)
            {
                _mentalFeature.Mental.OnPercentageChanged -= OnMentalChanged;
            }
            _unit = null;
            _baseStats = null;
            _levelingFeature = null;
            _vitalityFeature = null;
            _mentalFeature = null;
        }
        /// <summary>
        /// 유닛의 정보와 스탯을 새로고침하는 메서드, 새로운 유닛이 포커스될 때 해당 유닛의 최신 정보를 UI에 반영하기 위해 호출됨
        /// </summary>
        private void RefreshAll()
        {
            _view.SetUnitInfo("예언의 아이", "호죽이", "용사", 1); 

            // 스탯 세팅
            if (_baseStats != null)
            {
                UpdateIntensity(_baseStats.Intensity);
                UpdateControl(_baseStats.Control);
                UpdateSpeed(_baseStats.Speed);
                UpdateMobility(_baseStats.Mobility);
            }
            if(_levelingFeature?.IsActive == true)
            {
                ApplyExperienceChanged(_levelingFeature.Experience, ignoreAnimation: true);
            }
            if(_vitalityFeature?.Vitality != null)
            {
                var vStat = _vitalityFeature.Vitality;
                Percentage vP = new(vStat.MaxValue.Value > 0 ? Mathf.RoundToInt(vStat.CurValue * 100 / vStat.MaxValue.Value) : 0);
                ApplyVitalityChanged(vP, true);
            }
            if(_mentalFeature?.Mental != null)
            {
                var mStat = _mentalFeature.Mental;
                Percentage mP = new(mStat.MaxValue.Value > 0 ? Mathf.RoundToInt(mStat.CurValue * 100 / mStat.MaxValue.Value) : 0);
                ApplyMentalChanged(mP, true);
            }
        }
        private void OnLevelChange(int lv)
        {
            _view.SetLevel(lv);
        }
        private void OnExperienceChanged(Percentage percentage)
        {
            ApplyExperienceChanged(percentage);
        }
        private void ApplyExperienceChanged(Percentage percentage, bool ignoreAnimation = false)
        {
            _view.UpdateExperience(percentage, $"Lv: {_levelingFeature.Level}", ignoreAnimation);
        }
        private void OnVitalityChanged(Percentage p)
        {
            ApplyVitalityChanged(p);
        }
        private void ApplyVitalityChanged(Percentage p, bool ignoreAnimation = false)
        {
            string text = $"{_vitalityFeature.Vitality.CurValue} / {_vitalityFeature.Vitality.MaxValue.Value}";
            _view.UpdateVital(p, text, ignoreAnimation);
        }
        private void OnMentalChanged(Percentage p)
        {
            ApplyMentalChanged(p);
        }
        private void ApplyMentalChanged(Percentage p, bool ignoreAnimation = false)
        {
            string text = $"{_mentalFeature.Mental.CurValue} / {_mentalFeature.Mental.MaxValue.Value}";
            _view.UpdateMental(p, text, ignoreAnimation);
        }
        private void UpdateIntensity(IModifiableStat stat) => _view.UpdateIntensity(stat.Value.ToString());
        private void UpdateControl(IModifiableStat stat) => _view.UpdateControl(stat.Value.ToString());
        private void UpdateSpeed(IModifiableStat stat) => _view.UpdateSpeed(stat.Value.ToString());
        private void UpdateMobility(IModifiableStat stat) => _view.UpdateMobility(stat.Value.ToString());
        public void Dispose()
        {
            UnbindUnit();
            FocusUnitService.OnFocusChanged -= BindUnit;
        }
    }
}