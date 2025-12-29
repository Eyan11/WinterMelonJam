using UnityEngine;
using System;  // For Action reference (events)
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices; // Needed for PlayerInput reference to disable input

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float groundCheckDist = 0.1f;
    [SerializeField] private AudioClip landSfx;
    [SerializeField] private AudioClip footstepSfx;
    [SerializeField] private Vector2 deathBaseImpulse = new Vector2(5f, 10f);
    [SerializeField] private Vector2 deathBaseRandomRange = new Vector2(2f, 3f);

    private AudioSource audioSource;
    private Rigidbody2D body;
    private BoxCollider2D coll;
    public bool IsGrounded {get; private set;}  // Public getter
    private int floorMask;
    private int interactableMask;
    private RaycastHit2D[] hits = new RaycastHit2D[20];

    //list of observers for events
    public event Action onGroundedEvent;
    public event Action onUngroundedEvent;
    public event Action onDeathEvent;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        audioSource = GetComponent<AudioSource>();
        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
    }

    private void Update()
    {
        GroundCheck();
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
        Debug.Log("death impulse: " + deathImpulse);
        body.AddForce(deathImpulse, ForceMode2D.Impulse);

        this.enabled = false;   // prevent landing sfx
        return true;
    }


    // Determine if player is grounded and set the grounded variable
    private void GroundCheck()
    {
        if(body.linearVelocity.y > 0.1)
        {
            SetIsGrounded(false);
            return;
        }
        
        int numHits = coll.Cast(Vector2.down, hits, groundCheckDist);

        for(int i = 0; i < numHits; i++)
        {
            if(hits[i].transform.gameObject.layer == floorMask)
            {
                SetIsGrounded(true);
                return;
            }
            else if(hits[i].transform.gameObject.layer == interactableMask)
            {
                if(hits[i].transform.gameObject.CompareTag("Breakable") || hits[i].transform.gameObject.CompareTag("Non-Breakable"))
                {
                    SetIsGrounded(true);
                    return;
                }
            }
        }
        SetIsGrounded(false);
    }


    // Set IsGrounded and trigger event when value is changed
    private void SetIsGrounded(bool newBoolVal)
    {
        if(IsGrounded && newBoolVal == false)
            TriggerOnUngroundedEvent();
        else if(!IsGrounded && newBoolVal == true)
            TriggerOnGroundedEvent();
        
        IsGrounded = newBoolVal;
    }


    // *** Event Triggers *******************************************

    public void TriggerOnGroundedEvent() {
        if(onGroundedEvent != null)
            onGroundedEvent();
        
        PlayOneShotSFX(landSfx);
    }

    public void TriggerOnUngroundedEvent() {
        if(onUngroundedEvent != null)
            onUngroundedEvent();
    }

    public void TriggerOnDeathEvent() {
        if(onDeathEvent != null)
            onDeathEvent();
    }

    // Called when restart button is pressed from PlayerInput
    public void OnRestart()
    {
        GameManager.Instance.RestartLevel();    // Make sure GameManager prefab is in your level for this to work
    }

}
