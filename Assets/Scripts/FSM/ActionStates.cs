using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIdleState : IdleState
{
    static public AIdleState Create(StateParam stateParam)
    {
        return new AIdleState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        return null;
    }

    private AIdleState(StateParam stateParam) : base(stateParam) { }

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
