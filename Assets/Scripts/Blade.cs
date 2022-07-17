using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour, Attacks
{
    [SerializeField] public float Damage { get; private set; }

    public void SetDamage(float damage)
    {
        Damage = damage;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            var monster = other.gameObject.GetComponent<Monsters>();
            monster.TakeDamage(Damage);
        }
    }
}
