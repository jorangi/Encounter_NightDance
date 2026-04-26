using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Cysharp.Text;

namespace Encounter.NightDance.UI
{
    [Serializable]
    /// <summary>
    /// 게이지바 UI 뷰 클래스 (체력, 정신력 등)
    /// </summary>
    public class GaugeBarUIView
    {
        [Header("UI 컴포넌트")]
        [SerializeField]private Image _brightBarImage;
        [SerializeField]private Image _darkerBarImage;
        [SerializeField]private TextMeshProUGUI _valueText;
        [Space(10)]
        [Header("애니메이션 보간 속도 및 지연 시간")]
        [SerializeField]private float _fastDuration = 0.05f;
        [SerializeField]private float _slowDuration = 0.5f;
        [SerializeField]private float _delay = 0.5f;

        private SmoothImageBar _brightBar;
        private SmoothImageBar _darkerBar;
        private Percentage _cachedPercentage;
        private CancellationTokenSource _cts;

        public void Initialize(Percentage percentage, string text = "")
        {
            _brightBar = new SmoothImageBar(_brightBarImage, percentage);
            _darkerBar = new SmoothImageBar(_darkerBarImage, percentage);

            _cachedPercentage = percentage;
            _valueText.SetTextFormat("{0}", text);
        }
        public void UpdateGauge(Percentage percentage, string text = "", bool ignoreAnimation = false)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            _valueText.SetTextFormat("{0}", text);
            SetGauge(percentage, _cts.Token, ignoreAnimation).Forget();
        }
        private async UniTaskVoid SetGauge(Percentage percentage, CancellationToken token, bool ignoreAnimation = false)
        {
            try
            {
                bool isDamage = percentage <= _cachedPercentage;
                _cachedPercentage = percentage;
                if(isDamage)
                {
                    _brightBar.SetGauge(percentage, _fastDuration, ignoreAnimation: ignoreAnimation).Forget();
                    if(!ignoreAnimation) await UniTask.Delay((int)(_delay * 1000), cancellationToken: token);
                    await _darkerBar.SetGauge(percentage, _slowDuration, ignoreAnimation: ignoreAnimation);
                }
                else
                {
                    _darkerBar.SetGauge(percentage, _fastDuration, ignoreAnimation: ignoreAnimation).Forget();
                    if(!ignoreAnimation) await UniTask.Delay((int)(_delay * 1000), cancellationToken: token);
                    await _brightBar.SetGauge(percentage, _slowDuration, ignoreAnimation: ignoreAnimation);
                }
            }
            catch(OperationCanceledException){}
        }
        public void Dispose()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _brightBar.Dispose();
            _darkerBar.Dispose();
        }
    }
}