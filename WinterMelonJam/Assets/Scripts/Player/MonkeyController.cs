using UnityEngine;
using UnityEngine.InputSystem;      // IMPORTANT: make sure you have this to work with input system
using System.Collections.Generic; // Need to use List data structure


public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D body;
    private float moveInput;
    private float moveDir = 1f;

    [Header ("Climbing")]
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float exitRopeJumpSpeed = 3f;
    [SerializeField] private float groundCheckDist = 0.1f;
    private int floorMask;
    private int interactableMask;
    private RaycastHit2D[] hits = new RaycastHit2D[20];
    private bool isClimbing = false;
    private float climbInput;
    private float maxClimbHeight;
    private float minClimbHeight;
    private bool isUsingJumpHorVel = false;
    private CapsuleCollider2D coll;

    [Header ("Throwing")]
    [SerializeField] private float throwSpeed = 1f;



    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        coll = transform.parent.gameObject.GetComponent<CapsuleCollider2D>();
        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
    }


    // Handles movement update
    private void Update()
    {
        if(isClimbing)
            ClimbingUpdate();
        else
            MovementUpdate();
    }

    // Moves monkey vertically when climbing
    private void ClimbingUpdate()
    {
        float newHeight = transform.position.y + (climbInput * climbSpeed * Time.deltaTime);
        newHeight = Mathf.Clamp(newHeight, minClimbHeight, maxClimbHeight);
        transform.parent.transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }

    // Horizontal movement and accounts for velocity when jumping off of a rope
    private void MovementUpdate()
    {
        if(isUsingJumpHorVel)
        {
            // If grounded, stop using the jump velocity
            if(IsGrounded())
            {
                isUsingJumpHorVel = false;
                body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
            }
        }
        else
            body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
    }

    private void StartClimbingRope(Collider2D coll)
    {
        isClimbing = true;
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.zero;
        transform.parent.transform.position = new Vector2(coll.transform.position.x, coll.ClosestPoint(transform.position).y);

        maxClimbHeight = coll.bounds.max.y;
        minClimbHeight = coll.bounds.min.y;
    }

    private void ExitRope()
    {
        isClimbing = false;
        isUsingJumpHorVel = true;
        body.gravityScale = 1f;

        body.linearVelocity = new Vector2(moveDir * moveSpeed, exitRopeJumpSpeed);
    }

    private void InteractWithBox(GameObject obj)
    {
        Debug.Log("InteractWithBox. Obj.name = " + obj.name);
    }

    private void CheckForInteraction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        List<Collider2D> collList = new List<Collider2D>(hits);

        if (hits.Length <= 0)
            return;
        
        // Use closest point of obj instead of its center/pivot point
        collList.Sort((a, b) => {
            float distA = ((Vector2)transform.position - a.ClosestPoint(transform.position)).sqrMagnitude;
            float distB = ((Vector2)transform.position - b.ClosestPoint(transform.position)).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        GameObject hitObj = null;
        foreach(Collider2D hit in collList)
        {
            hitObj = hit.transform.gameObject;

            if(hitObj.CompareTag("Rope"))
            {
                StartClimbingRope(hit);
                return;
            }
            else if(hitObj.CompareTag("Breakable") || hitObj.CompareTag("Non-Breakable"))
            {
                InteractWithBox(hitObj);
                return;
            }
        }
    }

    // Returns true if monkey is on an object with floor layer, or interactable layer with breakable/non-breakable tag
    private bool IsGrounded()
    {
        if(body.linearVelocity.y > 0.1)
            return false;
        
        int numHits = coll.Cast(Vector2.down, hits, groundCheckDist);

        for(int i = 0; i < numHits; i++)
        {
            if(hits[i].transform.gameObject.layer == floorMask)
            {
                return true;
            }
            else if(hits[i].transform.gameObject.layer == interactableMask)
            {
                if(hits[i].transform.gameObject.CompareTag("Breakable") || hits[i].transform.gameObject.CompareTag("Non-Breakable"))
                {
                    return true;
                }
            }
        }
        return false;
    }




    // *** Events *************************************************************


    // Called by the InputAction component on the Move event.
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;

        moveInput = context.ReadValue<Vector2>().x;
        climbInput = context.ReadValue<Vector2>().y;

        if (moveInput != 0)
        {
            moveInput = Mathf.Sign(moveInput);
            moveDir = moveInput;
            isUsingJumpHorVel = false;
        }

        if (climbInput != 0)
            climbInput = Mathf.Sign(climbInput);
    }


    // Called by the InputAction component on the Interact event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started && gameObject.activeInHierarchy)
        {
            if(isClimbing)
                ExitRope();
            else
                CheckForInteraction();
        }
    }


    // Called when entering this mask transformation
    public void OnMaskEnter()
    {
        
    }

    // Called when leaving this mask transformation
    public void OnMaskExit()
    {
        moveInput = 0f;
        isUsingJumpHorVel = false;
    }
}
