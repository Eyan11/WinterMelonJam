using UnityEngine;
using System;  // For Action reference (events)
using UnityEngine.InputSystem; // Needed for PlayerInput reference to disable input


public class PlayerManager : MonoBehaviour
{
    [Header ("Ground Check")]
    [SerializeField] private float floorAngle = 90f;
    [SerializeField] private float floorNormalAlpha = 1f;
    private bool oldIsGroundedState;
    public bool IsGrounded => body?.IsTouching(contactFilter) ?? false; // Lambda wizardry
    private ContactFilter2D contactFilter;

    [Header ("Jump")]
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    private float canJumpTimer = 0f;
    private float jumpInputTimer = 0f;
    private bool hasJumped = false;
    private bool isJumpAllowed = false;
    private float jumpSpeed = 0f;

    [Header ("Death")]
    [SerializeField] private Vector2 deathBaseImpulse = new Vector2(5f, 10f);
    [SerializeField] private Vector2 deathBaseRandomRange = new Vector2(2f, 3f);

    [Header ("SFX")]
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private AudioClip jumpSfx;
    [SerializeField] private AudioClip landSfx;
    private AudioSource audioSource;

    private Rigidbody2D body;
    private BoxCollider2D coll;

    //list of observers for events
    public event Action onGroundedEvent;
    public event Action onUngroundedEvent;
    public event Action onDeathEvent;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();

        contactFilter = new ContactFilter2D();
        contactFilter.SetLayerMask(LayerMask.GetMask("Floor", "Interactable"));
        contactFilter.SetNormalAngle(floorAngle - floorNormalAlpha, floorAngle + floorNormalAlpha);

        if (GameManager.Instance == null) Debug.Log("ERROR: No GameManager instance!");
    }

    private void Update()
    {
        CheckIsGrounded();
        if(isJumpAllowed)
            JumpCalculations();
    }

    // Plays a specific audio clip once without looping on the player
    public void PlayOneShotSFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Toggles jump on and off and updates jump speed since some masks are not allowed to jump
    public void SetJumpSettings(bool newIsJumpAllowed, float newJumpSpeed)
    {
        isJumpAllowed = newIsJumpAllowed;
        jumpSpeed = newJumpSpeed;
    }

    // Called by the animal controllers when a foot hits the ground in their respective run animations
    public void PlayFootstepSfx()
    {
        PlayOneShotSFX(footstepSfx);
    }

    // Applies impulse to player for mario like death. Returns true if successful
    public bool TriggerPlayerDeath()
    {
        if(coll.isTrigger) return false;    // If player is already dead, ignore
        
        TriggerOnDeathEvent();              // Tell animal controllers to deactivate themselves

        // Set both colliders to triggers
        coll.isTrigger = true;
        CapsuleCollider2D capsuleColl = GetComponent<CapsuleCollider2D>();
        if(capsuleColl != null)
            capsuleColl.isTrigger = true;
        
        // Disable input
        PlayerInput input = GetComponent<PlayerInput>();
        if(input != null)
            input.DeactivateInput();
        
        body.linearVelocity = Vector2.zero;
        
        // Apply a random impulse in the air upon death
        Vector2 deathImpulse = deathBaseImpulse;
        deathImpulse.x += UnityEngine.Random.Range(-deathBaseRandomRange.x, deathBaseRandomRange.x);
        deathImpulse.y += UnityEngine.Random.Range(-deathBaseRandomRange.y, deathBaseRandomRange.y);
        deathImpulse.x *= UnityEngine.Random.value < 0.5f ? -1f : 1f;       // Randomly flip horizontal direction
        body.AddForce(deathImpulse, ForceMode2D.Impulse);

        this.enabled = false;   // prevent landing sfx
        return true;
    }

    // Set IsGrounded and trigger event when value is changed
    private void CheckIsGrounded()
    {
        if(IsGrounded && !oldIsGroundedState)
            TriggerOnGroundedEvent();
        else if(!IsGrounded && oldIsGroundedState)
            TriggerOnUngroundedEvent();
    }

    /** Handles jump input and calling Jump() **/
    private void JumpCalculations() {
        canJumpTimer -= Time.deltaTime;
        jumpInputTimer -= Time.deltaTime;

        if(!hasJumped && jumpInputTimer > 0f && (canJumpTimer > 0f || IsGrounded))
            Jump();
    }

    /** Applies jump settings and jump force **/
    private void Jump() {
        hasJumped = true;
        canJumpTimer = -1f;
        jumpInputTimer = -1f;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
        PlayOneShotSFX(jumpSfx);
    }


    // *** Event Triggers *******************************************

    public void TriggerOnGroundedEvent() {
        oldIsGroundedState = true;
        hasJumped = false;
        PlayOneShotSFX(landSfx);

        if(onGroundedEvent != null)
            onGroundedEvent();
    }

    public void TriggerOnUngroundedEvent() {
        oldIsGroundedState = false;
        canJumpTimer = coyoteTime; 

        if(onUngroundedEvent != null)
            onUngroundedEvent();
    }

    public void TriggerOnDeathEvent() {
        if(onDeathEvent != null)
            onDeathEvent();
    }

    // Called when restart button is pressed from PlayerInput
    public void OnRestart(InputAction.CallbackContext context)
    {
        // Only run during the initial phase, otherwise it might call it a lot (helps prevent multiple calls)
        if (context.started != true) return;

        GameManager.Instance.RestartLevel();    // Make sure GameManager prefab is in your level for this to work
    }

    // Uses InputAction to get the jump input
    public void OnJump(InputAction.CallbackContext context)
    {
        if(isJumpAllowed == false) return;
        if (IsValidContext(gameObject) == false) return;
        
        if(context.started)
        {
            jumpInputTimer = jumpBufferTime;

            if(!hasJumped && (IsGrounded || canJumpTimer > 0f))
                Jump();
        }
    }

    // *** Static functions *******************************************

    public static bool IsValidContext(GameObject refObj)
    {
        if (Time.timeScale <= 0.01f) return false; // Paused
        if (refObj.activeInHierarchy == false) return false; // Deactivated

        return true;
    }
}
