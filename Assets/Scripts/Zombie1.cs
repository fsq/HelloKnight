using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1 : MonoBehaviour, Monsters
{
    [SerializeField] public float MaxHealth = 100;
    [SerializeField] public float Health = 100;


    public float GetHealth() => Health;
    public float GetMaxHealth() => MaxHealth;

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
