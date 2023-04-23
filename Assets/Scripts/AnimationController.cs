using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum AnimationsState
{
    Idle,
    Move,
    EndMove,
    Interact,
}
public class AnimationController : MonoBehaviour
{
    [Header("Components")]
    public AnimationsState animationState;
    public Animator animator;
    public PlayerInput playerInput;
    [Header("Parameters")]
    public float groundValue;
    [Header("On Move Parameters")]
    //public float horizontalInput;
    public float xValue;
    public float onMoveSpeed;
    public float moveAnimationValue;
    public float maxWalkVelocity = 1.3f;
    public float maxRunVelocity = 2.3f;
    public float endMoveVelocity = 0.6f;
    [Header("On Jump Parameters")]
    public float airSpeed = 1f;
    public float jumpAnimationValue;
    public float maxJumpVelocity = 0.38f;
    public float maxLoopingJump = 0.18f;
    public float endJumpTime = 0.18f;
    public float maxJumpTime = 0.18f;
    public bool onEndJump;
    public float radius;
    [Header("On Interact Parameters")]
    public float currentFacingDirection;
    public float currentInteractValue;
    public float moveInteractValue;
    public float maxMoveInteractValue;
    public bool isInteract;
    public bool onPush;
    public bool onPull;

    

    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int XSpeed = Animator.StringToHash("XSpeed");
    private static readonly int YSpeed = Animator.StringToHash("YSpeed");
    private static readonly int EndMove = Animator.StringToHash("OnEndMove");
    private static readonly int Jump = Animator.StringToHash("JumpValue");
    private static readonly int EndJump = Animator.StringToHash("OnEndJump");
    private static readonly int Interacted = Animator.StringToHash("OnInteracted");
    private static readonly int InteractMoveValue = Animator.StringToHash("InteractMoveValue");
    private static readonly int InteractValue = Animator.StringToHash("InteractValue");
    void Update()
    {
        groundValue = playerInput.IsGrounded ? 0f : 1f;

        MovementState();
        JumpState();
        //InteractState();

        animator.SetFloat(XSpeed, moveAnimationValue);
        animator.SetFloat(YSpeed, jumpAnimationValue);
        animator.SetFloat(Grounded, groundValue);
        animator.SetBool(Interacted, isInteract);
        animator.SetFloat(InteractValue, currentInteractValue);
        animator.SetFloat(InteractMoveValue, moveInteractValue);
        UpdateAnimationsState();
    }

    private void MovementState()
    {
        if(!playerInput.IsGrounded)return;
        xValue = Mathf.Abs(playerInput.HorizontalInput);
        onMoveSpeed = playerInput.MoveSpeed;
        switch (xValue > 0)
        {
            case true when onMoveSpeed == 1:
                WalkState();
                break;
            case true when onMoveSpeed >= 2:
                RunState();
                break;
            case false:
                EndMoveState();
                break;
        }
        // if (xValue == 1 && onMoveSpeed == 1)
        // {
        //     WalkState();
        // }
        // else if (xValue == 1 && onMoveSpeed >= 2)
        // {
        //     RunState();
        // }
        // else if (xValue == 0)
        // {
        //     EndMoveState();
        // }
    }
    private void WalkState()
    {
        animationState = AnimationsState.Move;

        if (moveAnimationValue < 1) moveAnimationValue = 1; // Set First Value to Play Start Walk Animation
        if (moveAnimationValue < maxWalkVelocity) moveAnimationValue += 1* Time.deltaTime;
        else if (moveAnimationValue > maxWalkVelocity) moveAnimationValue = maxWalkVelocity;
    }
    private void RunState()
    {
        animationState = AnimationsState.Move;

        if (moveAnimationValue < 2) moveAnimationValue = 2; // Set First Value to Play Start Run Animation
        if (moveAnimationValue < maxRunVelocity) moveAnimationValue += 1 * Time.deltaTime;
    }
    private void EndMoveState()
    {
        if (moveAnimationValue >= maxWalkVelocity)
        {
            animationState = AnimationsState.EndMove;
            moveAnimationValue = endMoveVelocity;
        }

        if (moveAnimationValue > 0) moveAnimationValue -= 1 * Time.deltaTime;
        else if (moveAnimationValue < 0)
        {
            animationState = AnimationsState.Idle;
            moveAnimationValue = 0;
        }
    }
    private void JumpState()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     jumpAnimationValue = maxJumpVelocity;
        //     animator.SetFloat(Jump, 2);
        // }

        if (playerInput.IsGrounded)
        {
            jumpAnimationValue = maxJumpVelocity;
            animator.SetBool(EndJump, false);
        }
        else
        {
            DecrementJumpVelocity();
        }
    }
    // private void InteractState()
    // {
    //     if (Input.GetKeyDown(KeyCode.E) && isGrounded)
    //     {
    //         if (isInteract)
    //         {
    //             isInteract = false;
    //             animationState = AnimationsState.Idle;
    //         }
    //         else
    //         {
    //             animator.SetBool(EndMove, false);
    //             isInteract = true;
    //             currentFacingDirection = transform.localScale.x;
    //             currentInteractValue = currentFacingDirection;
    //             animationState = AnimationsState.Interact;
    //         }
    //     }
    //
    //     if (isInteract)
    //     {
    //         if(horizontalInput != 0)currentInteractValue = horizontalInput * currentFacingDirection;
    //
    //         if (xValue > 0 && moveInteractValue < maxMoveInteractValue)
    //         {
    //             moveInteractValue += 1 * Time.deltaTime;
    //         }
    //         else if (xValue == 0 && moveInteractValue > 0)
    //         {
    //             moveInteractValue = 0;
    //         }
    //     }
    //     else
    //     {
    //         currentInteractValue = 0;
    //         moveInteractValue = 0;
    //     }
    // }
    private void DecrementJumpVelocity()
    {
        //onEndJump = Physics2D.OverlapCircle(legs.transform.position, radius + this.endJumpTime, LayerMask.GetMask("Ground"));
        onEndJump = playerInput.IsEndJump(radius, endJumpTime);
        //var jumpValue = onEndJump ? 1f : 2f;
        //animator.SetFloat(Jump, jumpValue);
        if (jumpAnimationValue >= maxLoopingJump)
        {
            jumpAnimationValue -= airSpeed * Time.deltaTime;
        }

        if (onEndJump && jumpAnimationValue >= maxJumpTime && jumpAnimationValue < maxLoopingJump)
        {
            jumpAnimationValue = maxJumpTime;
            animator.SetBool(EndJump, true);
        }
    }
    
    private void UpdateAnimationsState()
    {
        switch (animationState)
        {
            case AnimationsState.Idle: animator.SetBool(EndMove, false); break;
            case AnimationsState.Move: animator.SetBool(EndMove, false); break;
            case AnimationsState.EndMove: animator.SetBool(EndMove, true); break;
        }
    }
}