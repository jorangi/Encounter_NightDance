using System;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI.View;
using R3;
using UnityEngine;

namespace Encounter.NightDance.UI.Presenter
{
    public class UnitVitalityPresenter
    {
        private UnitGaugeBarView _view;
        private VitalityFeature _vitalityFeature;
        private DisposableBag _vitalDisposal = new();
        public UnitVitalityPresenter(UnitGaugeBarView view, VitalityFeature vitalityFeature)
        {
            _view = view;
            _vitalityFeature = vitalityFeature;
            _vitalityFeature.Vitality.OnPercentageChangedAsObservable()
                .Subscribe(this, (p, state) => state._view.UpdateGauge(p))
                .AddTo(ref _vitalDisposal);
        }
        public void Dispose()
        {
            _vitalDisposal.Dispose();
        }
    }
}