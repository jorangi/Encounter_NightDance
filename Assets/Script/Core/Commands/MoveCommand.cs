using System;
using System.Collections.Generic;
using Encounter.NightDance.Character;
using Encounter.NightDance.Core.History;
using Unity.VisualScripting;
using UnityEngine;

namespace Encounter.NightDance.Core.Commands
{
    /// <summary>
    /// 유닛 이동 명령 커맨드
    /// </summary>
    public class MoveCommand : CommandBase
    {
        private readonly UnitController _unit;
        private readonly Vector2Int _targetPos;
        private readonly Vector2Int _startPos;
        /// <summary>
        /// 이동 명령 커맨드 생성자 - TODO: 필드 매니저를 받지 않고 좌표만 받아서 처리하게끔 수정
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="fieldManager"></param>
        /// <param name="targetPos"></param>
        public MoveCommand(UnitController unit, FieldManager fieldManager, Vector2Int targetPos)
        {
            _unit = unit;
            _targetPos = fieldManager.GetTilePos(targetPos);
            _startPos = unit.Pos;
        }
        public override void Execute()
        {
            //TODO: 이동 로직 구현
            _unit.MoveTo(_targetPos);
        }
        public override void Redo() => Execute();
    }
}