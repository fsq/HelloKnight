using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monsters : MonoBehaviour
{
    abstract public float Health { get; set; }

    abstract public float MaxHealth { get; set; }

    abstract public float Damage { get; set; }

    // Return the actual damage dealt.
    abstract public float UnderAttack(Attacks attack);

    virtual protected void Die() => Destroy(gameObject);
}