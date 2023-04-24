using UnityEngine;
public class PlayerInput : MonoBehaviour
{
    private PlayerControls playerControls;
    [Header("Parameters")]
    public float speed;
    private float normalSpeed;
    private float runSpeed;
    public float jumpForce;
    public float jumpMultiplier;
    [Header("On Move Parameters")]
    private float _horizontalInput;
    private float _moveSpeed;
    public float HorizontalInput => _horizontalInput;
    public float MoveSpeed => _moveSpeed;
    

    [Header("Components")]
    public GameObject legs;
    public float radius;
    public Rigidbody2D rb;
    private bool _isGrounded;
    public bool IsGrounded => _isGrounded;
    private bool _isEndJump;
    private bool _isInteract;
    public bool IsInteract => _isInteract;

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
        _isGrounded = Physics2D.OverlapCircle(legs.transform.position, radius, LayerMask.GetMask("Ground"));
        MoveInput();
        JumpInput();
        InteractInput();
        PlayerFlip();
    }

    private void MoveInput()
    {
        _horizontalInput = playerControls.PlayerActions.Movement.ReadValue<Vector2>().x;
        rb.velocity = new Vector2(_horizontalInput * speed, rb.velocity.y);
        _moveSpeed = Input.GetKey(KeyCode.LeftShift) ? 2 : 1;
        speed = _moveSpeed > 1 ? runSpeed : normalSpeed;
    }
    private void JumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jumpForce * jumpMultiplier));
        }
    }
    private void InteractInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && _isGrounded)
        {
            if (_isInteract)
            {
                _isInteract = false;
            }
            else
            {
                _isInteract = true;
            }
        }
    }
    
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
}