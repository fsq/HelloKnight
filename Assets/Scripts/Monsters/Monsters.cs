using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Monsters : MonoBehaviour, IHitable
{
    abstract public float Health { get; set; }

    abstract public float MaxHealth { get; set; }

    abstract public float Damage { get; set; }

    virtual public float UnderAttack(Attacks attack)
    {
        _incomingAttack = attack;
        Health -= attack.Damage;
        return attack.Damage;
    }

    virtual protected void Die() => Destroy(gameObject);

    protected Rigidbody2D _rb;

    // Record if is hit between last and current frame.
    protected Attacks _incomingAttack;
    [SerializeField] protected float _backoffDistance = 1f;
    [SerializeField] protected float _backoffSpeed = 12f;
    protected bool _backoffing;

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    protected void Update()
    {
        GetComponent<SpriteRenderer>().flipX = _rb.velocity.x < 0;
        HandleDamage();
    }

    virtual protected void HandleDamage()
    {
        if (_incomingAttack == null) return;

        _incomingAttack.hitDone();
        if (Health <= 0) Die();

        if (!_backoffing)
        {
            var source = _incomingAttack.transform.position;
            var me = transform.position;
            var direction = Vector3.Normalize(me - source);
            StartCoroutine(Backoff(direction, _backoffDistance, _backoffSpeed));
        }

        _incomingAttack = null;
    }

    virtual protected IEnumerator Backoff(Vector3 direction, float distance, float speed)
    {
        _backoffing = true;

        while (distance > 0)
        {
            var currentDist = speed * Time.deltaTime;
            transform.position += direction * currentDist;
            distance -= currentDist;
            yield return null;
        }
        // _rb.velocity = Vector2.zero;
        _backoffing = false;
    }

}