using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Move : MonoBehaviour
{
    public float speed;
    public float jumpForce;
    public float jumpMultiplier;
    [Header("On Move Parameters")]
    public float moveSpeed;
    public float currentMoveVelocity;
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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, rb.velocity.y);

        moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;

        if (Input.GetAxis("Horizontal") !=0 && moveSpeed >= 2)
        {
            //currentMoveVelocity -= moveSpeed * Time.deltaTime;
            if (currentMoveVelocity < 2) currentMoveVelocity = 2;
            if(currentMoveVelocity < maxRunVelocity) currentMoveVelocity += moveSpeed * Time.deltaTime;
            Debug.Log("Run");
        }
        else if (Input.GetAxis("Horizontal")!=0 && moveSpeed == 1)
        {
            if (currentMoveVelocity < 1) currentMoveVelocity = 1;
            if(currentMoveVelocity < maxWalkVelocity) currentMoveVelocity += moveSpeed * Time.deltaTime;
            if (currentMoveVelocity > maxWalkVelocity) currentMoveVelocity = maxWalkVelocity;
            Debug.Log("Walk");
        }
        else
        {
            currentMoveVelocity = 0;
        }

        if (Input.GetAxis("Horizontal") > 0f)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (Input.GetAxis("Horizontal") < 0f)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
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
        
        anim.SetFloat(XSpeed,currentMoveVelocity);
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
}
