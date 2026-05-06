using Cysharp.Text;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using TMPro;
using UnityEngine;
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
            _vitalBar.Initialize(new Percentage(100), "100/100");
            _mentalBar.Initialize(new Percentage(100), "100/100");
            _expBar = new SmoothImageBar(_expBarImage, new Percentage(0));
        }
        public void SetUnitInfo(string nickname, string name, string className, int level)
        {
            _unitNameText.SetTextFormat("<size=70%>{0}</size>, {1}", nickname, name);
            _classNameText.SetTextFormat("{0} |", className);
            _levelText.SetTextFormat("Lv: {0}", level);
        }
        public void SetLevel(int level) => _levelText.SetTextFormat("Lv: {0}", level);
        public void InitializeExperience(Percentage p, string text)
        {
            _expBar.SetGauge(p, 0.05f, ignoreAnimation: true).Forget();
            _levelText.SetTextFormat("{0}", text);
        }
        public void UpdateExperience(Percentage p, string text, bool ignoreAnimation = false)
        {
            _expBar.SetGauge(p, 0.05f, ignoreAnimation: ignoreAnimation).Forget();
            _levelText.SetTextFormat("{0}", text);
        }
        public void InitializeVitality(Percentage p, string text) => _vitalBar.UpdateGauge(p, text, ignoreAnimation: true);
        public void UpdateVital(Percentage p, string text, bool ignoreAnimation = false) => _vitalBar.UpdateGauge(p, text, ignoreAnimation: ignoreAnimation);
        public void UpdateMental(Percentage p, string text, bool ignoreAnimation = false) => _mentalBar.UpdateGauge(p, text, ignoreAnimation: ignoreAnimation);
        public void InitializeMental(Percentage p, string text) => _mentalBar.UpdateGauge(p, text, ignoreAnimation: true);
        public void UpdateIntensity(string intensity) => _IntensityText.SetTextFormat("강도: {0}", intensity);
        public void UpdateControl(string control) => _ControlText.SetTextFormat("통제: {0}", control);
        public void UpdateSpeed(string speed) => _SpeedText.SetTextFormat("속도: {0}", speed);
        public void UpdateMobility(string mobility) => _mobilityText.SetTextFormat("기동: {0}", mobility);
    }
}

