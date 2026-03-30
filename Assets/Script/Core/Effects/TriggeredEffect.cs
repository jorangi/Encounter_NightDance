using System;
using System.Collections.Generic;
using Encounter.NightDance.ScriptableObjects;
using Encounter.NightDance.Core.Event.Conditions;
using UnityEngine;
using Encounter.NightDance.Status;
using Encounter.NightDance.Core.Event;

namespace Encounter.NightDance.Core.Effects
{
    [Serializable]
    public class TriggeredEffect
    {
        public GameEventTrigger triggerAsset;
        [SerializeReference]public List<ICondition> conditions = new();
        [SerializeReference]public List<IEffect> effects = new();
        /// <summary>
        /// 조건이 모두 만족할 때 효과 실행, 트리거는 GameEventTrigger의 이벤트에 의해 호출됨
        /// </summary>
        /// <param name="source"></param>
        /// <param name="target"></param>
        public void TryTrigger(EventContext context)
        {
            foreach(ICondition condition in conditions)
            {
                if (!condition.Evaluate(context)) return;
            }
            foreach(IEffect effect in effects)
            {
                effect.Execute(context);
            }
        }
    }
}