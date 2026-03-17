using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// UGUI의 Image 컴포넌트를 이용한 부드러운 게이지(바) 클래스
    /// </summary>
    public class SmoothImageBar : ImageBar, IDisposable
    {
        private CancellationTokenSource _cts;
        public SmoothImageBar(Image image) : base(image){}
        public SmoothImageBar(Image image, int percentage) : base(image, percentage){}
        public SmoothImageBar(Image image, Percentage percentage) : base(image, percentage){}
        public override async UniTask SetGauge(Percentage percentage, CancellationToken ct = default)
        {
            Dispose(); // 이전 애니메이션 취소, CancellationTokenSource 정리
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            float startValue = image.fillAmount;
            float endValue = percentage;
            float duration = 0.5f; //일단은 0.5초, 나중에 조절할지 생각해볼것
            float elapsed = 0f;
            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(startValue, endValue, elapsed / duration);
                await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
            }
            image.fillAmount = endValue;
        }
        /// <summary>
        /// CancellationTokenSource를 이용한 애니메이션 취소 및 정리
        /// </summary>
        public override void Dispose()
        {
            if(_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = null;
            }
            base.Dispose();
        }
    }
}