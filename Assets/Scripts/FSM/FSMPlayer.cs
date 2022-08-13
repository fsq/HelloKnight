/*
*  Player script by Finite State Machine.
*/

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSMPlayer : MonoBehaviour, IHitable
{
    [SerializeField] public StateParam sp;
    [SerializeField] public int Coin;
    [SerializeField] private float _bladeCoolDown = 0.3f;
    [SerializeField] private float _bladeDamage = 10;

    [SerializeField] private float _bulletCoolDown = 0.3f;
    [SerializeField] private float _bulletDamage = 15;
    [SerializeField] private float _hitRecoverTime = 0.5f; // Invulnerable time after hit.

    [SerializeField] public ResourceGauge resource = new ResourceGauge(100, 30);

    [SerializeField] private GameObject PlayerFeet;
    private bool IsInAir()
    {
        return PlayerFeet.GetComponent<PlayerFeet>().IsInAir();
    }

    private Rigidbody2D _rb;
    private FSM _fsm;
    private FrameInput _frameInput;
    private float _lastHit = Constants.kNever;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        Dictionary<Constants.AttackType, float> CDs =
            new Dictionary<Constants.AttackType, float>();
        // TODO: Add default cooldown to Attacks baseclass(as a struct maybe?), 
        // and replace with them.
        CDs.Add(Constants.AttackType.Blade, _bladeCoolDown);
        CDs.Add(Constants.AttackType.Bullet, _bulletCoolDown);

        sp.Obj = this.gameObject;
        sp.Player = this;
        sp.DieCheck = this.DieCheck;
        sp.OnDie = this.OnDie;
        sp.IsInAir = this.IsInAir;
        sp.AttackCoolDown = CDs;
        sp.BladeDamage = _bladeDamage;
        sp.BulletDamage = _bulletDamage;

        _fsm = FSM.Create(sp);
    }

    private void Update()
    {
        GatherInput();
        _fsm.Update(_frameInput);
    }

    private void FixedUpdate()
    {
        _fsm.FixedUpdate(_frameInput);
    }

    private void GatherInput()
    {
        _frameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown(Constants.kJump),
            JumpUp = Input.GetButtonUp(Constants.kJump),
            JumpHold = Input.GetButton(Constants.kJump),
            X = Input.GetAxisRaw(Constants.kHorizontal),
            Y = Input.GetAxisRaw(Constants.kVertical),
            BladeDown = Input.GetButtonDown(Constants.kBladeAttack),
            BulletDown = Input.GetButtonDown(Constants.kBulletAttack)
        };
    }

    // Normally damage should be handle by Attacks class, however
    // collision damage is a special case (for now).
    private void OnCollisionStay2D(Collision2D other)
    {
        var obj = other.gameObject;
        if (obj.CompareTag(Constants.kTagMonsters))
        {
            // Still recovering from last hit.
            if (_lastHit + _hitRecoverTime > Time.time)
            {
                return;
            }
            else
            {
                _lastHit = Time.time;
            }
            var monster = obj.GetComponent<Monsters>();
            if (monster == null) return;
            UnderAttack(monster.Damage);
        }
    }

    public float UnderAttack(Attacks attack)
    {
        // Still recovering from last hit.
        if (_lastHit + _hitRecoverTime > Time.time)
        {
            return 0;
        }
        else
        {
            _lastHit = Time.time;
            return UnderAttack(attack.Damage);
        }
    }

    private float UnderAttack(float damage)
    {
        resource.Health -= damage;
        return damage;
        // Shock time
    }

    private bool DieCheck(StateParam sp)
    {
        //  TODO: parameterize
        return sp.Obj.transform.position.y < -30 || resource.Health <= 0;
    }

    private void OnDie()
    {
        SceneManager.LoadScene(Constants.kSceneDefault);
    }

    public void Heal(ResourceGauge amount)
    {
        if (amount.Health > 0)
        {
            TextDisplay.DisplayHealthRecovery(transform, amount.Health);
        }
        resource.Change(amount);
    }

    // TODO: Maybe use this same method for both earning/spending coins?
    public void PickupCoin(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Coin amount is not positive: " + amount);
        }
        Coin += amount;
    }
}
