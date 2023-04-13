using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Move : MonoBehaviour
{
    private PlayerControls playerControls;
    public float speed;
    private float normalSpeed;
    private float runSpeed;


    public float jumpForce;
    public float jumpMultiplier;
    [Header("On Move Parameters")]
    public float moveHorizontal;
    public float onMoveSpeed;
    public float moveAnimationValue;
    public float maxWalkVelocity = 1.3f;
    public float maxRunVelocity = 2.3f;
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

    public Animator anim;

    public GameObject legs;
    public float radius;
    public Rigidbody2D rb;
    public bool isGrounded;
    public bool isOnAir;
    private static readonly int Grounded = Animator.StringToHash("Grounded");
    private static readonly int XSpeed = Animator.StringToHash("XSpeed");
    private static readonly int YSpeed = Animator.StringToHash("YSpeed");
    private static readonly int Jump = Animator.StringToHash("JumpValue");
    private static readonly int EndJump = Animator.StringToHash("OnEndJump");

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

        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);
        moveHorizontal = Mathf.Abs(playerControls.PlayerActions.Movement.ReadValue<Vector2>().x);
        Debug.Log($"Move Horizontal: {moveHorizontal}");

        onMoveSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        if (moveHorizontal == 1 && onMoveSpeed >= 2)
        {
            speed = runSpeed;
            //currentMoveVelocity -= moveSpeed * Time.deltaTime;
            if (moveAnimationValue < 2) moveAnimationValue = 2;
            if(moveAnimationValue < maxRunVelocity) moveAnimationValue += onMoveSpeed * Time.deltaTime;
            Debug.Log("Run");
    
        }
        else if (moveHorizontal == 1 && onMoveSpeed == 1)
        {
            speed = normalSpeed;
            var t = (maxWalkVelocity + 1 )/2;
            if (moveAnimationValue < 1) moveAnimationValue = 1;
            //if (currentMoveVelocity < maxWalkVelocity) currentMoveVelocity += moveSpeed * Time.deltaTime;
            moveAnimationValue += onMoveSpeed * Time.deltaTime;
            if (moveAnimationValue > maxWalkVelocity) moveAnimationValue = maxWalkVelocity;
            Debug.Log("Walk");
        }
        else
        {
            moveAnimationValue = 0;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentAirVelocity = maxAirVelocity;
            anim.SetFloat(Jump, 2);
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce * jumpMultiplier));
            currentJumpVelocity = rb.velocity.y;
        }
        
        if(isGrounded)
        {
            currentAirVelocity = maxAirVelocity;
            anim.SetBool(EndJump,false);
        }
        else
        {
            //currentAirVelocity = rb.velocity.y;
          
            // Debug.Log($"Current Air Velocity: {currentAirVelocity}");
            // if (jumpDecrement > 0)
            // {
            //     jumpDecrement = (currentJumpVelocity - Time.deltaTime) / currentJumpVelocity;
            // }
            IncrementAirVelocity();
            //anim.SetFloat("XSpeed", 0);
        }
        isGrounded = Physics2D.OverlapCircle(legs.transform.position, radius, LayerMask.GetMask("Ground"));
        var groundValue = isGrounded ? 0f : 1f;

        anim.SetFloat(XSpeed,moveAnimationValue);
        anim.SetFloat(YSpeed, currentAirVelocity);
        anim.SetFloat(Grounded, groundValue);
    }
    private void IncrementAirVelocity()
    {
        onEndJump = Physics2D.OverlapCircle(legs.transform.position, radius + this.timeEndJump, LayerMask.GetMask("Ground"));
        var jumpValue = onEndJump ? 1f : 2f;
        anim.SetFloat(Jump, jumpValue);
        if(currentAirVelocity >= onLoopingJump)
        {
            //currentAirVelocity += Time.deltaTime;
            currentAirVelocity -= airSpeed * Time.deltaTime;
        }

        if (onEndJump && currentAirVelocity >= maxJumpTime && currentAirVelocity < onLoopingJump)
        {
            currentAirVelocity = maxJumpTime;
            anim.SetBool(EndJump,true);
            Debug.Log("End Jump");
        }
    }
    private void PlayerFlip()
    {
        if (Input.GetAxis("Horizontal") > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
