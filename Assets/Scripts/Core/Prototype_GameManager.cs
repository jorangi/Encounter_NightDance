using UnityEngine;
using Encounter.NightDance.Core.Commands;
using Encounter.NightDance.Character;
using Encounter.NightDance.UI;
namespace Encounter.NightDance.Core
{
    public class Prototype_GameManager : MonoBehaviour
    {
        [SerializeField]private Transform Focus;
        [SerializeField]private Transform FocusUnit;
        private CommandInvoker commandInvoker;
        [SerializeField]private Unit.Unit turnedUnit;
        [SerializeField]private Unit.Unit testUnit2;
        [SerializeField]private FieldManager fieldManager;
        private Vector2Int v = Vector2Int.zero; //테스트용
        private void Start()
        {
            Focus.transform.position = FocusUnit.transform.position;
            commandInvoker = new CommandInvoker();
            commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, new Vector2Int(7, 6)));
            commandInvoker.ExecuteCommand(new MoveCommand(testUnit2, fieldManager, new Vector2Int(1, 0)));
            FocusUnitService.SetFocus(turnedUnit);
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {   
                Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x+1, turnedUnit.Pos.y);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x-1, turnedUnit.Pos.y);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x, turnedUnit.Pos.y-1);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(turnedUnit.Pos.x, turnedUnit.Pos.y+1);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            
            if(Input.GetKeyDown(KeyCode.Z))
            {
                commandInvoker.Undo();
            }
            else if(Input.GetKeyDown(KeyCode.Y))
            {
                commandInvoker.Redo();
            }
        }
    }
}