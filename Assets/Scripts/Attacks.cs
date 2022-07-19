using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Attacks : MonoBehaviour
{
    abstract public GameObject Attacker { get; set; }

    abstract public float Damage { get; set; }

    // Maybe do something to the attacker. Healing, Charging, etc.
    // TODO: Refactor GameObject -> interface, hitable?
    abstract public void Hit(GameObject victim);

    virtual public void Init(GameObject attacker, float damage)
    {
        Attacker = attacker;
        Damage = damage;
    }

    virtual public void hitDone()
    {
        Destroy(gameObject);
    }
}