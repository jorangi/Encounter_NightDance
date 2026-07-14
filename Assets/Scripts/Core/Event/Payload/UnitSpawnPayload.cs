using Encounter.NightDance.Character;
using UnityEngine;

namespace Encounter.NightDance.Core.Event.Payload
{
    public class UnitSpawnPayload : IEventPayload
    {
        public UnitData data;
        public Vector2Int position;
    }
}