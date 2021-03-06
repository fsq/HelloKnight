/*
*  Player script by Finite State Machine.
*/

using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FSMPlayer : MonoBehaviour
{
    [SerializeField] public StateParam sp;
    [SerializeField] private float _bladeCoolDown = 0.3f;
    [SerializeField] private float _bladeDamage = 10;

    [SerializeField] private float _bulletCoolDown = 0.3f;
    [SerializeField] private float _bulletDamage = 15;
    [SerializeField] private float _hitRecoverTime = 0.5f; // Invulnerable time after hit.

    [SerializeField] private float _maxHealth = 100;
    [SerializeField] private float _maxEnergy = 30;

    public float MaxHealth { get => _maxHealth; }
    public float Health { get => sp.resource.Health; }

    public float MaxEnergy { get => _maxEnergy; }
    public float Energy { get => sp.resource.Energy; }

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
        sp.DieCheck = this.DieCheck;
        sp.OnDie = this.OnDie;
        sp.IsInAir = this.IsInAir;
        sp.AttackCoolDown = CDs;
        sp.BladeDamage = _bladeDamage;
        sp.BulletDamage = _bulletDamage;
        sp.resource.MaxEnergy = _maxEnergy;
        sp.resource.MaxHealth = sp.resource.Health = _maxHealth;

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

    private void UnderAttack(float damage)
    {
        sp.resource.Health -= damage;
        // Shock time
    }

    private bool DieCheck(StateParam sp)
    {
        //  TODO: parameterize
        return sp.Obj.transform.position.y < -30 || sp.resource.Health <= 0;
    }

    private void OnDie()
    {
        SceneManager.LoadScene(Constants.kSceneDefault);
    }
}
