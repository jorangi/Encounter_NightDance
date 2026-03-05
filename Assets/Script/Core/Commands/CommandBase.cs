using System.Collections;
using System.Collections.Generic;
using Encounter.NightDance.Core.History;

namespace Encounter.NightDance.Core.Commands
{
    public abstract class CommandBase : IUnitCommand
    {
        public Stack<IHistoryRecord> records { get; } = new();
        public void AddRecord(IHistoryRecord record)
        {
            records.Push(record);
        }
        public void ClearMemento()
        {
            records.Clear();
        }
        public abstract void Execute();
        public abstract void Redo();
        public virtual void Undo()
        {
            while(records.Count > 0)
            {
                IHistoryRecord record = records.Pop();
                record.Restore();
            }
        }
    }
}