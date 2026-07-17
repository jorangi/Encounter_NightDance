using System;
using System.Collections.Generic;
using Encounter.NightDance.Core;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Event.Payload;
using MessagePipe;
using UnityEngine;

namespace Encounter.NightDance.Core.Commands
{
    /// <summary>
    /// 유닛 이동 명령 커맨드
    /// </summary>
    public class MoveCommand : CommandBase
    {
        private readonly Unit _unit;
        private readonly Vector3 _targetPos;
        private readonly Vector2Int _startPos;
        private readonly Vector2Int _targetPosInt;
        private readonly FieldManager fieldManager;
        private readonly IPublisher<EventContext> _eventPublisher;
        private readonly List<Vector2Int> _path;
        /// <summary>
        /// 이동 명령 커맨드 생성자 - TODO: 필드 매니저를 받지 않고 좌표만 받아서 처리하게끔 수정
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="fieldManager"></param>
        /// <param name="targetPos"></param>
        public MoveCommand(Unit unit, FieldManager fieldManager, Vector2Int targetPos, IPublisher<EventContext> publisher, List<Vector2Int> path = null)
        {
            _unit = unit;
            _targetPosInt = targetPos;
            _targetPos = CoordinateUtility.GetWorldPos(targetPos);
            _startPos = unit.Pos;
            this.fieldManager = fieldManager;
            _eventPublisher = publisher;
            _path = path ?? new List<Vector2Int>();
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
            _unit.GetCurrentDestination.passedDistance += _path.Count;
            var context = new EventContext
            {
                Source = _unit,
                Target = null,
                payload = new MovePayload
                {
                    distance = _path.Count,
                    origin = _startPos,
                    destination = _targetPosInt
                }
            };
            _eventPublisher.Publish(context);
        }
        public override void Undo()
        {
            fieldManager.SetObjectOnTile(_unit, _startPos);
            _unit.MoveTo(CoordinateUtility.GetWorldPos(_startPos));
            _unit.SetPos(_startPos);
        }
        public override void Redo() => Execute();
    }
}