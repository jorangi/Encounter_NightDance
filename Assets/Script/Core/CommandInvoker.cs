using System;
using System.Collections.Generic;
using Encounter.NightDance.Core.Commands;
namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 커맨드 실행과 undo, redo 관리 담당 클래스
    /// </summary>
    public class CommandInvoker
    {
        private Stack<IUnitCommand> undoStack = new Stack<IUnitCommand>();
        private Stack<IUnitCommand> redoStack = new Stack<IUnitCommand>();

        public void ExecuteCommand(IUnitCommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }
        public void Undo()
        {
            if (undoStack.Count > 0)
            {
                IUnitCommand command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
        }
        public void Redo()
        {
            if (redoStack.Count > 0)
            {
                IUnitCommand command = redoStack.Pop();
                command.Redo();
                undoStack.Push(command);
            }
        }
    }
}