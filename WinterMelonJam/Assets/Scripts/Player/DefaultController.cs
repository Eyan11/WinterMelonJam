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
    [SerializeField] private AudioClip jumpSfx;
    private float canJumpTimer;
    private float jumpInputTimer = 0f;
    private bool hasJumped = false;



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

        if(!hasJumped && jumpInputTimer > 0f && (canJumpTimer > 0f || playerManager.IsGrounded))
            Jump();
    }

    /** Applies jump settings and jump force **/
    private void Jump() {
        hasJumped = true;
        canJumpTimer = -1f;
        jumpInputTimer = -1f;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
        playerManager.PlayOneShotSFX(jumpSfx);
    }




    // *** Events *************************************************************


    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        // Not checking if valid here because moveInput needs to be up to date in case player transforms and wants keeps holding move input
        moveInput = context.ReadValue<Vector2>().x;

        if (moveInput != 0)
        {
            moveInput = Mathf.Sign(moveInput);

            if (PlayerManager.IsValidContext(gameObject) == false) return;

            spriteRenderer.flipX = !(moveInput > 0);
        }
    }

    // Uses InputAction to get the jump input
    public void OnJump(InputAction.CallbackContext context)
    {
        if (PlayerManager.IsValidContext(gameObject) == false) return;
        
        if(context.started)
        {
            jumpInputTimer = jumpBufferTime;

            if(!hasJumped && (playerManager.IsGrounded || canJumpTimer > 0f))
                Jump();
        }
    }

    private void OnGrounded()
    {
        hasJumped = false;
        anim.SetBool("isGrounded", true);
    }

    private void OnUngrounded()
    {
        canJumpTimer = coyoteTime; 
        anim.SetBool("isGrounded", false);
    }

    // Subscribed to PlayerManager onDeathEvent
    private void OnDeath()
    {
        this.enabled = false;
        anim.SetBool("isGrounded", false);
    }

    // Called when entering this mask transformation
    private void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;
        playerManager.onDeathEvent += OnDeath;

        anim.SetBool("isGrounded", playerManager.IsGrounded);
        anim.SetBool("isMoving", false);

        if(moveInput != 0)
            spriteRenderer.flipX = !(moveInput > 0);
    }

    // Called when leaving this mask transformation
    private void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;
        playerManager.onDeathEvent -= OnDeath;

        jumpInputTimer = -1f;
        canJumpTimer = -1f;
    }

    // Called by animation event in run animation when foot hits ground
    private void PlayFootstepSfx()
    {
        playerManager.PlayFootstepSfx();
    }
}
