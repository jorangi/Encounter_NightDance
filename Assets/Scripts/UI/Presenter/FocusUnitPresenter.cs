using System;
using R3;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
using UnityEngine;
using Cysharp.Text;

namespace Encounter.NightDance.UI.Presenter
{
    public class FocusUnitPresenter : IDisposable
    {
        private readonly FocusUnitView _view;
        private DisposableBag _unitDisposables;
        private DisposableBag _presentDisposables;
        private IUnitCore _unit;
        private IBaseStats _baseStats;
        private VitalityFeature _vitalityFeature;
        private MentalFeature _mentalFeature;
        private LevelingFeature _levelingFeature;
        public FocusUnitPresenter(FocusUnitView view)
        {
            _view = view;
            _presentDisposables = new();
            FocusUnitService.OnFocusChangedAsObservable()
                .Subscribe(this, (h, state) => state.BindUnit(h))
                .AddTo(ref _presentDisposables);
        }
        /// <summary>
        /// 이벤트들을 바인딩하는 메서드, FocusUnitService에서 포커스가 변경될 때마다 호출됨
        /// </summary>
        /// <param name="unit"></param>
        private void BindUnit(IUnitCore unit)
        {
            _unitDisposables.Dispose();
            _unitDisposables = new();
            if(unit == null) return;
            _unit = unit;
            _view.SetUnitInfo("예언의 아이", "니아", "용사", 1);
            BindBaseStats(unit.GetFeature<IBaseStats>());
            _baseStats = _unit.GetFeature<IBaseStats>();
            _levelingFeature = _unit.GetFeature<LevelingFeature>();
            _vitalityFeature = _unit.GetFeature<VitalityFeature>();
            _mentalFeature = _unit.GetFeature<MentalFeature>();
            if (_levelingFeature.IsActive)
            {
                _levelingFeature.OnExperienceChangedAsObservable().Subscribe(this, (p, state) => state.OnExperienceChanged(p)).AddTo(ref _unitDisposables);
                _levelingFeature.OnExperienceChangedAsObservable().Subscribe(this, (p, state) => state.ApplyExperienceChanged(p)).AddTo(ref _unitDisposables);
                _levelingFeature.OnLevelChangeAsObservable().Subscribe(this, (lv, state) => state.OnLevelChange(lv)).AddTo(ref _unitDisposables);
            }
            _vitalityFeature?.Vitality.OnPercentageChangedAsObservable()
                .Subscribe(p => _view.UpdateVital(p, ZString.Format("{0} / {1}", _vitalityFeature.Vitality.CurValue, _vitalityFeature.Vitality.MaxValue.Value), ignoreAnimation: false))
                .AddTo(ref _unitDisposables);
            _mentalFeature?.Mental.OnPercentageChangedAsObservable()
                .Subscribe(p => _view.UpdateMental(p, ZString.Format("{0} / {1}", _mentalFeature.Mental.CurValue, _mentalFeature.Mental.MaxValue.Value), ignoreAnimation: false))
                .AddTo(ref _unitDisposables);

            RefreshAll();
        }
        /// <summary>
        /// 유닛의 기본스탯을 바인딩하는 메서드 (BindUnit에서 파생)
        /// </summary>
        /// <param name="baseStats"></param>
        private void BindBaseStats(IBaseStats baseStats)
        {
            if(baseStats == null) return;
            baseStats.Intensity.OnSyncAsObservable()
                .Subscribe(stat => _view.UpdateIntensity(stat.Value.ToString()))
                .AddTo(ref _unitDisposables);
            baseStats.Control.OnSyncAsObservable()
                .Subscribe(stat => _view.UpdateControl(stat.Value.ToString()))
                .AddTo(ref _unitDisposables);
            baseStats.Speed.OnSyncAsObservable()
                .Subscribe(stat => _view.UpdateSpeed(stat.Value.ToString()))
                .AddTo(ref _unitDisposables);
            baseStats.Mobility.OnSyncAsObservable()
                .Subscribe(stat => _view.UpdateMobility(stat.Value.ToString()))
                .AddTo(ref _unitDisposables);
        }
        /// <summary>
        /// 유닛의 정보와 스탯을 새로고침하는 메서드, 새로운 유닛이 포커스될 때 해당 유닛의 최신 정보를 UI에 반영하기 위해 호출됨
        /// </summary>
        private void RefreshAll()
        {
            _view.SetUnitInfo("예언의 아이", "니아", "용사", 1); 

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
        private void ApplyVitalityChanged(Percentage p, bool ignoreAnimation = false)
        {
            string text = $"{_vitalityFeature.Vitality.CurValue} / {_vitalityFeature.Vitality.MaxValue.Value}";
            _view.UpdateVital(p, text, ignoreAnimation);
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
            _unitDisposables.Dispose();
            _presentDisposables.Dispose();
        }
    }
}