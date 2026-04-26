using Encounter.NightDance.Core.Event.Payload;
using UnityEngine;
namespace Encounter.NightDance.Core.Event.Conditions
{
    public class MoveToPositionCondition : ICondition
    {
        public Vector2Int requiredPosition;
        public bool Evaluate(EventContext eventContext)
        {
            if(eventContext.payload is MovePayload moveContext)
            {
                return moveContext.distance > 0 && moveContext.destination == requiredPosition;
            }
            return false;
        }
    }
}