using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
using UnityEditor.SettingsManagement;
using UnityEngine;
using UnityEngine.ResourceManagement.Exceptions;

namespace Encounter.NightDance.UI.View
{
    public class UnitGaugeBarView : MonoBehaviour
    {
        private SmoothSpriteBar _brightGaugeBar;
        private SmoothSpriteBar _darkerGaugeBar;
        [Header("게이지 바 렌더러")]
        [SerializeField] SpriteRenderer _brightGaugeBarRenderer;
        [SerializeField] SpriteRenderer _darkerGaugeBarRenderer;
        [Space(10)]
        [Header("유닛 스탯 컴포넌트")]
        private Percentage _cachedPercentage;
        private CancellationTokenSource _cts;
        [Space(10)]
        [Header("애니메이션 보간 속도 및 지연 시간")]
        [SerializeField] private float _fastDuration = 0.05f;
        [SerializeField] private float _slowDuration = 0.5f;
        [SerializeField] private float _delay = 0.5f;

        public void Start()
        {
            _brightGaugeBar = new SmoothSpriteBar(_brightGaugeBarRenderer, 0.001f);
            _darkerGaugeBar = new SmoothSpriteBar(_darkerGaugeBarRenderer, 0.5f);
        }
        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
        public void UpdateGauge(Percentage percentage)
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new();
            SetGauge(percentage, _cts.Token).Forget();
        }
        public async UniTaskVoid SetGauge(Percentage percentage, CancellationToken ct)
        {
            try
            {
                bool isDamage = percentage <= _cachedPercentage;
                _cachedPercentage = percentage;

                if(isDamage)
                {
                    _brightGaugeBar.SetGauge(percentage, _fastDuration).Forget();
                    await UniTask.Delay((int)(_delay * 1000), cancellationToken: ct);
                    await _darkerGaugeBar.SetGauge(percentage, _slowDuration);
                }
                else
                {
                    _darkerGaugeBar.SetGauge(percentage, _fastDuration).Forget();
                    await UniTask.Delay((int)(_delay * 1000), cancellationToken: ct);
                    await _brightGaugeBar.SetGauge(percentage, _slowDuration);
                }
            }
            catch(OperationCanceledException){}
            
        }
    }
}