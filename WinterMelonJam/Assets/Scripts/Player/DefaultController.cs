using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class DefaultController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1f;
    private Rigidbody2D body;
    private float moveInput;
    private float moveDir = 1;

    [Header ("Jump")]
    [SerializeField] private float jumpSpeed = 10f;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.1f;
    [SerializeField] private float groundCheckDist = 0.1f;
    private int floorMask;
    private int interactableMask;
    private CapsuleCollider2D coll;
    private float canJumpTimer;
    private float jumpInputTimer = 0f;
    private RaycastHit2D[] hits = new RaycastHit2D[20];



    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
        coll = transform.parent.gameObject.GetComponent<CapsuleCollider2D>();
        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
    }


    private void Update()
    {
        body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
        GroundCheck();
        JumpCalculations();
    }



    private void GroundCheck()
    {
        if(body.linearVelocity.y > 0.1)
            return;
        
        int numHits = coll.Cast(Vector2.down, hits, groundCheckDist);

        for(int i = 0; i < numHits; i++)
        {
            if(hits[i].transform.gameObject.layer == floorMask)
            {
                canJumpTimer = coyoteTime; 
                return;
            }
            else if(hits[i].transform.gameObject.layer == interactableMask)
            {
                if(hits[i].transform.gameObject.CompareTag("Breakable") || hits[i].transform.gameObject.CompareTag("Non-Breakable"))
                {
                    canJumpTimer = coyoteTime;
                    return;
                }
            }
        }
    }

    /** Handles jump input and calling Jump() **/
    private void JumpCalculations() {
        canJumpTimer -= Time.deltaTime;
        jumpInputTimer -= Time.deltaTime;

        if(jumpInputTimer > 0f && canJumpTimer > 0f)
            Jump();
    }

    /** Applies jump settings and jump force **/
    private void Jump() {
        canJumpTimer = -1f;
        jumpInputTimer = -1f;
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpSpeed);
    }




    // *** Events *************************************************************


    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        moveInput = context.ReadValue<Vector2>().x;

        if (moveInput != 0) 
            moveInput = Mathf.Sign(moveInput);
            moveDir = moveInput;
    }

    // Uses InputAction to get the jump input
    public void OnJump(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        if(context.started)
        {
            jumpInputTimer = jumpBufferTime;

            if(canJumpTimer > 0f)
                Jump();
        }
    }

    // Called when entering this mask transformation
    public void OnMaskEnter()
    {
        
    }

    // Called when leaving this mask transformation
    public void OnMaskExit()
    {
        jumpInputTimer = -1f;
        canJumpTimer = -1f;
        moveInput = 0f;
    }
}
