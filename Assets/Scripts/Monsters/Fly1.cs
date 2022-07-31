using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fly1 : Monsters
{
    [SerializeField] private float _maxHealth = 50;
    public override float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] private float _health = 50;
    public override float Health { get => _health; set => _health = value; }

    [SerializeField] private float _damage = 15;
    public override float Damage { get => _damage; set => _damage = value; }
}
