using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Core.Status;
using Encounter.NightDance.Status;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Encounter.NightDance.UI
{
    public class FocusUnitView : MonoBehaviour
    {
        [Space(10)]
        [Header("유닛 Vital & Mental")]
        [SerializeField]private GaugeBarUIView _vitalBar;
        [SerializeField]private GaugeBarUIView _mentalBar;
        [Space(5)]

        [Header("유닛 레벨, 경험치, 클래스, 이름")]
        [SerializeField]private SmoothImageBar _expBar;
        [SerializeField]private Image _expBarImage;
        //[SerializeField]private Image _expBar;
        [SerializeField]private TextMeshProUGUI _levelText;
        [SerializeField]private TextMeshProUGUI _classNameText;
        [SerializeField]private TextMeshProUGUI _unitNameText;

        [Space(5)]
        [Header("유닛 스탯")]
        [SerializeField]private TextMeshProUGUI _IntensityText;
        [SerializeField]private TextMeshProUGUI _ControlText;
        [SerializeField]private TextMeshProUGUI _SpeedText;
        [SerializeField]private TextMeshProUGUI _mobilityText;
        public void InitBars(Percentage vitalP, string vitalText, Percentage mentalP, string mentalText, Percentage _expBarP, string level)
        {
            _vitalBar.Initialize(vitalP, vitalText);
            _mentalBar.Initialize(mentalP, mentalText);
            _expBar = new(_expBarImage, _expBarP);
            _levelText.text = level;
        }
        public void SetUnitInfo(string nickname, string name, string className, int level)
        {
            _unitNameText.text = $"<size=70%>{nickname}</size>, {name}";
            _classNameText.text = className + " | ";
            _levelText.text = $"Lv: {level}";
        }
        public void SetLevel(int level) => _levelText.text = $"Lv: {level}";
        public void UpdateExperience(Percentage p, string text)
        {
            _expBar.SetGauge(p, 0.05f).Forget();
            _levelText.text = text;
        }
        public void UpdateVital(Percentage p, string text) => _vitalBar.UpdateGauge(p, text);
        public void UpdateMental(Percentage p, string text) => _mentalBar.UpdateGauge(p, text);

        public void UpdateIntensity(string intensity) => _IntensityText.text = $"강도: {intensity}";
        public void UpdateControl(string control) => _ControlText.text = $"제어: {control}";
        public void UpdateSpeed(string speed) => _SpeedText.text = $"속도: {speed}";
        public void UpdateMobility(string mobility) => _mobilityText.text = $"기동: {mobility}";
    }
}