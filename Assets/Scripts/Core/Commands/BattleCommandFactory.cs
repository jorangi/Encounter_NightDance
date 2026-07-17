using System.Collections.Generic;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Commands;
using MessagePipe;
using UnityEngine;
using VContainer;
using R3;
using VContainer.Unity;

namespace Encounter.NightDance.Core.Commands
{
    public class BattleCommandFactory: IStartable
    {
        private readonly IPublisher<EventContext> _eventPublisher;
        private readonly FieldManager _fieldManager;
        [Inject]
        public BattleCommandFactory(IPublisher<EventContext> eventPublisher, FieldManager fieldManager)
        {
            _eventPublisher = eventPublisher;
            _fieldManager = fieldManager;
        }
        public MoveCommand CreateMoveCommand(Unit unit, Vector2Int targetPos, List<Vector2Int> path = null) => new(unit, _fieldManager, targetPos, _eventPublisher, path);

        public void Start(){}
    }
}