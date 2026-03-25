using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;
using UnityEngine.UI;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// UGUI의 Image 컴포넌트를 이용한 게이지(바) 클래스
    /// </summary>
    public class ImageBar : IUIBar
    {
        protected readonly Image image;
        public ImageBar(Image image)
        {
            this.image = image;
            SetGauge(new Percentage(100));
        }
        public ImageBar(Image image, int percentage)
        {
            this.image = image;
            SetGauge(new Percentage((byte)percentage));
        }
        public ImageBar(Image image, Percentage percentage)
        {
            this.image = image;
            SetGauge(percentage);
        }
        public virtual UniTask SetGauge(Percentage percentage, float dur = -1f, CancellationToken ct = default, bool ignoreAnimation = false)
        {
            image.fillAmount = percentage;
            return UniTask.CompletedTask;
        }
        public virtual void Dispose(){}
    }
}