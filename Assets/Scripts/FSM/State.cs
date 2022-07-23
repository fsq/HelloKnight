using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StateParam
{
    [NonSerialized] public GameObject Obj;

    // Check termination of FSM
    public delegate bool DieCheckDelegate(StateParam sp);
    public delegate void DieDelegate();

    public DieCheckDelegate DieCheck;
    public DieDelegate OnDie;

    #region Resourecs
    [SerializeField] public float MaxHealth = 100;
    [SerializeField] public float Health = 100;
    [SerializeField] public float MaxEnergy = 30;
    [SerializeField] public float Energy = 0;
    #endregion

    #region Movement
    [SerializeField] public float HorizontalSpeed = 9f;
    [SerializeField] public float JumpForce = 20f;
    [SerializeField] public float DefaultGScale = 4.5f;
    [SerializeField] public float FallingGScale = 5.5f;
    [NonSerialized] public bool HasBufferedJump;
    [SerializeField] public bool PrevFlip = false;      // Flipped in previous frame?
    [SerializeField] public bool CurrentFlip = false;   // Flipped in this frame?
    public delegate bool IsInAirDelegate();
    public IsInAirDelegate IsInAir;
    #endregion
}

abstract public class State
{
    // Return next state if applicable.
    // This function CAN modify fields in frame input.
    abstract public State HandleInput(FrameInput input);
    abstract public void Update(FrameInput input, FSMState context);
    virtual public void FixedUpdate(FrameInput input, FSMState context) { }

    // Invoke when enter/exit this state.
    virtual public void Enter()
    {
        // Debug.Log("FSM entered state: " + this.GetType().Name);
    }
    virtual public void Exit() { }

    // Client of the state, and corresponding GameObject.
    protected GameObject _obj;
    protected IStateful _client;
    protected StateParam _sp;
    protected State(StateParam sp)
    {
        _sp = sp;
        _obj = sp.Obj;
        _client = _obj.GetComponent<IStateful>();
    }
}

abstract public class IdleState : State
{
    public override void Update(FrameInput input, FSMState context) { }

    protected IdleState(StateParam sp) : base(sp) { }
}