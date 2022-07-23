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

    private Rigidbody2D _rb;
    private FSM _fsm;
    private FrameInput _frameInput;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        if (this.gameObject == null) Debug.Log("WF");

        sp.Obj = this.gameObject;
        sp.OnDie = this.OnDie;

        var register_states = new List<Type> {
                                typeof(HIdleState),
                                typeof(VIdleState),
                                typeof(DieState)};
        _fsm = FSM.Create(sp, register_states);
    }

    private void Update()
    {
        GatherInput();
        _fsm.Update(_frameInput);
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

    private void OnDie()
    {
        SceneManager.LoadScene(Constants.kSceneDefault);
    }
}
