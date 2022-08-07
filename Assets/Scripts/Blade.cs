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

        // Blade orientation
        var scale = obj.transform.localScale;
        scale.x *= Mathf.Sign(direction.x); // Flip collider and renderer
        obj.transform.localScale = scale;

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
