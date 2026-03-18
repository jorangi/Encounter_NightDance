using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter.NightDance.UI
{
    [Serializable]
    /// <summary>
    /// 게이지바 UI 뷰 클래스 (체력, 정신력 등)
    /// </summary>
    public class GaugeBarUIView
    {
        [SerializeField]private Image _brightBar;
        [SerializeField]private Image _darkerBar;
        [SerializeField]private TextMeshProUGUI _valueText;
    }
}