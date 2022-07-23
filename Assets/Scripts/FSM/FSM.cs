using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System;

public class FSMState
{
    public State H;  // Horizontal movement
    public State V;  // Vertical movement
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
    }

    public void Update(FrameInput input)
    {
        HandleInput(input);
        _state.H.Update(input, _state);
        _state.V.Update(input, _state);
    }

    static public State CreateState(Type stateType, StateParam stateParam)
    {
        var creator = stateType.GetMethod("Create", BindingFlags.Static |
                                                    BindingFlags.Public);
        return (State)creator.Invoke(null, new object[] { stateParam });
    }

    static public FSM Create(StateParam stateParam, List<Type> types)
    {
        var fsm = new FSM(stateParam);
        foreach (var type in types)
        {
            fsm.AppendState(type);
            Debug.Log("FSM registered state: " + type.Name);
        }

        fsm._state.H = fsm._registry[nameof(HIdleState)];
        fsm._state.V = fsm._registry[nameof(VIdleState)];

        return fsm;
    }

    public State AppendState(Type stateType)
    {
        var state = CreateState(stateType, StateParam);
        _registry.Add(stateType.Name, state);
        return state;
    }

    private FSM(StateParam stateParam)
    {
        StateParam = stateParam;
    }

    Dictionary<string, State> _registry = new Dictionary<string, State>();
    private FSMState _state = new FSMState();
}
