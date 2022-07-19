using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Attacks
{
    [SerializeField] private float _damage;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private GameObject _owner;
    public override GameObject Attacker { get => _owner; set => _owner = value; }

    [SerializeField] private float _lifeSpan = 3;

    private void Start()
    {
        StartCoroutine(DestroyTimer(_lifeSpan));
    }

    public override void Hit(GameObject victim)
    {
        var monster = victim.gameObject.GetComponent<Monsters>();
        monster.UnderAttack(this);
        gameObject.GetComponent<Collider2D>().enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            Hit(other.gameObject);
        }
        else if (other.CompareTag(Constants.kTagGround))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator DestroyTimer(float lifeSpan)
    {
        yield return new WaitForSeconds(lifeSpan);
        Destroy(gameObject);
    }
}
