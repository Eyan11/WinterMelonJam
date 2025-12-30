using UnityEngine;
using System;  // For Action reference (events)
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;
using System.Drawing; // Needed for PlayerInput reference to disable input

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float groundCheckDist = 0.1f;
    [SerializeField] private AudioClip landSfx;
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private Vector2 deathBaseImpulse = new Vector2(5f, 10f);
    [SerializeField] private Vector2 deathBaseRandomRange = new Vector2(2f, 3f);
    [SerializeField] private float floorAngle = 90f;
    [SerializeField] private float floorNormalAlpha = 1f;

    private AudioSource audioSource;
    private Rigidbody2D body;
    private BoxCollider2D coll;
    private ContactFilter2D contactFilter;
    private bool oldIsGroundedState;
    public bool IsGrounded => body?.IsTouching(contactFilter) ?? false; // Lambda wizardry

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
    }

    // Plays a specific audio clip once without looping on the player
    public void PlayOneShotSFX(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
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


    // *** Event Triggers *******************************************

    public void TriggerOnGroundedEvent() {
        oldIsGroundedState = true;
        if(onGroundedEvent != null)
            onGroundedEvent();
        
        PlayOneShotSFX(landSfx);
    }

    public void TriggerOnUngroundedEvent() {
        oldIsGroundedState = false;
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

    // *** Static functions *******************************************

    public static bool IsValidContext(GameObject refObj)
    {
        if (Time.timeScale <= 0.01f) return false; // Paused
        if (refObj.activeInHierarchy == false) return false; // Deactivated

        return true;
    }
}
