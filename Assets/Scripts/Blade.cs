using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : Attacks
{
    [SerializeField] private float _damage;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private GameObject _owner;
    public override GameObject Attacker { get => _owner; set => _owner = value; }

    public override void Hit(GameObject victim)
    {
        var monster = victim.gameObject.GetComponent<Monsters>();
        monster.UnderAttack(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            Hit(other.gameObject);
        }
    }

    public override void hitDone()
    {
        // no-op
    }
}
