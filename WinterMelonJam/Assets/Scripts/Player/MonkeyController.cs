using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float interactRadius;
    [SerializeField] private float climbSpeed;
    [SerializeField] private float throwSpeed;
    [SerializeField] private LayerMask interactableLayer;
    private float climbInput;
    



    // Handles climbing input. Called by the InputAction component on the Move event.
    public void OnMove(InputAction.CallbackContext context)
    {
        climbInput = context.ReadValue<Vector2>().y;
    }

    // Handles interact input. Called by the InputAction component on the Inter event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, 0f, Vector2.zero, interactRadius, interactableLayer);

        if (hits.Length <= 0)
            return;
        
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider == null)
                return;
            
            // TODO -left off here - I think hits is already sorted by closest hit object though
        }
    }
}
