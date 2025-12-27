using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system

public class RhinoController : MonoBehaviour
{
    // Settings for charge cooldown and speed
    [SerializeField] private float rhinoMoveSpeed;
    [SerializeField] private float rhinoChargeSpeed;
    [SerializeField] private float stunDuration;
    [SerializeField] private float maxChargeDuration;
    [SerializeField] private float chargeCooldownDuration;
    // State management
    private bool charging = false;
    private bool stunned = false;
    private float chargeDirection = 1;
    private float chargeTimeLeft;
    private float stunTimeLeft;
    private float moveInput;
    private float nextChargeTick;
    private int solidMask;
    // Player info
    private Vector3 scale;
    private Rigidbody2D body;

    // **********************************************
    // UNITY ACTIONS

    // Retrieves components on object activation
    private void Awake()
    {
        body = transform.parent.GetComponent<Rigidbody2D>();
        scale = transform.localScale;
        solidMask = LayerMask.GetMask("Floor", "Default", "Interactable");
    }

    // Handles changes to rigidbody velocity
    private void FixedUpdate()
    {
        // If charging, lower charge timer and check for collisions
        if (charging)
        {
            chargeTimeLeft -= Time.fixedDeltaTime;
            if (chargeTimeLeft < 0)
            {
                DeactivateCharge();
                goto SpeedControl;
            }

            float angle = chargeDirection == 1 ? 0 : 180;
            RaycastHit2D[] objs = Physics2D.BoxCastAll(transform.position + Vector3.right * scale.x * chargeDirection / 2,
                scale - Vector3.down * 0.1f, 0, Vector2.zero, 0.01f, solidMask
            );

            foreach (RaycastHit2D obj in objs)
            {
                if (obj.transform.CompareTag("Breakable") == true) Destroy(obj.transform.gameObject);
                else
                {
                    DeactivateCharge();
                    stunTimeLeft = stunDuration;
                    stunned = true;
                    goto SpeedControl;
                }
            }
        }
        SpeedControl:
        // Move at charge speed if charging
        if (charging) body.linearVelocity = new Vector2(chargeDirection * rhinoChargeSpeed, body.linearVelocity.y);
        else if (stunned == false) body.linearVelocity = new Vector2(moveInput * rhinoMoveSpeed, body.linearVelocity.y);

        if (stunned == true)
        {
            stunTimeLeft -= Time.fixedDeltaTime;
            if (stunTimeLeft <= 0) stunned = false;
        }
    }

    // Called when entering this mask transformation
    public void OnMaskEnter() {}

    // Called when leaving this mask transformation
    public void OnMaskExit() {}

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
        if (gameObject.activeInHierarchy == false) return;
        moveInput = context.ReadValue<Vector2>().x;
        if (moveInput != 0 && charging == false) chargeDirection = moveInput;
    }

    // Uses InputAction to track when the interaction key is used; when used, try to charge
    public void OnInteract(InputAction.CallbackContext context)
    {
        // Does ability only when the button is initialized and cooldown is off
        if (gameObject.activeInHierarchy == false) return;
        if (context.started == false) return;
        float currentTime = Time.fixedTime;
        if (nextChargeTick > currentTime || charging == true) return;

        chargeTimeLeft = maxChargeDuration;
        charging = true;
    }
}
