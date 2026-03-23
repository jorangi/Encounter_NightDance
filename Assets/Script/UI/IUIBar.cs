using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Encounter.NightDance.Core;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// 각종 UI의 게이지(바) 인터페이스
    /// </summary>
    public interface IUIBar: IDisposable
    {
        /// <summary>
        /// 게이지의 값(비율)을 설정하는 함수
        /// </summary>
        /// <param name="percentage"></param>
        public UniTask SetGauge(Percentage percentage, float dur = -1f, CancellationToken ct = default);
    }
}