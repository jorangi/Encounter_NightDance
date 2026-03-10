using Encounter.NightDance.Map;

namespace Encounter.NightDance.Core.Strategies
{
    public interface IMovementStrategy
    {
        public float Calc(ITile tile);
    }
}