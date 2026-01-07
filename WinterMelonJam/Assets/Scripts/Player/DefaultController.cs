using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class DefaultController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;
    private PlayerManager playerManager;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float moveInput;

    [Header ("Jump")]
    [SerializeField] private float jumpSpeed = 6f;



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

    private void OnGrounded()
    {
        anim.SetBool("isGrounded", true);
    }

    private void OnUngrounded()
    {
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

        playerManager.SetJumpSettings(true, jumpSpeed);
    }

    // Called when leaving this mask transformation
    private void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;
        playerManager.onDeathEvent -= OnDeath;

        playerManager.SetJumpSettings(false, 0f);
    }

    // Called by animation event in run animation when foot hits ground
    private void PlayFootstepSfx()
    {
        playerManager.PlayFootstepSfx();
    }
}
