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

    [SerializeField] private float _lastingTime = 0.2f;
    public override float LastingTime { get; }

    static public GameObject Create(GameObject attacker, Vector3 direction,
                                    float damage, float speed)
    {
        var obj = Instantiate(GameManager.Instance.GetPrefab(Constants.kPrefabBullet));
        var bullet = obj.GetComponent<Bullet>();
        bullet.Attacker = attacker;
        bullet.Damage = damage;

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
