using UnityEngine;
namespace Encounter.NightDance.Core.Event.Payload
{
    public class MovePayload : IEventPayload
    {
        public int distance;
        public Vector2Int origin;
        public Vector2Int destination;
    }
}