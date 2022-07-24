using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class ActionState : IdleState
{
    protected ActionState(StateParam sp) : base(sp)
    {
        _lastAttackDoneTime = new Dictionary<Constants.AttackType, float>();
    }

    public override State HandleInput(FrameInput input)
    {
        // Always collect attack inputs in Action states.
        // base.HandleInput() should be called in all subclasses.
        if (input.AttackDown())
        {
            _lastAttackDownTime = Time.time;
            _lastAttackDownType = input.BladeDown ?
                                Constants.AttackType.Blade :
                                Constants.AttackType.Bullet;
        }
        return null;
    }

    // Get Cooldown of an attack type from StateParam.
    protected float GetCoolDown(Constants.AttackType type)
    {
        float CD = 1f;
        if (_sp.AttackCoolDown == null)
        {
            Debug.LogError("AttackCoolDown is not set in FSM's param.");
        }
        if (!_sp.AttackCoolDown.TryGetValue(type, out CD))
        {
            Debug.LogError("AttackCoolDown is not set in FSM for attack type: " + nameof(type));
        }
        return CD;
    }

    // Shared across all Action states.
    protected float _lastAttackDownTime = Constants.kNever;
    protected Constants.AttackType _lastAttackDownType = Constants.AttackType.None;
    protected Dictionary<Constants.AttackType, float> _lastAttackDoneTime;
    protected float _lastAttackCoolDown = Constants.kNever;
}

public class AIdleState : ActionState
{
    static public AIdleState Create(StateParam stateParam)
    {
        return new AIdleState(stateParam);
    }

    private Constants.AttackType GetAttackType()
    {
        bool hasBufferedAttack =
                    _lastAttackDownTime + _sp.AttackInputBufferDuration >= Time.time;
        if (!hasBufferedAttack) return Constants.AttackType.None;

        float lastDone = Constants.kNever;
        _lastAttackDoneTime.TryGetValue(_lastAttackDownType, out lastDone);

        float CD = GetCoolDown(_lastAttackDownType);

        // Still in CD.
        if (lastDone + CD >= Time.time)
        {
            return Constants.AttackType.None;
        }

        // Clear buffered attack.
        _lastAttackDownTime = Constants.kNever;

        // TODO: Check resource
        return _lastAttackDownType;
    }

    private State FireAttack(Constants.AttackType type)
    {
        switch (type)
        {
            case Constants.AttackType.Blade:
                Exit();
                return FSM.CreateState(typeof(BladeAttackState), _sp);
            case Constants.AttackType.Bullet:
            default: return null;
        }
    }

    public override State HandleInput(FrameInput input)
    {
        base.HandleInput(input);
        var type = GetAttackType();
        if (type != Constants.AttackType.None)
        {
            return FireAttack(type);
        }
        else
        {
            return null;
        }
    }

    private AIdleState(StateParam stateParam) : base(stateParam) { }

}

class BladeAttackState : ActionState
{
    private GameObject _attackObj;
    private Attacks _attack;
    private float _timer;
    private bool _earlyTerminate;
    private bool _readyToExit;

    static public BladeAttackState Create(StateParam statePram)
    {
        return new BladeAttackState(statePram);
    }

    public override void Enter()
    {
        base.Enter();
        // TODO: Energy change

        Vector3 direction = Vector3.right;
        if (_sp.CurrentFlip) direction *= -1;
        _attackObj = Blade.Create(_obj, null, direction, _sp.BladeDamage);
        _attack = _attackObj.GetComponent<Attacks>();
        _timer = _attack.ActionDuration;
    }

    public override State HandleInput(FrameInput input)
    {
        base.HandleInput(input);

        if (_readyToExit)
        {
            Exit();
            return FSM.CreateState(typeof(AIdleState), _sp);
        }
        else if (_sp.PrevFlip ^ _sp.CurrentFlip)
        {
            // Cancel melee attack immediately when flip.
            _earlyTerminate = true;
        }
        return null;
    }

    public override void Update(FrameInput input, FSMState context)
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0 || _earlyTerminate)
        {
            Attacks.Destruct(_attack);
            _readyToExit = true;
        }
    }

    public override void Exit()
    {
        base.Exit();
        _lastAttackDoneTime[Constants.AttackType.Blade] = Time.time;
    }

    private BladeAttackState(StateParam stateParam) : base(stateParam) { }
}

public class DieState : State
{
    private StateParam.DieDelegate _onDie;

    static public DieState Create(StateParam statePram)
    {
        return new DieState(statePram);
    }

    public override void Enter()
    {
        _onDie.Invoke();
    }

    // Dead stays dead.
    public override State HandleInput(FrameInput input) { return this; }

    public override void Update(FrameInput input, FSMState context) { }

    private DieState(StateParam stateParam) : base(stateParam)
    {
        _onDie = stateParam.OnDie;
    }
}
