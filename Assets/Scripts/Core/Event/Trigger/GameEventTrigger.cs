using UnityEngine;
using System;
using Encounter.NightDance.Status;
using R3;
namespace Encounter.NightDance.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Scriptable Objects/GameEventTrigger")]
    public class GameEventTrigger : ScriptableObject
    {
        private readonly ReactiveProperty<(IUnitCore, IUnitCore)> _onEventTriggeredSubject = new();
        public Observable<(IUnitCore, IUnitCore)> OnEventTriggeredAsObservable() => _onEventTriggeredSubject;
        public void Raise(IUnitCore source, IUnitCore target) => _onEventTriggeredSubject.OnNext((source, target));
    }
}