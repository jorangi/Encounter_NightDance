using System;
using Encounter.NightDance.Status;
using R3;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// 마우스 오버 등 Focus중인 유닛의 스탯을 관리?하는 서비스 클래스 (UI에서 Focus된 유닛의 스탯을 구독하여 UI 업데이트 등에 활용)
    /// </summary>
    public static class FocusUnitService
    {
        public static IUnitCore CurrentTarget {get; private set;}
        private readonly static DisposableBag _disposables = new();
        private readonly static ReactiveProperty<IUnitCore> _onFocusChangedSubject = new(null);
        public static Observable<IUnitCore> OnFocusChangedAsObservable() => _onFocusChangedSubject;
        /// <summary>
        /// 대상 포커스 설정, 나중에 유닛을 조종하기 위해 선택하는 시스템과 함께 준비자세?(인게이지 참조)가 추가될 경우 조건을 추가해야할 듯함
        /// </summary>
        /// <param name="unit"></param>
        public static void SetFocus(IUnitCore unit)
        {
            if(unit == null)
            {
                ClearFocus();
                return;
            }
            if(CurrentTarget == unit) return;
            CurrentTarget = unit;
            _onFocusChangedSubject?.OnNext(CurrentTarget);
        }
        /// <summary>
        /// 포커스된 대상 클리어, private으로 하여 SetFocus(null)으로 실행
        /// </summary>
        private static void ClearFocus()
        {
            CurrentTarget = null;
            _onFocusChangedSubject?.OnNext(null);
        }
        /// <summary>
        /// 아직은 안쓰이기는 하는데, 일단 Dispose 구현
        /// </summary>
        private static void Dispose()
        {
            _disposables.Dispose();
        }
    }
}