using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface Monsters
{
    float GetHealth();
    float GetMaxHealth();

    void TakeDamage(float damage);
}
