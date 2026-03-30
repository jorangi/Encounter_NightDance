using Encounter.NightDance.Core.Event;
using Encounter.NightDance.Status;

namespace Encounter.NightDance.Core.Effects
{
    public interface IEffect
    {
        public void Execute(EventContext context);
    }
}