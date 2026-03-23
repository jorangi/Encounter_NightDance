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
        private readonly float _duration = 0.5f; // 애니메이션 지속 시간, 필요에 따라 조절 가능
        public SmoothSpriteBar(SpriteRenderer spriteRenderer) : base(spriteRenderer){}
        public SmoothSpriteBar(SpriteRenderer spriteRenderer, float duration) : base(spriteRenderer){_duration = duration;}
        public SmoothSpriteBar(SpriteRenderer spriteRenderer, int percentage) : base(spriteRenderer, percentage){}
        public SmoothSpriteBar(SpriteRenderer spriteRenderer, int percentage , float duration) : base(spriteRenderer, percentage){_duration = duration;}
        public SmoothSpriteBar(SpriteRenderer spriteRenderer, Percentage percentage) : base(spriteRenderer, percentage){}
        public SmoothSpriteBar(SpriteRenderer spriteRenderer, Percentage percentage, float duration) : base(spriteRenderer, percentage){_duration = duration;}
        public override async UniTask SetGauge(Percentage percentage, float dur = -1f, CancellationToken ct = default)
        {
            if(dur < 0) dur = _duration;
            Dispose(); // 이전 애니메이션 취소, CancellationTokenSource 정리
            _cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            spriteRenderer.GetPropertyBlock(mpb);
            float startValue = mpb.GetFloat(FillAmountID);
            float endValue = percentage;
            float elapsed = 0f;
            while(elapsed < dur)
            {
                elapsed += Time.deltaTime;
                mpb.SetFloat(FillAmountID, Mathf.Lerp(startValue, endValue, elapsed / dur));
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