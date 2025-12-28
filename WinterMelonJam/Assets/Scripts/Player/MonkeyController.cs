using UnityEngine;
using UnityEngine.InputSystem;      // IMPORTANT: make sure you have this to work with input system
using System.Collections.Generic;   // Need to use List data structure


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
    private int solidMask;
    private int interactableMask;
    private RaycastHit2D[] hits = new RaycastHit2D[20];
    private bool isClimbing = false;
    private float climbInput;
    private float maxClimbHeight;
    private float minClimbHeight;
    private bool isUsingJumpHorVel = false;
    private CapsuleCollider2D coll;

    [Header ("Throwing")]
    [SerializeField] private float throwSpeedMult = 0.5f;
    [SerializeField] private float throwObjHeightOffset = 1f;
    [SerializeField] private float minThrowSpeed = 5f;
    [SerializeField] private float maxThrowSpeed = 50f;
    private bool isThrowing = false;
    private Camera cam;
    private Vector2 throwVector = Vector2.zero;
    private GameObject throwObj;
    private Rigidbody2D throwBody;




    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        coll = transform.parent.gameObject.GetComponent<CapsuleCollider2D>();
        cam = Camera.main;
        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
        solidMask = LayerMask.GetMask("Floor", "Default");
    }


    // Handles movement update
    private void Update()
    {
        LockThrowObject();

        if(isClimbing)
            ClimbingUpdate();
        else if(isThrowing)
            ThrowingUpdate();
        else
            MovementUpdate();
    }

    private Vector3 CalculateThrowWithOffsetVector()
    {
        return transform.position + (Vector3.up * throwObjHeightOffset);
    }

    private void LockThrowObject()
    {
        if (throwObj == null) return;
        bool testState = ValidateThrowPosition(throwObj);
        Debug.Log("TESTING: " + testState);
        if (testState == false)
        {
            DropObject();
            return;
        }

        throwObj.transform.position = CalculateThrowWithOffsetVector();
        throwBody.transform.rotation = Quaternion.identity;
        throwBody.linearVelocity = Vector2.zero;
        throwBody.angularVelocity = 0;

    }

    // Moves monkey vertically when climbing
    private void ClimbingUpdate()
    {
        float newHeight = transform.position.y + (climbInput * climbSpeed * Time.deltaTime);
        newHeight = Mathf.Clamp(newHeight, minClimbHeight, maxClimbHeight);
        transform.parent.transform.position = new Vector3(transform.position.x, newHeight, transform.position.z);
    }

    // Horizontal movement and accounts for velocity when jumping off of a rope and prevents movement when throwing
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

    private void ThrowingUpdate()
    {
        Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();

        Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
        throwVector = new Vector2(mouseWorldPosition.x - transform.position.x, mouseWorldPosition.y - transform.position.y);

        MovementUpdate();
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

    private bool ValidateThrowPosition(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("ERROR: attempting to throw a null object!");
            return false;
        }
        Vector3 lookPosition = transform.position - obj.transform.position;

        // Checks if theres a clear path above the monkey for the box (clear line of sight)
        BoxCollider2D collider = obj.transform.GetComponent<BoxCollider2D>();
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, collider.bounds.size, 0, Vector3.up, throwObjHeightOffset, solidMask);
        if (hit == true) return false;

        return true;
    }

    private void StartThrowing(GameObject obj)
    {
        if (ValidateThrowPosition(obj) == false) return;

        Debug.Log("StartThrowing. Obj.name = " + obj.name);
        isThrowing = true;
        body.linearVelocity = Vector2.zero;

        throwObj = obj;
        throwBody = throwObj.GetComponent<Rigidbody2D>();
        throwBody.gravityScale = 0f;

        LockThrowObject();
    }

    private void DropObject()
    {
        isThrowing = false;
        throwBody.gravityScale = 1f;
        throwBody = null;
        throwObj = null;
    }

    private void ThrowObject()
    {
        isThrowing = false;
        throwVector *= throwSpeedMult;
        float throwMagnitude = throwVector.magnitude;
        Debug.Log("magnitude before update: " + throwMagnitude);

        if (throwMagnitude > maxThrowSpeed)
        {
            throwVector.Normalize();
            throwVector *= maxThrowSpeed;
            Debug.Log("Min Throw Speed");
        }
        else if(throwMagnitude < minThrowSpeed)
        {
            throwVector.Normalize();
            throwVector *= minThrowSpeed;
            Debug.Log("Max Throw Speed");
        }

        throwBody.gravityScale = 1f;
        throwBody.AddForce(throwVector, ForceMode2D.Impulse);
        throwBody = null;
        throwObj = null;
        Debug.Log("Throwing Object at speed: " + throwVector);
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
            else if(hitObj.CompareTag("Non-Breakable"))
            {
                StartThrowing(hitObj);
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
            else if(isThrowing)
                ThrowObject();
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

        if(isClimbing)
        {
            isClimbing = false;
            body.gravityScale = 1f;
        }
        else if(isThrowing)
        {
            DropObject();
        }
    }
}
