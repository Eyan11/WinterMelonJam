using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class FlamingoController : MonoBehaviour
{
    // Flamingo speeds, mid-air jump power settings
    [SerializeField] private float flamingoMoveSpeed;
    [SerializeField] private float updraftPower;
    // LayerMask
    private int floorMask;
    private int interactableMask;
    private int groundedMask;
    // State management
    private bool onGround;
    private bool gliding;
    private bool holdingSpacebar;
    private bool usedUpdraft;
    private bool updraftDeactivated;
    private float moveInput;
    private RaycastHit2D[] hits = new RaycastHit2D[20];
    // Player info
    private Rigidbody2D body;
    private Vector3 scale;

    // **********************************************
    // UNITY ACTIONS

    // Called when object is first activated
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();

        floorMask = LayerMask.NameToLayer("Floor");
        interactableMask = LayerMask.NameToLayer("Interactable");
        groundedMask = LayerMask.GetMask("Floor", "Interactable");

        scale = transform.localScale;
    }

    // Flamingo movement
    private void FixedUpdate()
    {
        GroundCheck();

        body.linearVelocityX = moveInput * flamingoMoveSpeed;
        if (gliding)
        {
            if (updraftDeactivated == true) body.linearVelocityY = -0.2f;
            else if (body.linearVelocityY < -0.2f)
            {
                updraftDeactivated = true;
            }
        }
    }

    // Called when entering this mask transformation
    public void OnEnable() {}

    // Called when leaving this mask transformation
    public void OnDisable() {}

    // **********************************************
    // HELPER FUNCTIONS

    private void SetOnGround()
    {
        onGround = true;
        if (gliding)
        {
            gliding = false;
            usedUpdraft = false;
            updraftDeactivated = true;
        }
    }

    private void GroundCheck()
    {
        if (body.linearVelocity.y > 0.1)
            return;

        int objsOnGround = Physics2D.BoxCastAll(transform.position + Vector3.down * scale.y,
            new Vector3(scale.x - 0.1f, 0.1f), 0, Vector2.zero, 0.1f, groundedMask
        ).Length;

        if (objsOnGround > 0)
        {
            SetOnGround();
            return;
        }
        onGround = false;
        gliding = holdingSpacebar;
    }

    // **********************************************
    // EVENTS

    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false) return;
        moveInput = context.ReadValue<Vector2>().x;
    }

    // Uses InputAction to enable updraft ability once per glide
    public void OnInteract(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false) return;
        if (context.started == false || gliding == false || usedUpdraft == true) return;

        usedUpdraft = true;
        updraftDeactivated = false;

        body.linearVelocityY = updraftPower;
    }

    // Uses InputAction to allow for gliding ability
    public void OnJump(InputAction.CallbackContext context) 
    {
        if (gameObject.activeInHierarchy == false) return;

        if (context.canceled == false)
        {
            holdingSpacebar = true;
        }
        else
        {
            holdingSpacebar = false;
        }
    }
}
