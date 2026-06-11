using System;
using Encounter.NightDance.Core.Unit;
using UnityEngine;

namespace Encounter.NightDance.Core.Commands
{
    /// <summary>
    /// 유닛 이동 명령 커맨드
    /// </summary>
    public class MoveCommand : CommandBase
    {
        private readonly Unit.Unit _unit;
        private readonly Vector3 _targetPos;
        private readonly Vector2Int _startPos;
        private readonly Vector2Int _targetPosInt;
        private readonly FieldManager fieldManager;
        /// <summary>
        /// 이동 명령 커맨드 생성자 - TODO: 필드 매니저를 받지 않고 좌표만 받아서 처리하게끔 수정
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="fieldManager"></param>
        /// <param name="targetPos"></param>
        public MoveCommand(Unit.Unit unit, FieldManager fieldManager, Vector2Int targetPos)
        {
            _unit = unit;
            _targetPosInt = targetPos;
            _targetPos = CoordinateUtility.GetWorldPos(targetPos);
            _startPos = unit.Pos;
            this.fieldManager = fieldManager;
        }
        public override bool CanExecute()
        {
            if(!FieldManager.IsWithinField(_targetPosInt))
            {
                Debug.LogWarning($"[{_targetPosInt}]해당 위치에 타일이 존재하지 않습니다.");
                return false;
            }
            IFieldObject tileObject = fieldManager.GetTileObject(_targetPosInt);
            if(tileObject != null)
            {
                Debug.LogWarning($"{tileObject}이 이미 {_targetPosInt}에 존재하여 {_unit}이 이동할 수 없습니다.");
                return false;
            }
            return true;
        }
        public override void Execute()
        {
            //TODO: 이동 로직 구현
            fieldManager.SetObjectOnTile(_unit, _targetPosInt);
            _unit.MoveTo(_targetPos);
            _unit.SetPos(_targetPosInt);
        }
        public override void Redo() => Execute();
    }
}