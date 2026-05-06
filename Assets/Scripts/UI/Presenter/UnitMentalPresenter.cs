using System;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.UI.View;
using R3;

namespace Encounter.NightDance.UI.Presenter
{
    public class UnitMentalPresenter
    {
        private UnitGaugeBarView _view;
        private MentalFeature _mentalFeature;
        private DisposableBag _vitalDisposal = new();
        public UnitMentalPresenter(UnitGaugeBarView view, MentalFeature mentalFeature)
        {
            _view = view;
            _mentalFeature = mentalFeature;
            _mentalFeature.Mental.OnPercentageChangedAsObservable()
                .Subscribe(this, (p, state) => state._view.UpdateGauge(p))
                .AddTo(ref _vitalDisposal);
        }
        public void Dispose()
        {
            _vitalDisposal.Dispose();
        }
    }
}