using UnityEngine;
using Encounter.NightDance.Core.Commands;
using Encounter.NightDance.Character;
using Encounter.NightDance.UI;
using UnityEngine.InputSystem;
using System;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
using VContainer;
using System.Collections.Generic;
using MessagePipe;
using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Core.Event.Payload;
namespace Encounter.NightDance.Core
{
    public class Prototype_GameManager : MonoBehaviour
    {
        [SerializeField] private Transform Focus;
        [SerializeField] private Transform FocusUnit;        
        private CommandInvoker _commandInvoker;
        [SerializeField] private Unit turnedUnit;
        [SerializeField] private UnitData turnedUnitData;
        [SerializeField] private Unit testUnit2;
        [SerializeField] private UnitData testUnit2Data;

        private Vector2Int v = Vector2Int.zero; //테스트용
        private MainAction _mainAction;
        [SerializeField] private RouteRenderer routeRenderer;
         private IPublisher<EventContext> _eventPublisher;
         private BattleCommandFactory _battleCommandFactory;
        [Inject]
        public void Construct(IPublisher<EventContext> eventPublisher, CommandInvoker commandInvoker, BattleCommandFactory battleCommandFactory)
        {
            _eventPublisher = eventPublisher;
            _commandInvoker = commandInvoker;
            _battleCommandFactory = battleCommandFactory;
        }
        private void Awake()
        {
            _mainAction = new();
            routeRenderer ??= FindAnyObjectByType<RouteRenderer>();
        }
        private void Start()
        {
            Focus.transform.position = FocusUnit.transform.position;
            var testUnitSpawnContext = new EventContext
            {
                Source = null,
                Target = null,
                payload = new UnitSpawnPayload
                {
                    data = turnedUnitData,
                    position = new Vector2Int(7, 10)
                }
            };
            _eventPublisher.Publish(testUnitSpawnContext);
            //유닛 스폰테스트
            // commandInvoker.ExecuteCommand(_commandFactory.CreateMoveCommand(turnedUnit, new Vector2Int(7, 10)));
            // commandInvoker.ExecuteCommand(_commandFactory.CreateMoveCommand(testUnit2, new Vector2Int(1, 0)));
            
        }
        private void OnEnable()
        {
            _mainAction?.UnitControl.Enable();
            _mainAction.UnitControl.Move.performed += MoveForTest;
            _mainAction.UnitControl.Undo.performed += Undo;
            _mainAction.UnitControl.Redo.performed += Redo;
            _mainAction.UnitControl.Interact.performed += InteractForTest;
        }
        private void OnDisable()
        {
            _mainAction?.UnitControl.Disable();
            _mainAction.UnitControl.Move.performed -= MoveForTest;
            _mainAction.UnitControl.Undo.performed -= Undo;
            _mainAction.UnitControl.Redo.performed -= Redo;
            _mainAction.UnitControl.Interact.performed -= InteractForTest;
        }
        private void MoveForTest(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            if(dir == Vector2.zero) return;
            Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x + (int)dir.x, turnedUnit.Pos.y - (int)dir.y);
            v = clampedPos;
            _commandInvoker.ExecuteCommand(_battleCommandFactory.CreateMoveCommand(turnedUnit, clampedPos));
        }
        private void Undo(InputAction.CallbackContext context)
        {
            _commandInvoker.Undo();
        }
        private void Redo(InputAction.CallbackContext context)
        {
            _commandInvoker.Redo();
        }
        private void InteractForTest(InputAction.CallbackContext context)
        {
            turnedUnit.SetDestination(CameraService.CameraController.Pos);
            ProcessUnitMoveSchedules(turnedUnit);
        }
        private void ProcessUnitMoveSchedules(Unit unit)
        {
            if (unit == null || !unit.HasDestination) return;
            Vector2Int targetPos = unit.GetCurrentDestination.Value;
            var path = routeRenderer.GetRenderedPath();
            if (path == null || path.Count == 0)
            {
                unit.ClearDestination();
                Debug.LogWarning("해당 지점으로 이동할 수 없습니다.");
                return;
            }
            int movement = unit.GetFeature<IBaseStats>().Mobility.Value;
            unit.GetCurrentDestination.totalDistance = path.Count;
            Vector2Int clampedTarget = path[Mathf.Min(path.Count - 1, movement)];
            List<Vector2Int> clampedPath = path.GetRange(0, movement + 1);
            _commandInvoker.ExecuteCommand(_battleCommandFactory.CreateMoveCommand(unit, clampedTarget, clampedPath));
            if (unit.Pos == targetPos)
            {
                unit.ClearDestination();
            }
            (CameraService.CameraController as CameraController).RefreshFocus();
        }
    }
}