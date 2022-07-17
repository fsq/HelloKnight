using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1 : MonoBehaviour, Monsters
{
    [SerializeField] private float _maxHealth = 100;
    public float MaxHealth { get => _maxHealth; }

    [SerializeField] public float _health = 100;
    public float Health { get => _health; private set => _health = value; }

    [SerializeField] public float _damage = 20;
    public float Damage { get => _damage; }

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
