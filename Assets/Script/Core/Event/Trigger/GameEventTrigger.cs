using UnityEngine;
using System;
using Encounter.NightDance.Status;
namespace Encounter.NightDance.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GameEventTrigger")]
    public class GameEventTrigger : ScriptableObject
    {
        private event Action<IUnitCore, IUnitCore> OnEventTriggered;
        public void Subscribe(Action<IUnitCore, IUnitCore> listener) => OnEventTriggered += listener;
        public void Unsubcribe(Action<IUnitCore, IUnitCore> listener) => OnEventTriggered -= listener;
        public void Raise(IUnitCore source, IUnitCore target) => OnEventTriggered?.Invoke(source, target);
    }
}