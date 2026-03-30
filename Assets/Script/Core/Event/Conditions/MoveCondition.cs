using Encounter.NightDance.Core.Event.Payload;
namespace Encounter.NightDance.Core.Event.Conditions
{
    public class MoveCondition : ICondition
    {
        public int requiredDistance = 0;
        public bool Evaluate(EventContext eventContext)
        {
            if(eventContext.payload is MovePayload moveContext)
            {
                return moveContext.distance >= requiredDistance;
            }
            return false;
        }
    }
}