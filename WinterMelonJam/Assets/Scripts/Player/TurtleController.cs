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
    // Shell data (shell shock!)
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
    }

    // Called when entering this mask transformation
    public void OnMaskEnter() {}

    // Called when leaving this mask transformation
    public void OnMaskExit() {}

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
        if (moveInput != 0 && shell == null) shellThrowDirection = moveInput;
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnInteract(InputAction.CallbackContext context)
    {
        // Prevents ability unless button was initialized and there is no shell
        if (gameObject.activeInHierarchy == false) return;
        if (context.started == false) return;
        if (CheckForShell()) return;

        shell = Instantiate(shellTemplate);
        Rigidbody2D shellBody = shell.GetComponent<Rigidbody2D>();
        if (shellBody == null)
        {
            Destroy(shell);
            shell = null;
            Debug.LogError("ERROR: Shell is missing a rigidbody! Clearing shell!");
            return;
        }
        shell.transform.position = transform.position;
        shellBody.linearVelocity = new Vector2(shellThrowDirection * shellThrowSpeed, 0);
    }
}
