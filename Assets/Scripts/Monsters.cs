using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monsters : MonoBehaviour
{
    abstract public float Health { get; set; }

    abstract public float MaxHealth { get; set; }

    abstract public float Damage { get; set; }

    abstract public void TakeDamage(float damage);

    virtual protected void Die() => Destroy(gameObject);
}