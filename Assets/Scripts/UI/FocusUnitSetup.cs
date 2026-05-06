using Encounter.NightDance.UI.Presenter;
using UnityEngine;

namespace Encounter.NightDance.UI
{
    public class FocusUnitSetup : MonoBehaviour
    {
        [SerializeField] private FocusUnitView _focusUnitView;
        private FocusUnitPresenter _presenter;
        private void Awake()
        {
            _presenter = new FocusUnitPresenter(_focusUnitView);
        }
        private void OnDestroy()
        {
            _presenter?.Dispose();
        }
    }
}