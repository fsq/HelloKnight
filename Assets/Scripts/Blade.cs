using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour, Attacks
{
    [SerializeField] private float _damage;
    public float Damage { get => _damage; set => _damage = value; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            var monster = other.gameObject.GetComponent<Monsters>();
            monster.TakeDamage(Damage);
        }
    }
}
