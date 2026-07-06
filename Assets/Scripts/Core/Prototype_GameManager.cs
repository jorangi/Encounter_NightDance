using UnityEngine;
using Encounter.NightDance.Core.Commands;
using Encounter.NightDance.Character;
using Encounter.NightDance.UI;
using UnityEngine.InputSystem;
namespace Encounter.NightDance.Core
{
    public class Prototype_GameManager : MonoBehaviour
    {
        [SerializeField] private Transform Focus;
        [SerializeField] private Transform FocusUnit;
        private CommandInvoker commandInvoker;
        [SerializeField] private Unit.Unit turnedUnit;
        [SerializeField] private Unit.Unit testUnit2;
        [SerializeField] private FieldManager fieldManager;
        private MainAction _mainAction;
        private Vector2Int v = Vector2Int.zero; //테스트용
        private void Awake()
        {
            _mainAction = new();
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
            _mainAction.UnitControl.Move.performed += MoveUnitForTest;
            _mainAction.UnitControl.Undo.performed += Undo;
            _mainAction.UnitControl.Redo.performed += Redo;

        }
        private void OnDisable()
        {
            _mainAction?.UnitControl.Disable();
            _mainAction.UnitControl.Move.performed -= MoveUnitForTest;
            _mainAction.UnitControl.Undo.performed -= Undo;
            _mainAction.UnitControl.Redo.performed -= Redo;
        }
        private void MoveUnitForTest(InputAction.CallbackContext context)
        {
            var moveDir = context.ReadValue<Vector2>();
            Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x + (int)moveDir.x, turnedUnit.Pos.y + (int)moveDir.y);
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
    }
}