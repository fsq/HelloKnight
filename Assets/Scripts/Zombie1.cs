using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie1 : MonoBehaviour, Monsters
{
    [SerializeField] private float _maxHealth = 100;
    public float MaxHealth { get => _maxHealth; }

    [SerializeField] private float _health = 100;
    public float Health { get => _health; private set => _health = value; }

    [SerializeField] private float _damage = 20;
    public float Damage { get => _damage; }

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _trackingDistance = 1.2f;

    // Target for attacking, moving, etc.
    [SerializeField] private GameObject _target;

    private bool _wasHit;

    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0)
        {
            Die();
        }
        _wasHit = true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    void Start() { }

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
}
