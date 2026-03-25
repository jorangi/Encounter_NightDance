using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using UnityEngine;
namespace Encounter.NightDance.UI
{
    /// <summary>
    /// SpriteRenderer의 MaterialPropertyBlock을 이용한 게이지(바) 클래스
    /// </summary>
    public class SpriteBar : IUIBar
    {
        protected readonly SpriteRenderer spriteRenderer;
        protected static readonly int FillAmountID = Shader.PropertyToID("_fillAmount"); // 매번 프로퍼티 아이디를 만드는건 오버헤드가 생기니까 static
        protected static MaterialPropertyBlock mpb; // MaterialPropertyBlock도 매번 새로 만드는건 오버헤드가 생기니까 static, 어짜피 SetPropertyBlock은 복사해서 쓰는거
        public SpriteBar(SpriteRenderer spriteRenderer)
        {
            this.spriteRenderer = spriteRenderer;
            mpb ??= new MaterialPropertyBlock();
            SetGauge(new Percentage(100));
        }
        public SpriteBar(SpriteRenderer spriteRenderer, int percentage)
        {
            this.spriteRenderer = spriteRenderer;
            mpb ??= new MaterialPropertyBlock();
            SetGauge(new Percentage((byte)percentage));
        }
        public SpriteBar(SpriteRenderer spriteRenderer, Percentage percentage)
        {
            this.spriteRenderer = spriteRenderer;
            mpb ??= new MaterialPropertyBlock();
            SetGauge(percentage);
        }
        public virtual UniTask SetGauge(Percentage percentage, float dur = -1f, CancellationToken ct = default, bool ignoreAnimation = false)
        {
            mpb.SetFloat(FillAmountID, percentage);
            spriteRenderer.SetPropertyBlock(mpb);
            return UniTask.CompletedTask;
        }
        public virtual void Dispose(){}
    }
}