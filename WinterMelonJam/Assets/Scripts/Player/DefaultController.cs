using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class DefaultController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    private PlayerManager playerManager;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float moveInput;

    [Header ("Jump")]
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float canJumpTimer;
    private float jumpInputTimer = 0f;



    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();
    }


    private void Update()
    {
        body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);

        JumpCalculations();
    }



    /** Handles jump input and calling Jump() **/
    private void JumpCalculations() {
        canJumpTimer -= Time.deltaTime;
        jumpInputTimer -= Time.deltaTime;

        if(jumpInputTimer > 0f && canJumpTimer > 0f)
            Jump();
    }

    /** Applies jump settings and jump force **/
    private void Jump() {
        canJumpTimer = -1f;
        jumpInputTimer = -1f;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
    }




    // *** Events *************************************************************


    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        moveInput = context.ReadValue<Vector2>().x;

        if (moveInput != 0)
        {
            moveInput = Mathf.Sign(moveInput);
            if(moveInput > 0)
                spriteRenderer.flipX = false;
            else
                spriteRenderer.flipX = true;
        }
    }

    // Uses InputAction to get the jump input
    public void OnJump(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        if(context.started)
        {
            jumpInputTimer = jumpBufferTime;

            if(playerManager.IsGrounded || canJumpTimer > 0f)
                Jump();
        }
    }

    private void OnGrounded()
    {
        anim.SetBool("isGrounded", true);
    }

    private void OnUngrounded()
    {
        canJumpTimer = coyoteTime; 
        anim.SetBool("isGrounded", false);
    }

    // Called when entering this mask transformation
    private void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;

        anim.SetBool("isGrounded", playerManager.IsGrounded);   // Initialize grounded anim bool
    }

    // Called when leaving this mask transformation
    private void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;

        jumpInputTimer = -1f;
        canJumpTimer = -1f;
        moveInput = 0f;
    }
}
