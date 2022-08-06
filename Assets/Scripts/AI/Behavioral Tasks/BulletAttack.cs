using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.Actions.Fsq
{
    public class BulletAttack : Attacks
    {

        public override void OnStart()
        {
            base.OnStart();

            var direction = Target.Value.transform.position - _tr.position;
            _attackObj = Bullet.CreateForMonster(gameObject, null, direction, Damage.Value);
            ActionDuration.Value = _attackObj.GetComponent<global::Attacks>().ActionDuration;
        }

        public override TaskStatus OnUpdate()
        {
            return base.OnUpdate();
        }
    }
}