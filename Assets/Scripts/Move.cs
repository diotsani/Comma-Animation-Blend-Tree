using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class Move : MonoBehaviour
{
    private PlayerControls playerControls;
    public AnimationsState animationState;
    public AnimationController animationController;
    public float speed;
    private float normalSpeed;
    private float runSpeed;
    public float jumpForce;
    public float jumpMultiplier;
    public float groundValue;
    [Header("On Move Parameters")]
    public float horizontalInput;
    public float xValue;
    public float onMoveSpeed;
    public float moveAnimationValue;
    public float maxWalkVelocity = 1.3f;
    public float maxRunVelocity = 2.3f;
    public float endMoveVelocity = 0.6f;
    [Header("On Air Parameters")]
    public float airSpeed = 1f;
    public float currentAirVelocity;
    public float maxAirVelocity = 0.38f;
    public float onLoopingJump = 0.18f;
    public float timeEndJump = 0.18f;
    public float maxJumpTime = 0.18f;
    public bool onEndJump;
    [Header("On Jump Parameter")]
    public float currentJumpVelocity;
    public float jumpDecrement;
    [Header("On Interact Parameters")]
    public float currentFacingDirection;
    public float maxInteractValue;
    public float currentInteractValue;
    public float moveInteractValue;
    public float maxMoveInteractValue;
    public bool isInteract;
    public bool onPush;
    public bool onPull;

    [Header("Components")]
    public Animator anim;
    public GameObject legs;
    public float radius;
    public Rigidbody2D rb;
    public bool isGrounded;
    public bool isOnAir;
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int XSpeed = Animator.StringToHash("XSpeed");
    private static readonly int YSpeed = Animator.StringToHash("YSpeed");
    private static readonly int EndMove = Animator.StringToHash("OnEndMove");
    private static readonly int Jump = Animator.StringToHash("JumpValue");
    private static readonly int EndJump = Animator.StringToHash("OnEndJump");
    private static readonly int Interacted = Animator.StringToHash("OnInteracted");
    private static readonly int InteractMoveValue = Animator.StringToHash("InteractMoveValue");
    private static readonly int InteractValue = Animator.StringToHash("InteractValue");
    private void Awake()
    {
        playerControls = new PlayerControls();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        normalSpeed = speed;
        runSpeed = speed * 1.8f;
    }

    private void OnEnable()
    {
        playerControls.PlayerActions.Enable();
    }
    private void OnDisable()
    {
        playerControls.PlayerActions.Disable();
    }
    void Update()
    {
        PlayerFlip();

        horizontalInput = playerControls.PlayerActions.Movement.ReadValue<Vector2>().x;
        rb.velocity = new Vector2(horizontalInput * speed, rb.velocity.y);
        xValue = Mathf.Abs(horizontalInput);

        onMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        isGrounded = Physics2D.OverlapCircle(legs.transform.position, radius, LayerMask.GetMask("Ground"));
        groundValue = isGrounded ? 0f : 1f;

        if (xValue == 1 && onMoveSpeed == 1 && isGrounded)
        {
            WalkState();
        }
        else if (xValue == 1 && onMoveSpeed >= 2 && isGrounded)
        {
            RunState();
        }
        else if (xValue == 0 && isGrounded)
        {
            EndMoveState();
        }

        JumpState();
        InteractState();

        anim.SetFloat(XSpeed, moveAnimationValue);
        anim.SetFloat(YSpeed, currentAirVelocity);
        anim.SetFloat(Grounded, groundValue);
        anim.SetBool(Interacted, isInteract);
        anim.SetFloat(InteractValue, currentInteractValue);
        anim.SetFloat(InteractMoveValue, moveInteractValue);
        UpdateAnimationsState();
    }
    private void WalkState()
    {
        animationState = AnimationsState.Move;
        speed = normalSpeed;

        if (moveAnimationValue < 1) moveAnimationValue = 1; // Set First Value to Play Start Walk Animation
        if (moveAnimationValue < maxWalkVelocity) moveAnimationValue += onMoveSpeed * Time.deltaTime;
        else if (moveAnimationValue > maxWalkVelocity) moveAnimationValue = maxWalkVelocity;
    }
    private void RunState()
    {
        animationState = AnimationsState.Move;
        speed = runSpeed;

        if (moveAnimationValue < 2) moveAnimationValue = 2; // Set First Value to Play Start Run Animation
        if (moveAnimationValue < maxRunVelocity) moveAnimationValue += onMoveSpeed * Time.deltaTime;
    }
    private void EndMoveState()
    {
        if (moveAnimationValue >= maxWalkVelocity)
        {
            animationState = AnimationsState.EndMove;
            moveAnimationValue = endMoveVelocity;
        }
        else moveAnimationValue = 0;

        if (moveAnimationValue > 0) moveAnimationValue -= onMoveSpeed * Time.deltaTime;
        else if (moveAnimationValue < 0)
        {
            animationState = AnimationsState.Idle;
            moveAnimationValue = 0;
        }
    }
    private void JumpState()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentAirVelocity = maxAirVelocity;
            anim.SetFloat(Jump, 2);
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce * jumpMultiplier));
            currentJumpVelocity = rb.velocity.y;
        }

        if (isGrounded)
        {
            currentAirVelocity = maxAirVelocity;
            anim.SetBool(EndJump, false);
        }
        else
        {
            IncrementAirVelocity();
        }
    }
    private void InteractState()
    {
        if (Input.GetKeyDown(KeyCode.E) && isGrounded)
        {
            if (isInteract)
            {
                isInteract = false;
                animationState = AnimationsState.Idle;
            }
            else
            {
                anim.SetBool(EndMove, false);
                isInteract = true;
                currentFacingDirection = transform.localScale.x;
                animationState = AnimationsState.Interact;
                maxInteractValue = GetInteractValue();
            }
        }

        if (isInteract)
        {
            if(horizontalInput != 0)currentInteractValue = horizontalInput * currentFacingDirection;
            else currentInteractValue = currentFacingDirection;

            if (xValue > 0 && moveInteractValue < maxMoveInteractValue)
            {
                moveInteractValue += 1 * Time.deltaTime;
            }
            else if (xValue == 0 && moveInteractValue > 0)
            {
                moveInteractValue = 0;
            }
        }
        else
        {
            currentInteractValue = 0;
            moveInteractValue = 0;
        }
    }
    private void IncrementAirVelocity()
    {
        onEndJump = Physics2D.OverlapCircle(legs.transform.position, radius + this.timeEndJump, LayerMask.GetMask("Ground"));
        var jumpValue = onEndJump ? 1f : 2f;
        anim.SetFloat(Jump, jumpValue);
        if (currentAirVelocity >= onLoopingJump)
        {
            //currentAirVelocity += Time.deltaTime;
            currentAirVelocity -= airSpeed * Time.deltaTime;
        }

        if (onEndJump && currentAirVelocity >= maxJumpTime && currentAirVelocity < onLoopingJump)
        {
            currentAirVelocity = maxJumpTime;
            anim.SetBool(EndJump, true);
            Debug.Log("End Jump");
        }
    }
    private void PlayerFlip()
    {
        if(isInteract)return;
        if (Input.GetAxis("Horizontal") > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
    private void UpdateAnimationsState()
    {
        switch (animationState)
        {
            case AnimationsState.Idle: anim.SetBool(EndMove, false); break;
            case AnimationsState.Move: anim.SetBool(EndMove, false); break;
            case AnimationsState.EndMove: anim.SetBool(EndMove, true); break;
        }
    }

    public bool IsInteracted()
    {
        return isInteract;
    }
    public float GetInteractValue()
    {
        var interactValue = 0f;
        interactValue = !onPush ? !onPull ? 0 : -1f : 1f;
        return interactValue;
    }
}