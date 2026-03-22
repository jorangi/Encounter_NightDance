using Encounter.NightDance.Status;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Encounter.NightDance.UI
{
    public class FocusUnitView : MonoBehaviour
    {
        [Header("유닛 Vital")]
        [SerializeField]private GaugeBarUIView _vitalBar;

        [Space(5)]
        [Header("유닛 Mental")]
        [SerializeField]private GaugeBarUIView _mentalBar;

        [Space(5)]
        [Header("유닛 레벨, 경험치, 클래스, 이름")]
        [SerializeField]private Image _expBar;
        [SerializeField]private TextMeshProUGUI _levelText;
        [SerializeField]private TextMeshProUGUI _classNameText;
        [SerializeField]private TextMeshProUGUI _unitNameText;

        [Space(5)]
        [Header("유닛 스탯")]
        [SerializeField]private TextMeshProUGUI _IntensityText;
        [SerializeField]private TextMeshProUGUI _ControlText;
        [SerializeField]private TextMeshProUGUI _SpeedText;
        [SerializeField]private TextMeshProUGUI _mobilityText;

        private void Awake()
        {
            //TODO: 각종 컴포넌트 유효성 검사
            FocusUnitService.OnFocusChanged += BindUnitStat;
        }
        private void BindUnitStat(IUnitCore unit)
        {
        }
    }
}