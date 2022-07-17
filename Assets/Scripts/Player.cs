using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 9f;
    [SerializeField]
    private float jumpForce = 20f;
    [SerializeField]
    private float maxJumpHoldTime = 0.35f;
    [SerializeField]
    private float defaultGravityScale = 4.5f;
    [SerializeField]
    private float fallingGravityScale = 5f;
    [SerializeField]
    private float maxFallSpeed = 18f;
    [SerializeField]
    private GameObject PlayerFeet;

    private Transform trans_;
    private Rigidbody2D rb_;
    private PlayerFeet playerFeet_;

    // Miss a case where player move and fall, isJumping should be true.
    // private bool isJumping_; 
    private float jumpHoldTime_ = 0f;
    private float minJumpHold_ = 0.2f;

    void Start()
    {
        trans_ = GetComponent<Transform>();
        rb_ = GetComponent<Rigidbody2D>();
        playerFeet_ = PlayerFeet.GetComponent<PlayerFeet>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMove();
        HandleAttack();
    }

    void FixedUpdate()
    {
        if (IsInAir())
        {
            if (rb_.velocity.y < 0)
            {
                if (-rb_.velocity.y > maxFallSpeed)
                {
                    rb_.velocity = new Vector2(rb_.velocity.x, -maxFallSpeed);
                }
                rb_.gravityScale = fallingGravityScale;
            }
        }
    }

    private void HandleMove()
    {
        HandleJump();

        // Handle horizontal move
        if (Input.GetButton(Constants.kHorizontal))
        {
            var h = Input.GetAxis(Constants.kHorizontal);
            trans_.position += Mathf.Sign(h) * Vector3.right *
                                    Time.deltaTime * moveSpeed;
        }
    }

    private void HandleJump()
    {
        if (Input.GetButtonDown(Constants.kJump))
        {
            if (!IsInAir())
            {
                jumpHoldTime_ = 0;
                rb_.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
            else
            {
                return;
                // TODO: 2nd jump 
            }
        }
        else if (IsInAir() && Input.GetButton(Constants.kJump))
        {
            if (jumpHoldTime_ < maxJumpHoldTime)
            {
                jumpHoldTime_ = Mathf.Min(jumpHoldTime_ + Time.deltaTime, maxJumpHoldTime);
                // Must xDeltatime, otherwise # forces added will differ per frame rate
                rb_.AddForce(Vector2.up * jumpForce * Time.deltaTime, ForceMode2D.Force);
            }
        }
        else if (IsInAir() && Input.GetButtonUp(Constants.kJump))
        {
            Debug.Log(jumpHoldTime_);
            if (jumpHoldTime_ <= maxJumpHoldTime)
            {
                // If hold max time, keep current speed. Prevent user from holding for a long time and 
                // sudden release before landing to get a pump
                if (jumpHoldTime_ < maxJumpHoldTime)
                {
                    rb_.velocity = new Vector2(rb_.velocity.x,
                                        CalculateInitialFallSpeed(jumpHoldTime_));
                }
                // Prevent jumping twice where sum of hold time < max hold time.
                jumpHoldTime_ = maxJumpHoldTime + 1;
            }
        }
    }

    private void HandleAttack()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag(Constants.kGround))
        {
            jumpHoldTime_ = 0;
            rb_.gravityScale = defaultGravityScale;
        }
    }

    private float CalculateInitialFallSpeed(float holdTime)
    {
        // If hold full 0.35 seconds, no speed decay after release
        // If no hold, speed should decay to 0
        // something like:
        // max(hold, 0.2) --> 0
        // min(hold, 0.35) --> jumpForce
        // y = jumpForce/(0.35-0.2)* x - 0.2/(0.35-0.2)*jumpForce
        holdTime = Mathf.Max(holdTime, minJumpHold_);
        float a = jumpForce / (maxJumpHoldTime - minJumpHold_);
        float b = -minJumpHold_ * jumpForce / (maxJumpHoldTime - minJumpHold_);
        return Mathf.Max(0, a * holdTime + b);
    }

    private bool IsInAir()
    {
        return playerFeet_.IsInAir();
    }
}
