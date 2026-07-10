using UnityEngine;
using Encounter.NightDance.Core.Commands;
using Encounter.NightDance.Character;
using Encounter.NightDance.UI;
using UnityEngine.InputSystem;
using System;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Status;
namespace Encounter.NightDance.Core
{
    public class Prototype_GameManager : MonoBehaviour
    {
        [SerializeField] private Transform Focus;
        [SerializeField] private Transform FocusUnit;
        private CommandInvoker commandInvoker;
        [SerializeField] private Unit turnedUnit;
        [SerializeField] private Unit testUnit2;
        [SerializeField] private FieldManager fieldManager;
        private Vector2Int v = Vector2Int.zero; //테스트용
        private MainAction _mainAction;
        [SerializeField] private RouteRenderer routeRenderer;
        private void Awake()
        {
            _mainAction = new();
            routeRenderer ??= FindAnyObjectByType<RouteRenderer>();
        }
        private void Start()
        {

            Focus.transform.position = FocusUnit.transform.position;
            commandInvoker = new CommandInvoker();
            commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, new Vector2Int(7, 10)));
            commandInvoker.ExecuteCommand(new MoveCommand(testUnit2, fieldManager, new Vector2Int(1, 0)));
            FocusUnitService.SetFocus(turnedUnit);
        }
        private void OnEnable()
        {
            _mainAction?.UnitControl.Enable();
            _mainAction.UnitControl.Move.performed += MoveForText;
            _mainAction.UnitControl.Undo.performed += Undo;
            _mainAction.UnitControl.Redo.performed += Redo;
            _mainAction.UnitControl.Interact.performed += InteractForTest;
        }
        private void OnDisable()
        {
            _mainAction?.UnitControl.Disable();
            _mainAction.UnitControl.Move.performed -= MoveForText;
            _mainAction.UnitControl.Undo.performed -= Undo;
            _mainAction.UnitControl.Redo.performed -= Redo;
            _mainAction.UnitControl.Interact.performed -= InteractForTest;
        }
        private void MoveForText(InputAction.CallbackContext context)
        {
            var dir = context.ReadValue<Vector2>();
            Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x + (int)dir.x, turnedUnit.Pos.y - (int)dir.y);
            v = clampedPos;
            commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
        }
        private void Undo(InputAction.CallbackContext context)
        {
            commandInvoker.Undo();
        }
        private void Redo(InputAction.CallbackContext context)
        {
            commandInvoker.Redo();
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
            Vector2Int clampedTarget = path[Mathf.Min(path.Count - 1, movement)];
            MoveCommand moveCommand = new(unit, fieldManager, clampedTarget);
            commandInvoker.ExecuteCommand(moveCommand);
            if (unit.Pos == targetPos)
            {
                unit.ClearDestination();
            }
            (CameraService.CameraController as CameraController).RefreshFocus();
        }
    }
}