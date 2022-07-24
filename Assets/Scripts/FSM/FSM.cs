using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class FSMState
{
    public State H;  // Horizontal movement
    public State V;  // Vertical movement
    public State A;  // Actions, attack, etc.
}

public class FSM
{
    public StateParam StateParam { get; set; }

    // State.HandleInput can potentially change frame input fields.
    // The order MATTERS.
    private void HandleInput(FrameInput input)
    {
        var next_h = _state.H.HandleInput(input);
        if (next_h != null)
        {
            _state.H = next_h;
            next_h.Enter();
        }
        var next_v = _state.V.HandleInput(input);
        if (next_v != null)
        {
            _state.V = next_v;
            next_v.Enter();
        }
        // Special case. An object can Die during any states.
        if (StateParam.DieCheck(StateParam) && _state.A.GetType() != typeof(DieState))
        {
            _state.A = FSM.CreateState(typeof(DieState), StateParam);
            _state.A.Enter();
        }
        else
        {
            var next_a = _state.A.HandleInput(input);
            if (next_a != null)
            {
                _state.A = next_a;
                next_a.Enter();
            }
        }
    }

    public void Update(FrameInput input)
    {
        HandleInput(input);
        _state.H.Update(input, _state);
        _state.V.Update(input, _state);
        _state.A.Update(input, _state);
    }

    public void FixedUpdate(FrameInput input)
    {
        _state.H.FixedUpdate(input, _state);
        _state.V.FixedUpdate(input, _state);
        _state.A.FixedUpdate(input, _state);
    }

    static public State CreateState(Type stateType, StateParam stateParam)
    {
        var creator = stateType.GetMethod("Create", BindingFlags.Static |
                                                    BindingFlags.Public);
        return (State)creator.Invoke(null, new object[] { stateParam });
    }

    static public FSM Create(StateParam stateParam)
    {
        var fsm = new FSM(stateParam);

        fsm._state.H = FSM.CreateState(typeof(HIdleState), stateParam);
        fsm._state.V = FSM.CreateState(typeof(VIdleState), stateParam);
        fsm._state.A = FSM.CreateState(typeof(AIdleState), stateParam);

        return fsm;
    }

    private FSM(StateParam stateParam)
    {
        StateParam = stateParam;
    }

    private FSMState _state = new FSMState();
}
