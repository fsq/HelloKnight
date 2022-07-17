using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Monsters
{
    float Health { get; }
    float MaxHealth { get; }
    float Damage { get; }

    void TakeDamage(float damage);
}
