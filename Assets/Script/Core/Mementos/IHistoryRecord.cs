using System.Collections.Generic;
using Encounter.NightDance.Core.Snapshot;
using UnityEngine;

namespace Encounter.NightDance.Core.History
{
    public interface IHistoryRecord
    {
        public Dictionary<string, object> metadatas {get; set;}
        public ISnapshot snapshot {get; set;}
        public void Restore();
    }
}