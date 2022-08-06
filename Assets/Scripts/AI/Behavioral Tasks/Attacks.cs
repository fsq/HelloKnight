using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Actions.Fsq
{
    public class Attacks : Action
    {
        public SharedFloat ActionDuration;
        public SharedFloat LastAttackTime;
        public SharedFloat Damage;
        public SharedGameObject Target;

        protected Transform _tr;
        protected GameObject _attackObj;

        public override void OnStart()
        {
            LastAttackTime.Value = Time.time;
            _tr = GetComponent<Transform>();
        }

        public override TaskStatus OnUpdate()
        {
            if (LastAttackTime.Value + ActionDuration.Value >= Time.time)
            {
                return TaskStatus.Running;
            }
            else
            {
                return TaskStatus.Success;
            }
        }
    }
}
