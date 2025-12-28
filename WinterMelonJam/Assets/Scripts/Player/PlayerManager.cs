using UnityEngine;
using System;    // For Action reference (events)

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float groundCheckDist = 0.1f;
    private Rigidbody2D body;
    private BoxCollider2D coll;
    public bool IsGrounded {get; private set;}  // Public getter
    private int floorMask;
    private int interactableMask;
    private RaycastHit2D[] hits = new RaycastHit2D[20];

    //list of observers for events
    public event Action onGroundedEvent;
    public event Action onUngroundedEvent;


    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<BoxCollider2D>();
        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
    }

    private void Update()
    {
        GroundCheck();
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
        {
            TriggerOnUngroundedEvent();
        }
        else if(!IsGrounded && newBoolVal == true)
        {
            TriggerOnGroundedEvent();
        }
        
        IsGrounded = newBoolVal;
    }


    // *** Event Triggers *******************************************

    public void TriggerOnGroundedEvent() {
        if(onGroundedEvent != null)
            onGroundedEvent();
    }

    public void TriggerOnUngroundedEvent() {
        if(onUngroundedEvent != null)
            onUngroundedEvent();
    }

}
