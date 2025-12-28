using UnityEngine;
using UnityEngine.InputSystem;      // IMPORTANT: make sure you have this to work with input system
using System.Collections.Generic;  // Need to use List data structure


public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float moveSpeed = 2f;
    private PlayerManager playerManager;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private float moveInput;

    [Header ("Climbing")]
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float exitRopeJumpSpeed = 3f;
    [SerializeField] private float groundCheckDist = 0.1f;
    private int floorMask;
    private int solidMask;
    private int interactableMask;
    private RaycastHit2D[] hits = new RaycastHit2D[20];
    [SerializeField] private bool isClimbing = false;
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
    [SerializeField] private bool isThrowing = false;
    private Camera cam;
    private Vector2 throwVector = Vector2.zero;
    private GameObject throwObj;
    private Rigidbody2D throwBody;




    private void Awake()
    {
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        coll = transform.parent.gameObject.GetComponent<CapsuleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        transform.parent.transform.position = new Vector3(transform.position.x, newHeight, transform.parent.transform.position.z);  // Using parent z so we can render player on top of everything
    
        bool isMovingOnRope = climbInput != 0 && newHeight != minClimbHeight && newHeight != maxClimbHeight;
        anim.SetBool("isMoving", isMovingOnRope);
    }

    // Horizontal movement and accounts for velocity when jumping off of a rope and prevents movement when throwing
    private void MovementUpdate()
    {
        if(isUsingJumpHorVel)
        {
            // If grounded, stop using the jump velocity
            if(playerManager.IsGrounded)
            {
                isUsingJumpHorVel = false;
                body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
            }
        }
        else
            body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);
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
        anim.SetBool("isClimbing", isClimbing);
        spriteRenderer.flipX = moveInput > 0;
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.zero;
        transform.parent.transform.position = new Vector3(coll.transform.position.x, coll.ClosestPoint(transform.position).y, transform.parent.transform.position.z);

        maxClimbHeight = coll.bounds.max.y;
        minClimbHeight = coll.bounds.min.y;
    }

    private void ExitRope()
    {
        isClimbing = false;
        anim.SetBool("isClimbing", isClimbing);
        spriteRenderer.flipX = !(moveInput > 0);
        isUsingJumpHorVel = true;
        body.gravityScale = 1f;

        body.linearVelocity = new Vector2(moveInput * moveSpeed, exitRopeJumpSpeed);
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
        Debug.Log("HERE!");
        if (ValidateThrowPosition(obj) == false) return;

        Debug.Log("StartThrowing. Obj.name = " + obj.name);
        isThrowing = true;
        anim.SetBool("isThrowing", isThrowing);
        body.linearVelocity = Vector2.zero;

        throwObj = obj;
        throwBody = throwObj.GetComponent<Rigidbody2D>();
        throwBody.gravityScale = 0f;

        LockThrowObject();
    }

    private void DropObject()
    {
        isThrowing = false;
        anim.SetBool("isThrowing", isThrowing);
        throwBody.gravityScale = 1f;
        throwBody = null;
        throwObj = null;
    }

    private void ThrowObject()
    {
        isThrowing = false;
        anim.SetBool("isThrowing", isThrowing);
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

    private List<Collider2D> GetSortedInteractables()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        List<Collider2D> collList = new List<Collider2D>(hits);

        if (hits.Length <= 0)
            return collList;

        // Use closest point of obj instead of its center/pivot point
        collList.Sort((a, b) => {
            float distA = ((Vector2)transform.position - a.ClosestPoint(transform.position)).sqrMagnitude;
            float distB = ((Vector2)transform.position - b.ClosestPoint(transform.position)).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        return collList;
    }

    private void CheckForRope()
    {
        List<Collider2D> collList = GetSortedInteractables();
        GameObject hitObj = null;
        foreach (Collider2D hit in collList)
        {
            hitObj = hit.transform.gameObject;

            if (hitObj.CompareTag("Rope"))
            {
                StartClimbingRope(hit);
                return;
            }
        }
    }

    private void CheckForThrowables()
    {
        List<Collider2D> collList = GetSortedInteractables();
        GameObject hitObj = null;
        foreach (Collider2D hit in collList)
        {
            hitObj = hit.transform.gameObject;

            if (hitObj.CompareTag("Non-Breakable"))
            {
                StartThrowing(hit.gameObject);
                return;
            }
        }
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
            isUsingJumpHorVel = false;
            if(isClimbing)
                spriteRenderer.flipX = moveInput > 0;
            else
                spriteRenderer.flipX = !(moveInput > 0);
        }

        if (climbInput != 0)
            climbInput = Mathf.Sign(climbInput);
    }

    // Called by the InputAction component on the Jump event.
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && gameObject.activeInHierarchy)
        {
            if (isClimbing)
                ExitRope();
            else if (isThrowing) return; // No throwing while climbing
            else CheckForRope();
        }
    }

    // Called by the InputAction component on the Interact event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started && gameObject.activeInHierarchy)
        {
            if (isClimbing) return; // No climbing while throwing
            else if (isThrowing)
                ThrowObject();
            else CheckForThrowables();
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

    // Called when entering this mask transformation
    public void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;

        anim.SetBool("isGrounded", playerManager.IsGrounded);   // Initialize grounded anim bool
        anim.SetBool("isClimbing", isClimbing);
        anim.SetBool("isThrowing", isThrowing);
    }

    // Called when leaving this mask transformation
    public void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;

        moveInput = 0f;
        isUsingJumpHorVel = false;

        if(isClimbing)
        {
            isClimbing = false;
            anim.SetBool("isClimbing", isClimbing);
            body.gravityScale = 1f;
        }
        else if(isThrowing)
        {
            DropObject();
        }
    }
}
