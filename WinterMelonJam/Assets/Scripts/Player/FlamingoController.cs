using UnityEngine;
using UnityEngine.InputSystem;

public class FlamingoController : MonoBehaviour
{
    // Flamingo speeds, mid-air jump power settings
    [SerializeField] private float flamingoMoveSpeed;
    [SerializeField] private float updraftPower;
    [SerializeField] private AudioClip flamingoFlapSfx;
    // LayerMask
    private int groundedMask;
    // State management
    private bool gliding;
    private bool holdingSpacebar;
    private bool usedUpdraft;
    private bool updraftDeactivated;
    private float moveInput;
    // Player info
    private PlayerManager playerManager;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;
    private Vector3 scale;

    // **********************************************
    // UNITY ACTIONS

    // Called when object is first activated
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();

        groundedMask = LayerMask.GetMask("Floor", "Interactable");

        scale = transform.localScale;
    }

    // Flamingo movement
    private void FixedUpdate()
    {
        body.linearVelocityX = moveInput * flamingoMoveSpeed;

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);

        if (gliding)
        {
            if (updraftDeactivated == true) body.linearVelocityY = -0.2f;
            else if (body.linearVelocityY < -0.2f)
            {
                updraftDeactivated = true;
            }
        }
    }

    // Called when entering this mask transformation
    public void OnEnable() 
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;
        playerManager.onDeathEvent += OnDeath;

        anim.SetBool("isGrounded", playerManager.IsGrounded);
        anim.SetBool("isMoving", false);
        anim.SetBool("isGliding", false);

        if(moveInput != 0)
            spriteRenderer.flipX = !(moveInput > 0);
    }

    // Called when leaving this mask transformation
    public void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;
        playerManager.onDeathEvent -= OnDeath;
    }


    // **********************************************
    // EVENTS

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

    // Uses InputAction to enable updraft ability once per glide
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (PlayerManager.IsValidContext(gameObject) == false) return;
        if (context.started == false || gliding == false || usedUpdraft == true) return;

        usedUpdraft = true;
        playerManager.PlayOneShotSFX(flamingoFlapSfx);
        updraftDeactivated = false;

        body.linearVelocityY = updraftPower;
    }

    // Uses InputAction to allow for gliding ability
    public void OnJump(InputAction.CallbackContext context) 
    {
        if (PlayerManager.IsValidContext(gameObject) == false) return;

        if (context.canceled == false)
            holdingSpacebar = true;
        else
            holdingSpacebar = false;

        if(!playerManager.IsGrounded)
        {
            gliding = holdingSpacebar;
            anim.SetBool("isGliding", gliding);
        }

    }

    // Subscribed to on grounded action in player manager
    private void OnGrounded()
    {
        anim.SetBool("isGrounded", true);

        if (gliding)
        {
            gliding = false;
            anim.SetBool("isGliding", gliding);
            usedUpdraft = false;
            updraftDeactivated = true;
        }
    }

    // Subscribed to on ungrounded action in player manager
    private void OnUngrounded()
    {
        anim.SetBool("isGrounded", false);

        gliding = holdingSpacebar;
        anim.SetBool("isGliding", gliding);
    }

    // Subscribed to PlayerManager onDeathEvent
    private void OnDeath()
    {
        this.enabled = false;
        anim.SetBool("isGrounded", false);
        anim.SetBool("isGliding", false);
    }

    // Called by animation event in run animation when foot hits ground
    private void PlayFootstepSfx()
    {
        playerManager.PlayFootstepSfx();
    }
}
