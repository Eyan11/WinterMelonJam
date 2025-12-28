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
    [SerializeField] private GameObject shellTemplate;
    // State management
    private bool destroyedShellEarly = false;
    private GameObject shell;
    private float shellThrowDirection = 1;
    // Player info
    private float moveInput;
    private Rigidbody2D body;

    // **********************************************
    // UNITY ACTIONS

    // Retrieves components on object activation
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (CheckForShell()) body.linearVelocityX = moveInput * turtleNoShellMoveSpeed;
        else body.linearVelocityX = moveInput * turtleNormalMoveSpeed;

        Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        shellThrowDirection = (worldMousePos - transform.position).x;
        shellThrowDirection = shellThrowDirection / Mathf.Abs(shellThrowDirection); // Normalizes
    }

    // Called when entering this mask transformation
    public void OnEnable() {}

    // Called when leaving this mask transformation
    public void OnDisable() {}

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
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnJump(InputAction.CallbackContext context)
    {
        // Prevents ability unless button was released and there is no shell
        if (gameObject.activeInHierarchy == false) return;
        if (context.canceled == false)
        {
            if (CheckForShell() == false) return;
            Destroy(shell);
            shell = null;
            destroyedShellEarly = true;
            return;
        }
        // Only runs after the keystroke for destroying shell early
        else if (context.canceled == true && destroyedShellEarly == true) 
        {
            destroyedShellEarly = false;
            return;
        }

        shell = Instantiate(shellTemplate, transform.position, Quaternion.identity);
        Rigidbody2D shellBody = shell.GetComponent<Rigidbody2D>();
        shellBody.linearVelocityX = shellThrowDirection * shellThrowSpeed;
        destroyedShellEarly = false;
    }
}
