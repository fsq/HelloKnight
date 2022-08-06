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
    [SerializeField] private float _triggerDistance = 20f; // Start tracking when target in range.

    void Start()
    {
        if (_target == null)
        {
            _target = GameObject.FindGameObjectWithTag(Constants.kTagPlayer);
        }
    }

    public override void Update()
    {
        base.Update();

        if (_target == null) return;
        // Moving towards target.
        var dist = Vector2.Distance(transform.position, _target.transform.position);
        if (dist > _triggerDistance)
        {
            // Idle
        }
        else if (dist > _trackingDistance)
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
