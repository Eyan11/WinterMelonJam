using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TurtleController : MonoBehaviour
{
    // Settings for throw distance, speed, and what the hell the shell is
    [SerializeField] private float turtleNormalMoveSpeed;
    [SerializeField] private float turtleNoShellMoveSpeed;
    [SerializeField] private float shellThrowSpeed;
    [SerializeField] private float shellIncrement;
    [SerializeField] private GameObject shellTemplate;
    [SerializeField] private GameObject arrowTemplate;
    // Layer info
    private int solidMask;
    // Shell template info
    private float shellMaxDist;
    private Vector3 shellSize;
    // State management
    private bool destroyedShellEarly = false;
    private bool aiming = false;
    private GameObject shell;
    private GameObject arrow;
    private float shellThrowDirection = 1;
    // Player info
    private float moveInput;
    private PlayerManager playerManager;
    private Rigidbody2D body;
    private Animator anim;
    private SpriteRenderer spriteRenderer;


    // **********************************************
    // UNITY ACTIONS

    // Retrieves components on object activation
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = transform.parent.gameObject.GetComponent<PlayerManager>();

        solidMask = LayerMask.GetMask("Default", "Interactables", "Floor");

        shellMaxDist = Mathf.Sqrt(shellTemplate.GetComponent<Shell>().getMaxDistanceSquared());
        shellSize = shellTemplate.GetComponent<Collider2D>().bounds.size;
    }

    private void FixedUpdate()
    {
        if (CheckForShell()) 
        {
            body.linearVelocityX = moveInput * turtleNoShellMoveSpeed;
            anim.SetFloat("moveSpeed", 2.5f);
        }
        else
        {
            body.linearVelocityX = moveInput * turtleNormalMoveSpeed;
            anim.SetFloat("moveSpeed", 1f);
        }

        anim.SetBool("isMoving", Mathf.Abs(body.linearVelocity.x) > 0.01);

        if (aiming == true)
        {
            // Calculate shell throw direction
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            shellThrowDirection = (worldMousePos - transform.position).x;
            shellThrowDirection = Mathf.Sign(shellThrowDirection); // Normalizes

            Vector3 angle = Vector3.right * shellThrowDirection;
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, shellSize, 0, angle, shellMaxDist, solidMask);
            Vector3 baseShift = transform.position + Vector3.up * shellSize.y;
            Debug.Log("Hit: " + (hit == true));
            if (hit == false)
                arrow.transform.position = baseShift + Vector3.right * shellThrowDirection * shellMaxDist;
            else
                arrow.transform.position = baseShift + Vector3.right * shellThrowDirection * hit.distance;

            Debug.Log("Position: " + arrow.transform.position);
        }
    }

    // Called when entering this mask transformation
    public void OnEnable()
    {
        playerManager.onGroundedEvent += OnGrounded;
        playerManager.onUngroundedEvent += OnUngrounded;

        anim.SetBool("isGrounded", playerManager.IsGrounded);
        anim.SetBool("isMoving", false);
        anim.SetBool("isThrowing", false);
    }

    // Called when leaving this mask transformation
    public void OnDisable()
    {
        playerManager.onGroundedEvent -= OnGrounded;
        playerManager.onUngroundedEvent -= OnUngrounded;
    }

    private void OnGrounded()
    {
        anim.SetBool("isGrounded", true);
    }

    private void OnUngrounded()
    {
        anim.SetBool("isGrounded", false);
    }

    // **********************************************
    // HELPER FUNCTIONS

    private bool CheckForShell()
    {
        return (shell != null && shell.IsDestroyed() == false);
    }

    // **********************************************
    // EVENTS

    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false) return;

        moveInput = context.ReadValue<Vector2>().x;

        if (moveInput != 0 && aiming == false)
        {
            moveInput = Mathf.Sign(moveInput);
            spriteRenderer.flipX = !(moveInput > 0);
        }
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnJump(InputAction.CallbackContext context)
    {
        // Prevents ability unless button was released and there is no shell
        if (gameObject.activeInHierarchy == false) return;
        if (context.canceled == false && CheckForShell() == true)
        {
            Destroy(shell);
            shell = null;
            destroyedShellEarly = true;
            return;
        }
        // Only runs after the keystroke for destroying shell early
        else if (context.canceled == true) 
        {
            if (destroyedShellEarly == true)
            {
                destroyedShellEarly = false;
                return;
            }
            else
            {
                Destroy(arrow);
                arrow = null;
            }
        }
        else if (CheckForShell() == false && destroyedShellEarly == false)
        {
            aiming = true;
            if (arrow == null)
            {
                arrow = Instantiate(arrowTemplate, transform.position, Quaternion.LookRotation(Vector3.down));
                arrow.transform.rotation = Quaternion.Euler(180f, 0, 0);
            }

            return;
        }
        aiming = false;

        // Spawn and throw shell in throw direction
        shell = Instantiate(shellTemplate, transform.position, Quaternion.identity);
        Rigidbody2D shellBody = shell.GetComponent<Rigidbody2D>();
        shellBody.linearVelocityX = shellThrowDirection * shellThrowSpeed;
        destroyedShellEarly = false;
        anim.SetTrigger("throwShell");
    }
}
