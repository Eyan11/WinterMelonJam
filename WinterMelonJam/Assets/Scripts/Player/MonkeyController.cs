using UnityEngine;
using UnityEngine.InputSystem;      // IMPORTANT: make sure you have this to work with input system
using System.Collections.Generic;   // For List data structure


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
    [SerializeField] private float exitRopeJumpVertSpeed = 3f;
    [SerializeField] private float exitRopeJumpHorSpeed = 3f;
    [SerializeField] private AudioClip exitRopeJumpSfx;
    private GameObject ropeObj;
    private Rope ropeComp;
    private int solidMask;
    [SerializeField] private bool isClimbing = false;
    private float climbInput;
    private bool isUsingJumpHorVel = false;

    [Header ("Throwing")]
    [SerializeField] private float throwObjHeightOffset = 1f;
    [SerializeField] private float minThrowSpeed = 5f;
    [SerializeField] private float maxThrowSpeed = 50f;
    [SerializeField] private float chargeIncrement = 500f;
    [SerializeField] private AudioClip throwSfx;
    [SerializeField] private float minArrowDist = 20f;
    [SerializeField] private float maxArrowDist = 100f;
    private Transform arrowBaseTran;
    private Transform arrowVisualsTran;
    private bool isThrowing = false;
    private bool isCharging = false;
    private int excludePlayerMask;
    private Camera cam;
    private Vector2 throwVector = Vector2.zero;
    private float chargePower = 0f;    
    private GameObject throwObj;
    private Rigidbody2D throwBody;
    private Collider2D throwCollider;


    private void Awake()
    {
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        arrowBaseTran = transform.GetChild(0);
        arrowVisualsTran = arrowBaseTran.GetChild(0);
        cam = Camera.main;
        solidMask = LayerMask.GetMask("Floor", "Default");
        excludePlayerMask = ~LayerMask.GetMask("Player");
    }


    // Handles movement update
    private void Update()
    {
        LockThrowObject();

        if(isClimbing)
            ClimbingUpdate();
        else
            MovementUpdate();
        
        if(isThrowing)
            ThrowingUpdate();
    }

    private Vector3 CalculateThrowWithOffsetVector()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, throwCollider.bounds.size - new Vector3(throwCollider.bounds.size.x * 0.4f, 0, 0), 0, Vector3.up, throwObjHeightOffset, solidMask);
        float height = throwObjHeightOffset;
        if (hit == true) height = hit.distance;

        return transform.position + (Vector3.up * height);
    }

    private void LockThrowObject()
    {
        if (throwObj == null) return;

        throwObj.transform.position = CalculateThrowWithOffsetVector();
        throwBody.transform.rotation = Quaternion.identity;
        throwBody.linearVelocity = Vector2.zero;
        throwBody.angularVelocity = 0;
    }

    private Vector3 CalculateClimbingPosition()
    {
        if (ropeComp.IsHorizontalMode() == false) // Vertical
        {
            float newHeight = transform.parent.transform.position.y + (climbInput * climbSpeed * Time.deltaTime);
            newHeight = Mathf.Clamp(newHeight, ropeComp.MinRopeBound, ropeComp.MaxRopeBound);
            return new Vector3(
                ropeObj.transform.position.x, newHeight, ropeObj.transform.position.z
            );
        }
        else
        {
            float newX = transform.parent.transform.position.x + (climbInput * climbSpeed * Time.deltaTime);
            newX = Mathf.Clamp(newX, ropeComp.MinRopeBound, ropeComp.MaxRopeBound);
            return new Vector3(
                newX, ropeObj.transform.position.y, ropeObj.transform.position.z
            );
        }
    }

    // Moves monkey vertically when climbing
    private void ClimbingUpdate()
    {
        if (body.linearVelocity != Vector2.zero) ExitRope();
        
        Vector3 oldVector3 = transform.parent.transform.position;
        transform.parent.transform.position = CalculateClimbingPosition();

        bool isMovingOnRope = climbInput != 0 && (oldVector3 - transform.parent.transform.position).sqrMagnitude != 0f;
        anim.SetBool("isMoving", isMovingOnRope);
    }

    // Horizontal movement and accounts for velocity when jumping off of a rope and prevents movement when throwing
    private void MovementUpdate()
    {
        if(isUsingJumpHorVel)
            body.linearVelocity = new Vector2(moveInput * exitRopeJumpHorSpeed, body.linearVelocity.y);
        else
            body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);
    }

    private void ThrowingUpdate()
    {
        if (isCharging == false)
        {            
            Vector3 mouseScreenPosition = Mouse.current.position.ReadValue();
            Vector3 mouseWorldPosition = cam.ScreenToWorldPoint(mouseScreenPosition);
            throwVector = new Vector2(mouseWorldPosition.x - transform.position.x, mouseWorldPosition.y - transform.position.y);

            float angle = Mathf.Atan2(throwVector.y, throwVector.x) * Mathf.Rad2Deg;
            arrowBaseTran.rotation = Quaternion.Euler(0f, 0f, angle);
        }
        else if (chargePower < maxThrowSpeed)
        {
            chargePower += chargeIncrement * Time.deltaTime;
            chargePower = Mathf.Clamp(chargePower, minThrowSpeed, maxThrowSpeed);

            float chargePercent = (chargePower - minThrowSpeed) / (maxThrowSpeed - minThrowSpeed);
            float arrowIncrement = minArrowDist + (chargePercent * (maxArrowDist - minArrowDist));
            arrowVisualsTran.localPosition = new Vector3(arrowIncrement, 0f, 0f);
        }
    }

    private void StartClimbingRope(Collider2D coll)
    {
        isClimbing = true;
        anim.SetBool("isClimbing", isClimbing);
        ropeObj = coll.gameObject;
        ropeComp = ropeObj.GetComponent<Rope>();
        spriteRenderer.flipX = moveInput > 0;
        body.gravityScale = 0f;
        body.linearVelocity = Vector2.zero;
        climbInput = 0;
        transform.parent.transform.position = CalculateClimbingPosition();
    }

    private void UnsetRope()
    {
        isClimbing = false;
        ropeObj = null;
        ropeComp = null;
        anim.SetBool("isClimbing", isClimbing);
        body.gravityScale = 1f;
    }

    private void ExitRope()
    {
        UnsetRope();
        anim.SetBool("isClimbing", isClimbing);
        playerManager.PlayOneShotSFX(exitRopeJumpSfx);
        spriteRenderer.flipX = !(moveInput > 0);
        isUsingJumpHorVel = true;

        body.linearVelocity = new Vector2(moveInput * exitRopeJumpHorSpeed, exitRopeJumpVertSpeed);
    }

    //private bool ValidateThrowPosition(GameObject obj)
    //{
    //    if (obj == null)
    //    {
    //        Debug.LogError("ERROR: attempting to throw a null object!");
    //        return false;
    //    }
    //    Vector3 lookPosition = transform.position - obj.transform.position;

    //    // Checks if theres a clear path above the monkey for the box (clear line of sight)
    //    BoxCollider2D collider = obj.transform.GetComponent<BoxCollider2D>();
    //    RaycastHit2D hit = Physics2D.BoxCast(transform.position, collider.bounds.size - new Vector3(collider.bounds.size.x * 0.4f, 0, 0), 0, Vector3.up, throwObjHeightOffset, solidMask);
    //    if (hit == true) return false;

    //    return true;
    //}

    private void StartThrowing(GameObject obj)
    {
        //if (ValidateThrowPosition(obj) == false) return;

        isThrowing = true;
        chargePower = minThrowSpeed;
        anim.SetBool("isThrowing", isThrowing);
        arrowBaseTran.gameObject.SetActive(true);
        arrowVisualsTran.localPosition = new Vector3(minArrowDist, 0f, 0f);
        body.linearVelocity = Vector2.zero;

        throwObj = obj;
        throwBody = throwObj.GetComponent<Rigidbody2D>();
        throwBody.gravityScale = 0f;
        throwCollider = throwObj.GetComponent<Collider2D>();
        throwCollider.forceSendLayers = excludePlayerMask;

        LockThrowObject();
    }

    private void DropObject()
    {
        isThrowing = false;
        isCharging = false;
        anim.SetBool("isThrowing", isThrowing);
        arrowBaseTran.gameObject.SetActive(false);
        throwBody.gravityScale = 1f;
        throwCollider.forceSendLayers = Physics2D.AllLayers;
        ObjectClipping throwClip = throwObj.AddComponent<ObjectClipping>();
        throwClip.playerObj = transform.parent.gameObject;
        throwCollider = null;
        throwBody = null;
        throwObj = null;
    }

    private void ThrowObject()
    {
        isThrowing = false;
        isCharging = false;
        anim.SetBool("isThrowing", isThrowing);
        arrowBaseTran.gameObject.SetActive(false);
        playerManager.PlayOneShotSFX(throwSfx);
        throwVector.Normalize();
        throwVector *= chargePower;

        throwBody.gravityScale = 1f;
        throwBody.AddForce(throwVector, ForceMode2D.Impulse);
        throwCollider.forceSendLayers = Physics2D.AllLayers;
        throwCollider = null;
        throwBody = null;
        throwObj = null;
        Debug.Log("Final Throw Speed: " + throwVector);
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
        moveInput = context.ReadValue<Vector2>().x;

        if (moveInput != 0)
        {
            moveInput = Mathf.Sign(moveInput);

            if(!gameObject.activeInHierarchy) return;

            if(isClimbing)
                spriteRenderer.flipX = moveInput > 0;
            else
                spriteRenderer.flipX = !(moveInput > 0);
        }

        if(!gameObject.activeInHierarchy) return;

        if (ropeComp != null)
        {
            if (ropeComp.IsHorizontalMode() == false) // Vertical
                climbInput = context.ReadValue<Vector2>().y;
            else climbInput = moveInput;
        }
        else climbInput = 0;

        if (climbInput != 0)
            climbInput = Mathf.Sign(climbInput);
    }

    // Called by the InputAction component on the Jump event.
    public void OnJump(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false) return;
        if (context.started)
        {
            if (isClimbing)
                ExitRope();
            else if (isThrowing) return; // No throwing while climbing
            else CheckForRope();
        }
        else if (context.canceled && isThrowing == true) // Throwing
        {
            ThrowObject();
        }
        else if (isThrowing && isCharging == false)
        {
            isCharging = true;
            chargePower = minThrowSpeed;
        }
    }

    // Called by the InputAction component on the Interact event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started && gameObject.activeInHierarchy)
        {
            if (isClimbing) return; // No climbing while throwing
            else if (isThrowing) return; // Throwing is handled with the jump button
            else CheckForThrowables();
        }
    }

    private void OnGrounded()
    {
        isUsingJumpHorVel = false;
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
        anim.SetBool("isClimbing", false);
        anim.SetBool("isThrowing", false);
        arrowBaseTran.gameObject.SetActive(false);
    }

    // Called when entering this mask transformation
    public void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;
        playerManager.onDeathEvent += OnDeath;

        anim.SetBool("isGrounded", playerManager.IsGrounded);
        anim.SetBool("isClimbing", false);
        anim.SetBool("isThrowing", false);
        arrowBaseTran.gameObject.SetActive(false);

        if(moveInput != 0)
            spriteRenderer.flipX = !(moveInput > 0);
    }

    // Called when leaving this mask transformation
    public void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;
        playerManager.onDeathEvent -= OnDeath;

        isUsingJumpHorVel = false;

        if(isClimbing)
        {
            UnsetRope();
        }
        else if(isThrowing)
        {
            DropObject();
        }
    }

    // Called by animation event in run animation when foot hits ground
    private void PlayFootstepSfx()
    {
        playerManager.PlayFootstepSfx();
    }
}
