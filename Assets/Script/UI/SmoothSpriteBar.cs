using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using UnityEngine;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// SpriteRenderer의 MaterialPropertyBlock을 이용한 부드러운 게이지(바) 클래스의
    /// </summary>
    public class SmoothSpriteBar : SpriteBar
    {
        private CancellationTokenSource _cts;
        public SmoothSpriteBar(SpriteRenderer spriteRenderer) : base(spriteRenderer){}

        public SmoothSpriteBar(SpriteRenderer spriteRenderer, int percentage) : base(spriteRenderer, percentage){}

        public SmoothSpriteBar(SpriteRenderer spriteRenderer, Percentage percentage) : base(spriteRenderer, percentage){}
        public override async UniTask SetGauge(Percentage percentage, CancellationToken ct = default)
        {
            Dispose(); // 이전 애니메이션 취소, CancellationTokenSource 정리
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            spriteRenderer.GetPropertyBlock(mpb);
            float startValue = mpb.GetFloat(FillAmountID);
            float endValue = percentage;
            float duration = 0.5f; //일단은 0.5초, 나중에 조절할지 생각해볼것
            float elapsed = 0f;
            while(elapsed < duration)
            {
                elapsed += Time.deltaTime;
                mpb.SetFloat(FillAmountID, Mathf.Lerp(startValue, endValue, elapsed / duration));
                spriteRenderer.SetPropertyBlock(mpb);
                await UniTask.Yield(PlayerLoopTiming.Update, _cts.Token);
            }
            mpb.SetFloat(FillAmountID, endValue);
            spriteRenderer.SetPropertyBlock(mpb);
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