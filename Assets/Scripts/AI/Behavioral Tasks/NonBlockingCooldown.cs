using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BehaviorDesigner.Runtime.Tasks.Fsq
{
    [TaskDescription("Return Failure during Cooldown.")]
    [TaskIcon("{SkinColor}CooldownIcon.png")]
    public class NonBlockingCooldown : Conditional
    {
        public SharedFloat CD = 2;
        public SharedFloat LastAttackTime = -1;

        private bool InCD()
        {
            return LastAttackTime.Value != -1 &&
                    LastAttackTime.Value + CD.Value > Time.time;
        }

        public override TaskStatus OnUpdate()
        {
            if (InCD())
            {
                return TaskStatus.Failure;
            }
            else
            {
                return TaskStatus.Success;
            }
        }

    }
}
