using UnityEngine;

namespace Encounter.NightDance.Core.Snapshot
{
    /// <summary>
    /// 범용 스냅샷 인터페이스
    /// </summary>
    public interface ISnapshot
    {
        public abstract int TargetId{get; set;}
        public abstract int Version{get;}
    }
}