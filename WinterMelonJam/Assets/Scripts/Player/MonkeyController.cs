using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float interactRadius;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float throwSpeed;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float moveSpeed;
    private Rigidbody2D body;
    private float climbInput;
    private float moveInput;



    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
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
        Debug.Log("Moving:" + moveInput);
    }


    // Handles interact input. Called by the InputAction component on the Inter event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0f, Vector2.zero, interactRadius, interactableLayer);

        if (hits.Length <= 0)
            return;
        
        foreach(RaycastHit2D hit in hits)
        {
            
        }

    }


    // Called when entering this mask transformation
    public void OnMaskEnter()
    {
        
    }

    // Called when leaving this mask transformation
    public void OnMaskExit()
    {
        
    }
}
