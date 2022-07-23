using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    private float _jumpHoldDuration;
    private float _lastJumpPressed = Constants.kNever;
    private float _enterTime;
    private float _prepareDuration = 0.1f;

    private Rigidbody2D _rb;

    private const float _maxJumpHoldDuration = 0.35f;
    private const float _minJumpHoldDuration = 0.2f;
    private const float _jumpBuffer = 0.1f;
    private const float _maxFallSpeed = 18f;

    static public JumpingState Create(StateParam stateParam)
    {
        return new JumpingState(stateParam);
    }

    public override void Enter()
    {
        _sp.HasBufferedJump = false;
        if (_rb.velocity.y != 0)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, 0);
        }
        _rb.gravityScale = _sp.DefaultGScale;
        _rb.AddForce(Vector2.up * _sp.JumpForce, ForceMode2D.Impulse);

        _enterTime = Time.time;
        base.Enter();
    }


    public override State HandleInput(FrameInput input)
    {
        // Landed. 
        if (!_sp.IsInAir())
        {
            // Prevent State to immediates exits, give it some time to leave ground.
            if (Time.time - _enterTime < _prepareDuration)
            {
                return null;
            }
            Exit();
            var land = FSM.CreateState(typeof(VIdleState), _sp);
            return land;
        }
        else
        {
            if (input.JumpDown)
            {
                _lastJumpPressed = Time.time;
            }
        }
        return null;
    }

    public override void Update(FrameInput input, FSMState context)
    {
        if (input.JumpHold)
        {
            if (_jumpHoldDuration < _maxJumpHoldDuration)
            {
                _jumpHoldDuration = Mathf.Clamp(_jumpHoldDuration + Time.deltaTime,
                                                0, _maxJumpHoldDuration);
                _rb.AddForce(Vector2.up * _sp.JumpForce * Time.deltaTime, ForceMode2D.Force);
            }
        }
        else if (input.JumpUp)
        {
            if (_jumpHoldDuration < _maxJumpHoldDuration)
            {
                _rb.velocity = new Vector2(_rb.velocity.x,
                    CalculateInitialFallSpeed(_jumpHoldDuration, _rb.velocity.y));
                _jumpHoldDuration = _maxJumpHoldDuration;
            }
        }
    }

    public override void FixedUpdate(FrameInput input, FSMState context)
    {
        // Clamp maximum falling speed
        if (_rb.velocity.y < 0)
        {
            if (_rb.velocity.y < -_maxFallSpeed)
                _rb.velocity = new Vector2(_rb.velocity.x, -_maxFallSpeed);
            _rb.gravityScale = _sp.FallingGScale;
        }
    }

    public override void Exit()
    {
        _sp.HasBufferedJump = _lastJumpPressed + _jumpBuffer >= Time.time;
        _rb.gravityScale = _sp.DefaultGScale;
    }

    private JumpingState(StateParam stateParam) : base(stateParam)
    {
        if (stateParam.IsInAir == null)
        {
            Debug.LogError("StateParam must set IsInAir to create " + this.GetType().Name);
        }
        _rb = _obj.GetComponent<Rigidbody2D>();
    }

    private float CalculateInitialFallSpeed(float holdTime, float initSpeed)
    {
        // If hold full 0.35 seconds, no speed decay after release
        // If no hold, speed should decay to 0
        // something like:
        // max(hold, 0.2) --> 0
        // min(hold, 0.35) --> initSpeed
        // y = initSpeed/(0.35-0.2)* x - 0.2/(0.35-0.2)*initSpeed
        holdTime = Mathf.Max(holdTime, _minJumpHoldDuration);
        float a = initSpeed / (_maxJumpHoldDuration - _minJumpHoldDuration);
        float b = -_minJumpHoldDuration * initSpeed / (_maxJumpHoldDuration - _minJumpHoldDuration);
        return Mathf.Max(0, a * holdTime + b);
    }
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
    // TODO: coyote 
    private float _lastGrounded = Constants.kNever;
    private float _coyoteThreshold = 0.1f;

    static public VIdleState Create(StateParam stateParam)
    {
        return new VIdleState(stateParam);
    }

    public override State HandleInput(FrameInput input)
    {
        bool inAir = _sp.IsInAir();

        if (input.JumpDown || _sp.HasBufferedJump)
        {
            // Coyote
            // Should trigger ONLY when FALL off platform.
            // TODO: Only check time gap may not be enough, imagine player dash
            // far from platform and press jump.
            if (!inAir || Time.time - _lastGrounded <= _coyoteThreshold)
            {
                return FSM.CreateState(typeof(JumpingState), _sp);
            }
        }
        if (inAir)
        {
            return FSM.CreateState(typeof(FallingState), _sp);
        }
        return null;
    }

    public override void Update(FrameInput input, FSMState context)
    {
        if (!_sp.IsInAir())
        {
            _lastGrounded = Time.time;
        }
    }


    private VIdleState(StateParam stateParam) : base(stateParam) { }
}

public class FallingState : State
{
    public override State HandleInput(FrameInput input)
    {
        if (!_sp.IsInAir())
        {
            return FSM.CreateState(typeof(VIdleState), _sp);
        }
        else
        {
            return null;
        }
    }

    public override void Update(FrameInput input, FSMState context) { }

    static public FallingState Create(StateParam stateParam)
    {
        return new FallingState(stateParam);
    }

    private FallingState(StateParam stateParam) : base(stateParam) { }
}
