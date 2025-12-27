using UnityEngine;
using UnityEngine.InputSystem;

public class FlamingoController : MonoBehaviour
{
    // Flamingo speeds, mid-air jump power settings
    [SerializeField] private float flamingoWalkSpeed;
    [SerializeField] private float updraftPower;
    // State management
    private bool onGround;
    private bool usedUpdraft;
    private float moveInput;
    // Player info
    private Rigidbody2D body;

    // **********************************************
    // UNITY ACTIONS

    // Called when object is first activated
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }

    // Called when entering this mask transformation
    public void OnMaskEnter() {}

    // Called when leaving this mask transformation
    public void OnMaskExit() {}

    // **********************************************
    // HELPER FUNCTIONS

    private bool ValidateInput(InputAction.CallbackContext context)
    {
        if (gameObject.activeInHierarchy == false) return false;
        if (context.started == false) return false;

        return true;
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
        if (ValidateInput(context) == false) return; 
    }

    // Uses InputAction to allow for gliding ability
    public void OnJump(InputAction.CallbackContext context) 
    {
        if (ValidateInput(context) == false) return;
    }
}
