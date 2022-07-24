using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1 : Monsters
{
    [SerializeField] private float _maxHealth = 80;
    public override float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] private float _health = 80;
    public override float Health { get => _health; set => _health = value; }

    [SerializeField] private float _damage = 10;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _trackingDistance = 0f;
    [SerializeField] private float _backoffDistance = 1f;
    [SerializeField] private float _backoffSpeed = 12f;

    // Target for attacking, moving, etc.
    [SerializeField] private GameObject _target;

    private bool _wasHit;
    private bool _backoffing;
    private Attacks _incomingAttack;
    private Rigidbody2D _rb;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag(Constants.kTagPlayer);
            if (_target == null) return;
        }

        GetComponent<SpriteRenderer>().flipX = transform.position.x > _target.transform.position.x;

        if (_wasHit)
        {
            // Increase order in layer?
            Health -= _incomingAttack.Damage;
            if (Health <= 0) Die();
            _incomingAttack.hitDone();

            if (!_backoffing)
            {
                var source = _incomingAttack.transform.position;
                var me = transform.position;
                var direction = Vector3.Normalize(me - source);
                StartCoroutine(Backoff(direction, _backoffDistance, _backoffSpeed));
            }
            _wasHit = false;
        }
        // Moving towards target.
        if (Vector2.Distance(transform.position, _target.transform.position) > _trackingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position,
                                    _target.transform.position, _moveSpeed * Time.deltaTime);
        }
        else
        {
            // Attack
        }
    }

    public override void UnderAttack(Attacks attack)
    {
        _incomingAttack = attack;
        _wasHit = true;
    }

    IEnumerator Backoff(Vector3 direction, float distance, float speed)
    {
        _backoffing = true;

        while (distance > 0)
        {
            var currentDist = speed * Time.deltaTime;
            transform.position += direction * currentDist;
            distance -= currentDist;
            yield return null;
        }
        _rb.velocity = Vector2.zero;
        _backoffing = false;
    }
}
