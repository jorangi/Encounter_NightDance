using UnityEngine;
using Encounter.NightDance.Core.Commands;
using Encounter.NightDance.Character;
namespace Encounter.NightDance.Core
{
    public class Prototype_GameManager : MonoBehaviour
    {
        [SerializeField]private Transform Focus;
        [SerializeField]private Transform FocusUnit;
        private CommandInvoker commandInvoker;
        [SerializeField]private UnitController turnedUnit;
        [SerializeField]private FieldManager fieldManager;

        private Vector2Int v = Vector2Int.zero;
        private void Start()
        {
            Focus.transform.position = FocusUnit.transform.position;
            commandInvoker = new CommandInvoker();
            commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, new Vector2Int(0, 16)));
        }
        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(v.x+1, v.y);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(v.x-1, v.y);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(v.x, v.y-1);
                v = clampedPos;
                commandInvoker.ExecuteCommand(new MoveCommand(turnedUnit, fieldManager, clampedPos));
            }
            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                Vector2Int clampedPos = FieldManager.ClampToField(v.x, v.y+1);
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