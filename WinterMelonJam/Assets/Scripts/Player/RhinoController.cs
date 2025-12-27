using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class RhinoController : MonoBehaviour
{
    // Settings for charge cooldown and speed
    [SerializeField] private float rhinoMoveSpeed;
    [SerializeField] private float rhinoChargeSpeed;
    [SerializeField] private float maxChargeDuration;
    [SerializeField] private float chargeCooldownDuration;
    private bool charging = false;
    private float chargeDirection = 1;
    private float chargeTimeLeft;
    private Rigidbody2D body;
    private float moveInput;
    private float nextChargeTick;

    // **********************************************
    // UNITY ACTIONS

    // Retrieves components on object activation
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
    }

    // Handles changes to rigidbody velocity
    private void FixedUpdate()
    {
        // If charging, then use the direction that the player is facing
        if (chargeTimeLeft > 0)
        {
            chargeTimeLeft -= Time.fixedDeltaTime;
            if (chargeTimeLeft < 0)
            {
                DeactivateCharge();
                return;
            }
           // Physics2D[] objs = Physics2D.BoxCastAll(transform.position, transform.size);
        }
        // Move at charge speed if charging
        if (charging) body.linearVelocity = new Vector2(chargeDirection * rhinoChargeSpeed, body.linearVelocity.y);
        else body.linearVelocity = new Vector2(moveInput * rhinoMoveSpeed, body.linearVelocity.y);
    }

    // **********************************************
    // HELPER FUNCTIONS

    private void DeactivateCharge()
    {
        nextChargeTick = Time.fixedTime + chargeCooldownDuration;
        charging = false;
    }

    // **********************************************
    // EVENTS

    // Uses InputAction to get the movement direction
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>().x;
        if (moveInput != 0 && charging == false) chargeDirection = moveInput;
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnInteract(InputAction.CallbackContext context)
    {
        float currentTime = Time.fixedTime;
        if (nextChargeTick > currentTime || charging == true) return;

        chargeTimeLeft = maxChargeDuration;
        charging = true;
    }
}
