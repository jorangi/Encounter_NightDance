using UnityEngine;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 타일에 올라가는 오브젝트 인터페이스
    /// </summary>
    public interface IFieldObject
    {
        /// <summary>
        /// 오브젝트의 위치 좌표
        /// </summary>
        public Vector2Int Pos{get; set;}
    }
}