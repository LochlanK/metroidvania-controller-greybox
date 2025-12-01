using System.Collections;
using System;
using UnityEngine;

public class CharacterPhysicsManager : MonoBehaviour
{
    [SerializeField] private ScriptableMovementStats movementStats;

    [Header("Collider References")]
    [SerializeField] private Collider2D bodyCollider;
    [SerializeField] private Collider2D footCollider;
    [SerializeField] private Collider2D headCollider;

    [Header("Rigidbody")]
    [SerializeField] Rigidbody2D rb2D;

    //Will be injected by Character.cs
    private Vector2 moveVelocity;
    private Vector2 moveInput;
    private bool isSprinting;

    private bool isFacingRight = false;

    // Sensors
    private RaycastHit2D groundHit;
    private RaycastHit2D headHit;
    private bool isGrounded;
    private bool bumpedHead;

    //jump runtime vars
    public float VerticalVelocity {get; private set;}
    private bool isJumping;
    private bool isFastFalling;
    private bool isFalling;
    private float fastFallTime;
    private float fastFallReleaseSpeed;
    private int jumpsUsedCounter;

    //apex runtime vars
    private float apexPoint;
    private float timePastApexThreshold;
    private bool isPastApexThreshold;

    //jump buffer runtime vars
    private float jumpBufferTimer;
    private bool jumpReleasedDuringBuffer;

    //coyote jump timer runtime.
    private float coyoteTimer;

    public void Init()
    {
        isFacingRight = true;
        moveInput = Vector2.zero;
        moveVelocity = Vector2.zero;
    }

    /// <summary>
    /// Run this in Update.
    /// </summary>
    public void JumpChecks(bool jumpPressed, bool jumpHeld, bool jumpReleased)
    {
        //When pressed - passed in by Character.cs
        if (jumpPressed)
        {
            jumpBufferTimer = movementStats.JumpBufferTime;
            jumpReleasedDuringBuffer = false;
        }

        //When released - passed in by Character.cs
        if (jumpReleased)
        {
            if(jumpBufferTimer > 0f)
            {
                jumpReleasedDuringBuffer = true;
            }

            if(isJumping && VerticalVelocity > 0f)
            {
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                    isFastFalling = true;
                    fastFallTime = movementStats.TimeForUpwardCancel;
                    VerticalVelocity = 0f;
                }
                else
                {
                    isFastFalling = true;
                    fastFallReleaseSpeed = VerticalVelocity;
                }
            }
        }

        //Initiate Jump with buffering and coyote time
        if(jumpBufferTimer > 0f && !isJumping && (isGrounded || coyoteTimer > 0f))
        {
            InitiateJump(1);
            if (jumpReleasedDuringBuffer)
            {
                isFastFalling = true;
                fastFallReleaseSpeed = VerticalVelocity;
            }
        }
        //Double Jump
        else if (jumpBufferTimer > 0f && isJumping && jumpsUsedCounter < movementStats.NumberOfJumpsAllowed)
        {
            isFastFalling = false;
            InitiateJump(1);
        }
        //Jump case where falling from ledge
        else if (jumpBufferTimer > 0f && isFalling && jumpsUsedCounter < movementStats.NumberOfJumpsAllowed - 1)
        {
            InitiateJump(2);
            isFastFalling = false;
        }
        //Landed
        if((isJumping || isFalling) && isGrounded && VerticalVelocity <= 0f)
        {
            isJumping = false;
            isFalling = false;
            isFastFalling = false;
            fastFallTime = 0f;
            isPastApexThreshold = false;
            jumpsUsedCounter = 0;
            VerticalVelocity = Physics2D.gravity.y;
        }
    }

    private void InitiateJump(int numOfJumpsUsed)
    {
        if (!isJumping)
        {
            isJumping = true;
        }

        jumpBufferTimer = 0f;
        jumpsUsedCounter += numOfJumpsUsed;
        VerticalVelocity = movementStats.InitialJumpVelocity;

    }

    public void UpdateTimers()
    {
        jumpBufferTimer -= Time.deltaTime;
        if (!isGrounded)
        {
            coyoteTimer -= Time.deltaTime;
        }
        else
        {
            coyoteTimer = movementStats.JumpCoyoteTime;
        }
    }

    private void Jump()
    {
        //Apply gravity during jump
        if (isJumping)
        {
            //check for head bump
            if (bumpedHead)
            {
                isFastFalling = true;
            }
        }

        //gravity ascending
        if(VerticalVelocity >= 0f)
        {
            apexPoint = Mathf.InverseLerp(movementStats.InitialJumpVelocity, 0f, VerticalVelocity);
            if (apexPoint > movementStats.ApexThreshold)
            {
                if (!isPastApexThreshold)
                {
                    isPastApexThreshold = true;
                    timePastApexThreshold = 0f;
                }

                if (isPastApexThreshold)
                {
                    timePastApexThreshold += Time.fixedDeltaTime;
                    if(timePastApexThreshold < movementStats.ApexHangTime)
                    {
                        VerticalVelocity = 0f;
                    }
                    else
                    {
                        VerticalVelocity = -0.01f;
                    }
                }

            }
            //Gravity on ascending but not past the threshold
            else
            {
                VerticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
                if (isPastApexThreshold)
                {
                    isPastApexThreshold = false;
                }
            }
        }

        //Gravity on Descent
        else if (!isFastFalling)
        {
            VerticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
        }

        else if (VerticalVelocity < 0f)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
        }

        //Jump Cut

        if (isFastFalling)
        {
            if(fastFallTime >= movementStats.TimeForUpwardCancel)
            {
                VerticalVelocity += movementStats.Gravity * movementStats.GravityOnReleaseMultiplier * Time.fixedDeltaTime;
            }
            else if (fastFallTime < movementStats.TimeForUpwardCancel){
                VerticalVelocity = Mathf.Lerp(fastFallReleaseSpeed, 0f, (fastFallTime/movementStats.TimeForUpwardCancel));
            }
            fastFallTime += Time.fixedDeltaTime;
        }

        //normal gravity while falling off ledge
        if (!isGrounded && !isJumping)
        {
            if (!isFalling)
            {
                isFalling = true;
            }
            VerticalVelocity += movementStats.Gravity * Time.fixedDeltaTime;
        }

        //clamp falling speeds
        //Need to expose 100f as a variable if needing to go above it or have different values per level.
        VerticalVelocity = Mathf.Clamp(VerticalVelocity, -movementStats.MaxFallSpeed, 100f);

        //apply to rigidbody.
        rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, VerticalVelocity);

    }

    //Call from character.cs to update the input values for the physics manager.
    public void UpdateMovementInput(Vector2 cartesianInput, bool sprinting = false)
    {
        moveInput = cartesianInput;
        isSprinting = sprinting;
    }

    //immutable return to check if facing right or is grounded. Probably add more later for unpacking.
    public (bool grounded, bool facingRight) ProcessPhysics()
    {
        CollisionChecks();

        Jump();
        if (isGrounded)
        {
            Move(movementStats.GroundAcceleration, movementStats.GroundDeceleration);
        }
        else
        {
            Move(movementStats.AirAcceleration, movementStats.AirDeceleration);
        }
        
        return (grounded: isGrounded, facingRight: isFacingRight);
    }

    //physically move the character.
    private void Move(float acceleration, float deceleration)
    {
        TurnCheck();
        Vector2 targetHorizontalVelocity = Vector2.zero;
        if(moveInput != Vector2.zero)
        {
            if (isSprinting)
            {
                targetHorizontalVelocity = new Vector2(moveInput.x, 0f) * movementStats.MaxRunSpeed;
            }
            else
            {
                targetHorizontalVelocity = new Vector2(moveInput.x, 0f) * movementStats.MaxWalkSpeed;
            }

            moveVelocity = Vector2.Lerp(moveVelocity, targetHorizontalVelocity, acceleration * Time.fixedDeltaTime);
            rb2D.linearVelocity = new Vector2(moveVelocity.x, rb2D.linearVelocity.y);
        }
        else if (moveInput == Vector2.zero)
        {
            moveVelocity = Vector2.Lerp(moveVelocity, Vector2.zero, deceleration * Time.fixedDeltaTime);
            rb2D.linearVelocity = new Vector2(moveVelocity.x, rb2D.linearVelocity.y);
        }
    }

    private void TurnCheck()
    {
        if(isFacingRight && moveInput.x < 0f)
        {
            Turn(false);
        }
        else if (!isFacingRight && moveInput.x > 0f)
        {
            Turn(true);
        }
    }

    private void Turn(bool turnRight)
    {
        if (turnRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        else
        {
            isFacingRight = false;
            transform.Rotate(0f,-180f,0f);
        }
    }

    private void IsGrounded()
    {
        Vector2 boxCastOrigin = new Vector2(footCollider.bounds.center.x, footCollider.bounds.min.y);
        Vector2 boxCastSize = new Vector2(footCollider.bounds.size.x, movementStats.GroundDetectionRayLength);
        groundHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.down, movementStats.GroundDetectionRayLength, movementStats.GroundedLayer);
        if (groundHit.collider != null)
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if (movementStats.DebugShowIsGroundedBox)
        {
            Color rayColor;
            if (isGrounded)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            //Draw Gizmos
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * movementStats.GroundDetectionRayLength,rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + boxCastSize.x / 2, boxCastOrigin.y), Vector2.down * movementStats.GroundDetectionRayLength,rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2, boxCastOrigin.y - movementStats.GroundDetectionRayLength), Vector2.right * boxCastSize.x,rayColor);

        }

    }

    private void BumpedHead()
    {
        Vector2 boxCastOrigin = new Vector2(footCollider.bounds.center.x, bodyCollider.bounds.max.y);
        Vector2 boxCastSize = new Vector2(footCollider.bounds.size.x * movementStats.HeadWidth, movementStats.HeadDetectionRayLength);
        headHit = Physics2D.BoxCast(boxCastOrigin, boxCastSize, 0f, Vector2.up, movementStats.HeadDetectionRayLength, movementStats.GroundedLayer);
        if (headHit.collider != null)
        {
            bumpedHead = true;
        }
        else
        {
            bumpedHead = false;
        }

        if (movementStats.DebugShowHeadBumpBox)
        {
            float headWidth = movementStats.HeadWidth;
            Color rayColor;
            if (bumpedHead)
            {
                rayColor = Color.green;
            }
            else
            {
                rayColor = Color.red;
            }

            //Draw Gizmos
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y), Vector2.up * movementStats.HeadDetectionRayLength,rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x + (boxCastSize.x / 2) * headWidth, boxCastOrigin.y), Vector2.up * movementStats.GroundDetectionRayLength,rayColor);
            Debug.DrawRay(new Vector2(boxCastOrigin.x - boxCastSize.x / 2 * headWidth, boxCastOrigin.y + movementStats.HeadDetectionRayLength), Vector2.right * boxCastSize.x * headWidth,rayColor);

        }
    }

    private void CollisionChecks()
    {
        IsGrounded();
        BumpedHead();
    }




}
