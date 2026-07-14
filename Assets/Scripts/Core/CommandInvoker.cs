using System;
using System.Collections.Generic;
using Encounter.NightDance.Core.Commands;
using UnityEngine;
using VContainer.Unity;
namespace Encounter.NightDance.Core
{
    /// <summary>
    /// 커맨드 실행과 undo, redo 관리 담당 클래스
    /// </summary>
    public class CommandInvoker:IStartable
    {
        private Stack<IUnitCommand> undoStack = new Stack<IUnitCommand>();
        private Stack<IUnitCommand> redoStack = new Stack<IUnitCommand>();

        /// <summary>
        /// 커맨드 실행
        /// </summary>
        /// <param name="command">실행할 커맨드</param>
        /// <returns>커맨드 실행 성공 여부</returns>
        public bool ExecuteCommand(IUnitCommand command)
        {
            if (!command.CanExecute())
            {
                Debug.LogWarning($"{command.GetType().Name}을 실행할 수 없습니다.");
                return false;
            }
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
            return true;
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
        public void Start(){}
    }
}