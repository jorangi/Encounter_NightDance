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
        public override async UniTask SetGauge(Percentage percentage, float dur = -1f, CancellationToken ct = default, bool ignoreAnimation = false)
        {
            if(dur < 0) dur = 0.5f;
            if(ignoreAnimation) dur = 0f;
            Dispose(); // 이전 애니메이션 취소, CancellationTokenSource 정리
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            float startValue = image.fillAmount;
            float endValue = percentage;
            float elapsed = 0f;
            while(elapsed < dur)
            {
                elapsed += Time.deltaTime;
                image.fillAmount = Mathf.Lerp(startValue, endValue, elapsed / dur);
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