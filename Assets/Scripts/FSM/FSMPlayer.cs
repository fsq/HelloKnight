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

    [SerializeField] private GameObject PlayerFeet;
    private bool IsInAir()
    {
        return PlayerFeet.GetComponent<PlayerFeet>().IsInAir();
    }

    private Rigidbody2D _rb;
    private FSM _fsm;
    private FrameInput _frameInput;

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

    private bool DieCheck(StateParam sp)
    {
        //  TODO: parameterize
        return sp.Obj.transform.position.y < -30 || sp.Health <= 0;
    }

    private void OnDie()
    {
        SceneManager.LoadScene(Constants.kSceneDefault);
    }
}
