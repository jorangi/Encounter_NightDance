using System;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.UI
{
    /// <summary>
    /// 마우스 오버 등 Focus중인 유닛의 스탯을 관리?하는 서비스 클래스 (UI에서 Focus된 유닛의 스탯을 구독하여 UI 업데이트 등에 활용)
    /// </summary>
    public static class FocusUnitService
    {
        private static VitalUnitStat CurrentTarget {get; set;}
        public static event Action<VitalUnitStat> OnFocusChanged;
        public static void SetFocus(VitalUnitStat stat)
        {
            if(CurrentTarget == stat) return;
            CurrentTarget = stat;
            OnFocusChanged?.Invoke(CurrentTarget);
        }
        public static void ClearFocus()
        {
            CurrentTarget = null;
            OnFocusChanged?.Invoke(null);
        }
    }
}