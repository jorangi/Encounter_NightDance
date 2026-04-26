using Encounter.NightDance.Status;
using Encounter.NightDance.Core.Event.Payload;
namespace Encounter.NightDance.Core.Event
{
    public class EventContext
    {
        public IUnitCore Source;
        public IUnitCore Target;
        public IEventPayload payload;
    }
}