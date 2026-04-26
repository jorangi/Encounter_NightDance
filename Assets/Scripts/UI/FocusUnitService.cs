using System;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// 마우스 오버 등 Focus중인 유닛의 스탯을 관리?하는 서비스 클래스 (UI에서 Focus된 유닛의 스탯을 구독하여 UI 업데이트 등에 활용)
    /// </summary>
    public static class FocusUnitService
    {
        public static IUnitCore CurrentTarget {get; private set;}
        public static event Action<IUnitCore> OnFocusChanged;
        public static void SetFocus(IUnitCore unit)
        {
            if(CurrentTarget == unit) return;
            CurrentTarget = unit;
            OnFocusChanged?.Invoke(CurrentTarget);
        }
        public static void ClearFocus()
        {
            CurrentTarget = null;
            OnFocusChanged?.Invoke(null);
        }
    }
}