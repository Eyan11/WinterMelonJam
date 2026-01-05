using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class RhinoController : MonoBehaviour
{
    // Settings for charge cooldown and speed
    [SerializeField] private float rhinoMoveSpeed;
    [SerializeField] private float rhinoChargeSpeed;
    [SerializeField] private float stunDuration;
    [SerializeField] private float maxChargeDuration;
    [SerializeField] private float chargeCooldownDuration;
    [SerializeField] private AudioClip rhinoChargeSfx;
    [SerializeField] private AudioClip rhinoStunSfx;
    // Managers 
    MaskWheelManager maskWheelManager;
    private PlayerManager playerManager;
    // State management
    private bool charging = false;
    private bool stunned = false;
    private float chargeDirection = 1;
    private float chargeTimeLeft;
    private float stunTimeLeft;
    private float moveInput;
    private float nextChargeTick;
    private int solidMask;
    // Player info
    private Vector3 scale;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Animator vfxAnim;

    // **********************************************
    // UNITY ACTIONS

    // Retrieves components on object activation
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();

        scale = transform.parent.GetComponent<Collider2D>().bounds.size;
        solidMask = LayerMask.GetMask("Floor", "Default", "Interactable");
        maskWheelManager = FindFirstObjectByType<MaskWheelManager>();
        anim = GetComponent<Animator>();
        vfxAnim = transform.GetChild(0).gameObject.GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();
    }

    // Handles changes to rigidbody velocity
    private void FixedUpdate()
    {
        // If charging, lower charge timer and check for collisions
        if (charging)
        {
            chargeTimeLeft -= Time.fixedDeltaTime;
            if (chargeTimeLeft < 0)
            {
                DeactivateCharge();
                goto SpeedControl;
            }

            RaycastHit2D[] objs = Physics2D.BoxCastAll(transform.position,
                scale, 0, Vector2.right * chargeDirection, 1f, solidMask
            );

            foreach (RaycastHit2D obj in objs)
            {
                if (obj.transform.GetComponent<Rope>() != null) continue;

                if (obj.transform.CompareTag("Breakable") == true) Destroy(obj.transform.gameObject);
                else
                {
                    stunTimeLeft = stunDuration;
                    stunned = true;
                    anim.SetBool("isStunned", stunned);
                    playerManager.PlayOneShotSFX(rhinoStunSfx);
                    DeactivateCharge();
                }
            }
        }
        SpeedControl:
        // Move at charge speed if charging
        if (charging) body.linearVelocity = new Vector2(chargeDirection * rhinoChargeSpeed, body.linearVelocity.y);
        else if (stunned == false) body.linearVelocity = new Vector2(moveInput * rhinoMoveSpeed, body.linearVelocity.y);

        if (stunned == true)
        {
            stunTimeLeft -= Time.fixedDeltaTime;
            if (stunTimeLeft <= 0)
            {
                stunned = false;
                anim.SetBool("isStunned", stunned);
                maskWheelManager.LockWheel = false;

                if(moveInput != 0)  // If player is moving now, update sprite direction
                    spriteRenderer.flipX = !(moveInput > 0);
            }
        }

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);
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
        anim.SetBool("isCharging", false);
        anim.SetBool("isStunned", false);
    }


    // Called when entering this mask transformation
    public void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;
        playerManager.onDeathEvent += OnDeath;

        anim.SetBool("isGrounded", playerManager.IsGrounded);
        anim.SetBool("isMoving", false);
        anim.SetBool("isCharging", false);
        anim.SetFloat("moveSpeed", 1);
        anim.SetBool("isStunned", false);

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
    // HELPER FUNCTIONS

    private void DeactivateCharge()
    {
        nextChargeTick = Time.fixedTime + chargeCooldownDuration;
        charging = false;
        anim.SetBool("isCharging", charging);
        anim.SetFloat("moveSpeed", 1);
        if (stunned == false)
        {
            maskWheelManager.LockWheel = false;
            if(moveInput != 0)  // If player is moving now, update sprite direction
                spriteRenderer.flipX = !(moveInput > 0);
        }
        chargeDirection = moveInput;    // Update charge direction after charge
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

            if(!charging)
                chargeDirection = moveInput;

            if (PlayerManager.IsValidContext(gameObject) == false) return;

            if(!charging && !stunned)
                spriteRenderer.flipX = !(moveInput > 0);    // Don't flip sprite while charging or stunned
        }
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnJump(InputAction.CallbackContext context)
    {
        if (PlayerManager.IsValidContext(gameObject) == false) return;
        // Does ability only when the button is initialized and cooldown is off
        if (context.started == false) return;
        float currentTime = Time.fixedTime;
        if (nextChargeTick > currentTime || charging == true) return;

        chargeTimeLeft = maxChargeDuration;
        charging = true;
        anim.SetBool("isCharging", charging);
        anim.SetFloat("moveSpeed", 3);
        maskWheelManager.LockWheel = true;

        // Play rhino charge vfx and sfx
        vfxAnim.SetTrigger("startCharging");
        playerManager.PlayOneShotSFX(rhinoChargeSfx);
    }

    // Called by animation event in run animation when foot hits ground
    private void PlayFootstepSfx()
    {
        playerManager.PlayFootstepSfx();
    }
}
