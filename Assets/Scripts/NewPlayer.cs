using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewPlayer : MonoBehaviour
{
    #region Serialize Fields

    [Header("Movement")]
    [SerializeField] private float _horizontalSpeed = 9f;
    [SerializeField] private float _jumpBuffer = 0.1f;
    [SerializeField] private float _jumpForce = 20f;
    [SerializeField] private float _coyoteThreshold = 0.1f;
    [SerializeField] private float _defaultGravityScale = 4.5f;
    [SerializeField] private float _fallingGravityScale = 5.5f;
    [SerializeField] private float _maxJumpHoldTime = 0.35f;
    [SerializeField] private float _minJumpHoldTime = 0.2f;
    [SerializeField] private float _maxFallSpeed = 18f;
    [SerializeField] private GameObject PlayerFeet;
    [Space(10)]

    [Header("Attack")]
    [SerializeField] private float _attackBuffer = 0.15f;
    [SerializeField] private float _attackCoolDown = 0.3f;
    [SerializeField] private float _bulletSpeed = 20f;
    [SerializeField] private float _bladeDamage = 10;
    public float BladeDamage { get => _bladeDamage; set => _bladeDamage = value; }
    [SerializeField] private float _bladeEnergyBoost = 5;
    [SerializeField] private float _bulletDamage = 15;
    public float BulletDamage { get => _bulletDamage; set => _bulletDamage = value; }
    [SerializeField] private float _bulletEnergyCost = 15;
    [SerializeField] private float _hitRecoverTime = 0.5f;  // Invulnerable time after hit.
    [Space(10)]

    [SerializeField] private float _maxHealth = 100;
    public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    [SerializeField] private float _maxEnergy = 30;
    public float MaxEnergy { get => _maxEnergy; set => _maxEnergy = value; }

    [SerializeField] private float _health;
    public float Health { get => _health; private set => _health = value; }
    [SerializeField] private float _energy;
    public float Energy { get => _energy; private set => _energy = value; }
    #endregion

    public FrameInput FrameInput { get; private set; }

    private Rigidbody2D _rb;

    private bool _currentFlip;
    private bool _prevFlip;

    #region Attack
    private bool _isAttacking;
    private float _lastAttackDown = Constants.kNever;
    private Constants.AttackType _lastAttackType = Constants.AttackType.None;
    private float _lastAttackFinish = Constants.kNever;
    private float _lastHit = Constants.kNever;
    private bool _earlyTerminateAttack;
    #endregion

    #region Jump
    public bool LandingThisFrame { get; private set; }
    private bool _isJumping;
    private float _lastJumpPressed = Constants.kNever;
    private float _lastGrounded = Constants.kNever;
    private float _jumpHoldTime;
    private bool _coyoteUsed;
    private bool _wasInAir;
    // Includes two cases: player walk off a platform and fall, or jump.
    private bool IsInAir()
    {
        return PlayerFeet.GetComponent<PlayerFeet>().IsInAir();
    }
    #endregion

    // Callback functions for attacks.
    public Attacks.AttackerDelegate energyChangeDelegate(float change)
    {
        return delegate (GameObject victim)
        {
            Energy = Mathf.Clamp(Energy + change, 0, MaxEnergy);
        };
    }


    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        // Prevent sleeping for continous collision detection. 
        // Sleeping objs can't trigger collision.
        _rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
        Health = MaxHealth;
        _rb.gravityScale = _defaultGravityScale;
    }

    private void Update()
    {
        DieCheck();

        GatherInput();
        Land();
        Walk();
        Jump();
        Attack();
    }

    void FixedUpdate()
    {
        if (IsInAir() && _rb.velocity.y < 0)
        {
            if (_rb.velocity.y < -_maxFallSpeed)
                _rb.velocity = new Vector2(_rb.velocity.x, -_maxFallSpeed);
            _rb.gravityScale = _fallingGravityScale;
        }
    }

    private bool CheckAttackResource(Constants.AttackType type)
    {
        switch (type)
        {
            case Constants.AttackType.Bullet:
                {
                    return Energy >= _bulletEnergyCost;
                }
            default: return true;
        }
    }

    private Constants.AttackType GetAttackType()
    {
        // Pressed attack or has buffered attack.
        bool hasBufferedAttack = _lastAttackDown + _attackBuffer >= Time.time;
        if (FrameInput.AttackDown() || hasBufferedAttack)
        {
            // Return false when still in CD, or still attacking.
            if (_isAttacking || _lastAttackFinish + _attackCoolDown >= Time.time)
            {
                return Constants.AttackType.None;
            }
            Constants.AttackType attackType;
            if (FrameInput.AttackDown())
            {
                attackType = FrameInput.BladeDown ?
                        Constants.AttackType.Blade :
                        Constants.AttackType.Bullet;
            }
            else // Check buffered attack
            {
                attackType = _lastAttackType;
            }

            // Clear buffered attack.
            _lastAttackDown = Constants.kNever;
            _lastAttackType = Constants.AttackType.None;

            if (!CheckAttackResource(attackType)) return Constants.AttackType.None;

            return attackType;
        }
        return Constants.AttackType.None;
    }

    private void Attack()
    {
        var type = GetAttackType();
        if (type != Constants.AttackType.None)
        {
            StartCoroutine(FireAttack(type));
        }
    }

    IEnumerator FireAttack(Constants.AttackType type)
    {
        if (type == Constants.AttackType.None)
        {
            yield break;
        }

        GameObject obj = null;
        float attackLastTime = 0;
        Vector3 direction = Vector3.right;
        if (_currentFlip) direction *= -1;

        if (type == Constants.AttackType.Bullet)
        {
            energyChangeDelegate(-_bulletEnergyCost).Invoke(null);
            obj = Bullet.Create(gameObject, null, direction, BulletDamage, _bulletSpeed);
        }
        else
        {
            Attacks.AttackerDelegate del = energyChangeDelegate(_bladeEnergyBoost);
            obj = Blade.Create(gameObject, del, direction, BulletDamage);
        }

        Attacks attack = obj.GetComponent<Attacks>();
        attackLastTime = attack.LastingTime;
        _isAttacking = true;

        for (float timer = attackLastTime; timer >= 0; timer -= Time.deltaTime)
        {
            if (_earlyTerminateAttack)
            {
                if (type == Constants.AttackType.Blade)
                {
                    // Cancel melee attack immediately when flip.
                    // TODO: move this to Blade class, to check Attacker.
                    attack.Destruct();
                }
                _earlyTerminateAttack = false;
                break;
            }
            // Wait for next frame.
            yield return null;
        }
        _isAttacking = false;
        _earlyTerminateAttack = false;
        _lastAttackFinish = Time.time;
    }


    private void Land()
    {
        if (!IsInAir())
        {
            if (_wasInAir)
            {
                _jumpHoldTime = 0;
                _isJumping = false;
                _coyoteUsed = false;
                _wasInAir = false;
                _rb.gravityScale = _defaultGravityScale;
            }
            _lastGrounded = Time.time;
        }
        else
        {
            _wasInAir = true;
        }
    }

    private void Jump()
    {
        if (CanStartJump())
        {
            _jumpHoldTime = 0;
            _isJumping = true;
            // Sometimes velocity can still be negative at the moment of landing.
            // Change to 0 for consistent Force effect.
            if (_rb.velocity.y != 0)
            {
                _rb.velocity = new Vector2(_rb.velocity.x, 0);
            }
            _rb.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
        else if (_isJumping && FrameInput.JumpHold)
        {
            if (_jumpHoldTime < _maxJumpHoldTime)
            {
                _jumpHoldTime = Mathf.Clamp(_jumpHoldTime + Time.deltaTime,
                                                0, _maxJumpHoldTime);
                _rb.AddForce(Vector2.up * _jumpForce * Time.deltaTime, ForceMode2D.Force);
            }
        }
        else if (_isJumping && FrameInput.JumpUp)
        {
            if (_jumpHoldTime < _maxJumpHoldTime)
            {
                _rb.velocity = new Vector2(_rb.velocity.x,
                    CalculateInitialFallSpeed(_jumpHoldTime, _rb.velocity.y));
                _jumpHoldTime = _maxJumpHoldTime;
            }
        }
    }

    private void Walk()
    {
        if (FrameInput.X != 0)
        {
            transform.position += Mathf.Sign(FrameInput.X) * Vector3.right *
                        Time.deltaTime * _horizontalSpeed;

            _currentFlip = FrameInput.X < 0;
            if (_currentFlip ^ _prevFlip)
            {
                // Cancel current attacking when flip
                if (_isAttacking) _earlyTerminateAttack = true;

                // Flip player sprite
                GetComponent<SpriteRenderer>().flipX = _currentFlip;
            }
            _prevFlip = _currentFlip;
            // apex bonus? 
        }
    }

    private void GatherInput()
    {
        FrameInput = new FrameInput
        {
            JumpDown = Input.GetButtonDown(Constants.kJump),
            JumpUp = Input.GetButtonUp(Constants.kJump),
            JumpHold = Input.GetButton(Constants.kJump),
            X = Input.GetAxisRaw(Constants.kHorizontal),
            BladeDown = Input.GetButtonDown(Constants.kBladeAttack),
            BulletDown = Input.GetButtonDown(Constants.kBulletAttack)
        };
        // Record jump press before landing
        // 
        if (IsInAir() && FrameInput.JumpDown)
        {
            _lastJumpPressed = Time.time;
        }
        if (FrameInput.AttackDown())
        {
            _lastAttackDown = Time.time;
            _lastAttackType = FrameInput.BladeDown ?
                                Constants.AttackType.Blade :
                                Constants.AttackType.Bullet;
        }
    }

    private bool CanStartJump()
    {
        if (IsInAir())
        {
            // Coyote
            // Should trigger ONLY when FALL off platform.
            // TODO: Only check time gap may not be enough, imagine player dash
            // far from platform and press jump.
            if (!_isJumping && FrameInput.JumpDown && !_coyoteUsed &&
                    (Time.time - _lastGrounded <= _coyoteThreshold))
            {
                // Prevent using coyote twice;
                Debug.Log("Coyote triggered: " + _lastGrounded + " " + Time.time);
                _coyoteUsed = true;
                return true;
            }
            return false;
        }
        else
        {
            if (FrameInput.JumpDown) return true;
            if (_lastJumpPressed + _jumpBuffer >= Time.time)
            {
                // TODO:
                // If press jump right before landing, the following buffered 
                // jump can be much shorted than regular one-hit jump.
                // This is because the former holding time is shortened.
                // Say we time between press/release space is 0.1s. 
                // For buffered jump, first 0.05s we pressed before landing, then
                // ascending for 0.05s, then release, speed decays immediately to 0.
                // For normal jump, force is added for full 0.1s before releasing.
                // So the later trajectory can be higher than former.
                // Debug.Log("Jump buffer triggered." + _lastJumpPressed + " " + Time.time);
                _lastJumpPressed = 0;
                return true;
            }
            return false;
        }
    }

    private float CalculateInitialFallSpeed(float holdTime, float initSpeed)
    {
        // If hold full 0.35 seconds, no speed decay after release
        // If no hold, speed should decay to 0
        // something like:
        // max(hold, 0.2) --> 0
        // min(hold, 0.35) --> initSpeed
        // y = initSpeed/(0.35-0.2)* x - 0.2/(0.35-0.2)*initSpeed
        holdTime = Mathf.Max(holdTime, _minJumpHoldTime);
        float a = initSpeed / (_maxJumpHoldTime - _minJumpHoldTime);
        float b = -_minJumpHoldTime * initSpeed / (_maxJumpHoldTime - _minJumpHoldTime);
        return Mathf.Max(0, a * holdTime + b);
    }

    private void DieCheck()
    {
        //  TODO: parameterize
        if (transform.position.y < -30 || Health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        SceneManager.LoadScene(Constants.kSceneDefault);
    }

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
        Health -= damage;
        // Shock time
    }
}
