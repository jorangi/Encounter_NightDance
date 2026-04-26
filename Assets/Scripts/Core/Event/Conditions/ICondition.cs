using Encounter.NightDance.Status;
using Encounter.NightDance.Core.Event;
namespace Encounter.NightDance.Core.Event.Conditions
{
    public interface ICondition
    {
        public bool Evaluate(EventContext eventContext);
    }
}