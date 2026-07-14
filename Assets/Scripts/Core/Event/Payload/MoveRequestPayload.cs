using System.Collections.Generic;
using UnityEngine;

namespace Encounter.NightDance.Core.Event.Payload
{
    /// <summary>
    /// 유닛 이동 요청을 전달하는 이벤트 페이로드
    /// </summary>
    public class MoveRequestPayload : IEventPayload
    {
        public Unit unit;
        public Vector2Int targetPos;
        public List<Vector2Int> path;
    }
}
