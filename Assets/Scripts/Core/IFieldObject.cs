using UnityEngine;

namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 타일에 올라가는 오브젝트 인터페이스
    /// </summary>
    public interface IFieldObject
    {
        /// <summary>
        /// 오브젝트의 월드 transform 좌표
        /// </summary>
        public Vector2 WorldPos{get; set;}
        /// <summary>
        /// 오브젝트의 타일 기준 좌표
        /// </summary>
        public Vector2Int Pos{get; set;}
        public string ToString();
    }
}