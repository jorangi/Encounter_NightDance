using Encounter.NightDance.Core.Snapshot;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Snapshot
{
    /// <summary>
    /// 유닛 생존 수치 스냅샷
    /// </summary>
    public class UnitVitalSnapshot : UnitStatSnapshot
    {
        public override int Version => 1;
        public int CurValue{get; private set;}
        public bool IsDead{get; private set;}
        public bool IsAlive{get; private set;}
        public void Initialize(int targetId, ObjectHealth objectHealth)
        {
            base.Initialize(targetId, objectHealth.MaxValue);
            CurValue = objectHealth.CurValue;
            IsDead = objectHealth.IsDead;
            IsAlive = objectHealth.IsAlive;
        }
    }
}