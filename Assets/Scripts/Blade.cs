using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Attacks
{
    // Can't Hit when false.
    // OnTriggerEnter2D still triggers for all entered objects even if we 
    // disable collider after the first Hit().
    private bool _isActive = true;

    static public ResourceGauge GetCostByAttackerTag(string tag)
    {
        return new ResourceGauge();
    }

    // <direction> has form (X, Y, <ignore>). 
    // -/+ X flips blade to left/right.
    // -/+ Y rotates blade to down/up.
    static public GameObject Create(GameObject attacker, AttackerDelegate del,
                                    Vector3 direction, float damage)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabBlade),
                                attacker.transform);
        if (obj == null)
        {
            Debug.LogError("No prefab found: " + Constants.kPrefabBlade);
            return null;
        }
        var blade = obj.GetComponent<Blade>();
        if (blade == null)
        {
            Debug.LogError("No Blade component found in prefab: " + Constants.kPrefabBlade);
            return null;
        }
        blade.Attacker = attacker;
        blade.Damage = damage;
        blade.Delegate = del;

        // Use Euler(or vector) representation instead of Quaternion, since Quaternions
        // can only be multiplied but not added. We want both two rotations apply to the original
        // transform, instead of first apply rotation1, then apply rotation2 on modified axis.
        Vector3 rotation = Vector3.zero;
        // Face up/down
        if (direction.y != 0)
        {
            // Align the forward(Z) direction to Z axis, and align the Y direction to left/right,
            // So that the X axis of blade sprite is facing up/down:
            // https://forum.unity.com/threads/look-rotation-2d-equivalent.611044/#post-4092259
            rotation = Quaternion.LookRotation(
                                    Vector3.forward,
                                    direction.y > 0 ? Vector3.left : Vector3.right
                                ).eulerAngles;
        }
        // Rotate on Y axis to face left/right
        rotation += Quaternion.LookRotation(
                        direction.x > 0 ? Vector3.forward : Vector3.back,
                        Vector3.up).eulerAngles;
        obj.transform.rotation = Quaternion.Euler(rotation);
        return obj;
    }

    public override void Hit(GameObject victim)
    {
        base.Hit(victim);
        _isActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!_isActive) return;
        if (other.CompareTag(Constants.kTagMonsters))
        {
            Hit(other.gameObject);
        }
    }

    public override void hitDone()
    {
        // no-op
    }
}
