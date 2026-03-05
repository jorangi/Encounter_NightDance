using System.Collections.Generic;
using Encounter.NightDance.Core.History;
using UnityEngine;

namespace Encounter.NightDance.Core.Commands
{
    public interface IUnitCommand
    {
        public Stack<IHistoryRecord> records {get;}
        public void Execute();
        public void Undo();
        public void Redo();
        public void AddRecord(IHistoryRecord record);
        public void ClearMemento();
    }
}