using Encounter.NightDance.Character;
using Encounter.NightDance.Core.Features;
using Encounter.NightDance.Core.Strategies;
using Encounter.NightDance.Status;
using UnityEngine;

namespace Encounter.NightDance.Core.Features
{
    public class WalkingFeature : IMovable
    {
        
        public IMovementStrategy _movementStrategy { get; private set; }
        private readonly UnitController _unitController;
        public WalkingFeature(UnitController unitController, IMovementStrategy movementStrategy)
        {
            _unitController = unitController;
            _movementStrategy = movementStrategy;
        }
        public void MoveTo(Vector2 newPos) => _unitController.MoveTo(newPos);
        public void OnRegister(IUnitCore owner)
        {
        }
        public void OnUnregister(IUnitCore owner)
        {
        }
    }
}