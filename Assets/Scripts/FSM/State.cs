using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StateParam
{
    public GameObject Obj;

    // Check termination of FSM
    public delegate bool DieCheckDelegate();
    public delegate void DieDelegate();

    public DieCheckDelegate DieCheck;
    public DieDelegate OnDie;

    #region Resourecs
    [SerializeField] public float Maxhealth = 100;
    [SerializeField] public float Health = 100;
    [SerializeField] public float MaxEnergy = 30;
    [SerializeField] public float Energy = 0;
    #endregion

    #region Movement
    [SerializeField] public float HorizontalSpeed = 9f;
    [SerializeField] public bool PrevFlip = false;      // Flipped in previous frame?
    [SerializeField] public bool CurrentFlip = false;   // Flipped in this frame?
    #endregion
}

abstract public class State
{
    // Return next state if applicable.
    // This function CAN modify fields in frame input.
    abstract public State HandleInput(FrameInput input);
    abstract public void Update(FrameInput input, FSMState context);

    // Invoke when enter/exit this state.
    virtual public void Enter()
    {
        Debug.Log("FSM entered state: " + this.GetType().Name);
    }
    virtual public void Exit() { }

    // Client of the state, and corresponding GameObject.
    protected GameObject _obj;
    protected IStateful _client;
    protected StateParam _sp;
    protected State(StateParam stateParam)
    {
        _sp = stateParam;
        _obj = stateParam.Obj;
        _client = _obj.GetComponent<IStateful>();
    }
}

// Horizontal move
public class HMoveState : State
{
    static public HMoveState Create(StateParam stateParam)
    {
        return new HMoveState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        _sp.PrevFlip = _sp.CurrentFlip;
        if (input.X == 0)
            return FSM.CreateState(typeof(HIdleState), _sp);
        else
            return null;
    }

    public override void Update(FrameInput input, FSMState context)
    {
        _obj.transform.position += Mathf.Sign(input.X) * Vector3.right *
                    Time.deltaTime * _sp.HorizontalSpeed;
        _sp.CurrentFlip = input.X < 0;
        if (_sp.CurrentFlip ^ _sp.PrevFlip)
        {
            _obj.GetComponent<SpriteRenderer>().flipX = _sp.CurrentFlip;
        }
    }

    private HMoveState(StateParam stateParam) : base(stateParam) { }
}

public class JumpingState : State
{
    static public JumpingState Create(StateParam stateParam)
    {
        return new JumpingState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        // TODO
        return null;
    }

    public override void Update(FrameInput input, FSMState context) { }

    private JumpingState(StateParam stateParam) : base(stateParam) { }
}

abstract public class IdleState : State
{
    public override void Update(FrameInput input, FSMState context) { }

    protected IdleState(StateParam stateParam) : base(stateParam) { }
}

// Idle state on horizontal axis
public class HIdleState : IdleState
{
    static public HIdleState Create(StateParam stateParam)
    {
        return new HIdleState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        if (input.X != 0)
        {
            return FSM.CreateState(typeof(HMoveState), _sp);
        }
        else
        {
            return null;
        }
    }
    private HIdleState(StateParam stateParam) : base(stateParam) { }
}

// Idle state on vertical axis
public class VIdleState : IdleState
{
    static public VIdleState Create(StateParam stateParam)
    {
        return new VIdleState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        // TODO
        return null;
    }
    private VIdleState(StateParam stateParam) : base(stateParam) { }
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