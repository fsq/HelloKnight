using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Attacks
{
    [SerializeField] private float _damage;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private GameObject _attacker;
    public override GameObject Attacker { get => _attacker; set => _attacker = value; }

    [SerializeField] private float _lifeSpan = 3;
    public override float LifeSpan => _lifeSpan;

    [SerializeField] private float _lastingTime = 0.2f;
    public override float LastingTime => _lastingTime;

    [SerializeField] private float _recoverEnergy = 5f;

    static public GameObject Create(GameObject attacker, AttackerDelegate del,
                                    Vector3 direction, float damage, float speed)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabBullet));
        var bullet = obj.GetComponent<Bullet>();
        bullet.Attacker = attacker;
        bullet.Damage = damage;
        bullet.Delegate = del;

        // Bullet start pos
        var pos = obj.transform.position;
        pos.x *= Mathf.Sign(direction.x);
        obj.transform.position = attacker.transform.position + pos;

        // Bullet velocity
        var v = obj.GetComponent<Rigidbody2D>().velocity;
        v.x = speed * Mathf.Sign(direction.x);
        obj.GetComponent<Rigidbody2D>().velocity = v;

        return obj;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(Constants.kTagMonsters))
        {
            Hit(other.gameObject);
        }
        else if (other.CompareTag(Constants.kTagGround))
        {
            Destruct();
        }
    }

    public override void hitDone()
    {
        // no-op, penetrate.
    }
}
