using System;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI.View;

namespace Encounter.NightDance.UI.Presenter
{
    public class UnitHealthPresenter
    {
        private UnitHealthView _view;
        private VitalityFeature _vitalityFeature;
        public UnitHealthPresenter(UnitHealthView view, VitalityFeature vitalityFeature)
        {
            _view = view;
            _vitalityFeature = vitalityFeature;
            _vitalityFeature.Vitality.OnPercentageChanged += OnHpChanged;
        }
        private void OnHpChanged(Percentage percentage)
        {
            _view.UpdateGauge(percentage);
        }
        public void Dispose()
        {
            if(_vitalityFeature != null)
            {
                _vitalityFeature.Vitality.OnPercentageChanged -= OnHpChanged;
            }
        }
    }
}