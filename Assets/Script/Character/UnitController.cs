using Encounter.NightDance.Status;
using Encounter.NightDance.Core;
using UnityEngine;
using System;

namespace Encounter.NightDance.Character
{
    /// <summary>
    /// 유닛의 행동을 제어하는 컴포넌트 클래스
    /// </summary>
    [RequireComponent(typeof(UnitStat))]
    public class UnitController : Prototype_TileObject, IMovable
    {
        private UnitStat stat;

        public void MoveTo(Vector2Int newPos)
        {
            transform.position = new(newPos.x, 0, newPos.y);
            // TODO: 이동 실행, 애니메이션 등
        }
        private void Start()
        {
            stat = stat != null ? stat : gameObject.GetComponent<UnitStat>();
        }
    }
}
