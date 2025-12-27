using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    
    private Rigidbody2D body;
    private float moveInput;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }


    // Handles changes to rigidbody velocity
    private void FixedUpdate()
    {
        body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
    }


    // Handles movement input. Called by the InputAction component on the Move event.
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
    }


}
