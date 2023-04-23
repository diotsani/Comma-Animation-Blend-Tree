using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


public class PlayerInput : MonoBehaviour
{
    private PlayerControls playerControls;
    public AnimationsState animationState;
    public AnimationController animationController;
    [Header("Parameters")]
    public float speed;
    private float normalSpeed;
    private float runSpeed;
    public float jumpForce;
    public float jumpMultiplier;
    //public float groundValue;
    [Header("On Move Parameters")]
    private float _horizontalInput;
    private float _moveSpeed;
    public float HorizontalInput => _horizontalInput;
    public float MoveSpeed => _moveSpeed;
    // public float xValue;
    // public float moveAnimationValue;
    // public float maxWalkVelocity = 1.3f;
    // public float maxRunVelocity = 2.3f;
    // public float endMoveVelocity = 0.6f;
    [Header("On Jump Parameters")]
    // public float airSpeed = 1f;
    // public float currentJumpVelocity;
    // public float maxJumpVelocity = 0.38f;
    // public float onLoopingJump = 0.18f;
    // public float timeEndJump = 0.18f;
    // public float maxJumpTime = 0.18f;
    private bool _isEndJump;

    [Header("On Interact Parameters")]
    // public float currentFacingDirection;
    // public float currentInteractValue;
    // public float moveInteractValue;
    // public float maxMoveInteractValue;
    private bool _isInteract; 
    private bool _onPush;
    private bool _onPull;

    [Header("Components")]
    public Animator anim;
    public GameObject legs;
    public float radius;
    public Rigidbody2D rb;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;

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

        _horizontalInput = playerControls.PlayerActions.Movement.ReadValue<Vector2>().x;
        rb.velocity = new Vector2(_horizontalInput * speed, rb.velocity.y);

        _moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        _isGrounded = Physics2D.OverlapCircle(legs.transform.position, radius, LayerMask.GetMask("Ground"));
        
        JumpInput();
        //InteractState();
    }
    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce * jumpMultiplier));
        }
    }
    // private void InteractState()
    // {
    //     if (Input.GetKeyDown(KeyCode.E) && _isGrounded)
    //     {
    //         if (_isInteract)
    //         {
    //             _isInteract = false;
    //         }
    //         else
    //         {
    //             anim.SetBool(EndMove, false);
    //             _isInteract = true;
    //             currentFacingDirection = transform.localScale.x;
    //             currentInteractValue = currentFacingDirection;
    //             animationState = AnimationsState.Interact;
    //         }
    //     }
    //
    //     if (_isInteract)
    //     {
    //         if(_horizontalInput != 0)currentInteractValue = _horizontalInput * currentFacingDirection;
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
    
    private void PlayerFlip()
    {
        if(_isInteract)return;
        if (Input.GetAxis("Horizontal") > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }

    public bool IsEndJump(float radius, float time)
    {
        return _isEndJump = Physics2D.OverlapCircle(legs.transform.position, radius + time, LayerMask.GetMask("Ground"));
    }
    public float GetInteractValue()
    {
        var interactValue = 0f;
        interactValue = !_onPush ? !_onPull ? 0 : -1f : 1f;
        return interactValue;
    }
}