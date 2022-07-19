using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1 : Monsters
{
    [SerializeField] private float _maxHealth = 100;
    public override float MaxHealth { get => _maxHealth; set => _maxHealth = value; }

    [SerializeField] private float _health = 100;
    public override float Health { get => _health; set => _health = value; }

    [SerializeField] private float _damage = 20;
    public override float Damage { get => _damage; set => _damage = value; }

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _trackingDistance = 1.2f;
    [SerializeField] private float _backoffDistance = 0.5f;
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

        if (_wasHit)
        {
            // Increase order in layer?
            Health -= _incomingAttack.Damage;
            if (Health <= 0) Die();

            if (!_backoffing)
            {
                var source = _incomingAttack.transform.position;
                var me = transform.position;
                var direction = Vector3.Normalize(me - source);
                StartCoroutine(Backoff(direction, _backoffDistance, _backoffSpeed));
                // Push backwards
                _rb.AddForce(direction * 5, ForceMode2D.Impulse);
            }

            _wasHit = false;
            _incomingAttack.hitDone();
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
