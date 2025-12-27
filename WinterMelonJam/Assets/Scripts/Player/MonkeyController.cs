using UnityEngine;
using UnityEngine.InputSystem;  // IMPORTANT: make sure you have this to work with input system
using System.Collections.Generic;   // Need to use List data structure

public class MonkeyController : MonoBehaviour
{
    [SerializeField] private float interactRadius = 0.5f;
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float throwSpeed= 1f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private float moveSpeed = 2f;
    private Rigidbody2D body;
    private float moveInput;



    private void Awake()
    {
        body = transform.parent.gameObject.GetComponent<Rigidbody2D>();
    }


    // Handles changes to rigidbody velocity
    private void Update()
    {
        body.linearVelocity = new Vector2(moveInput * moveSpeed, body.linearVelocity.y);
    }

    private void InteractWithRope(GameObject obj)
    {
        Debug.Log("InteractWithRope. Obj.name = " + obj.name);
    }

    private void InteractWithBox(GameObject obj)
    {
        Debug.Log("InteractWithBox. Obj.name = " + obj.name);
    }

    private void CheckForInteraction()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, interactableLayer);
        List<Collider2D> collList = new List<Collider2D>(hits);

        if (hits.Length <= 0)
            return;
        
        // Use closest point of obj instead of its center/pivot point
        collList.Sort((a, b) => {
            float distA = ((Vector2)transform.position - a.ClosestPoint(transform.position)).sqrMagnitude;
            float distB = ((Vector2)transform.position - b.ClosestPoint(transform.position)).sqrMagnitude;
            return distA.CompareTo(distB);
        });

        GameObject hitObj = null;
        foreach(Collider2D hit in collList)
        {
            hitObj = hit.transform.gameObject;

            if(hitObj.CompareTag("Rope"))
            {
                InteractWithRope(hitObj);
                return;
            }
            else if(hitObj.CompareTag("Breakable") || hitObj.CompareTag("Non-Breakable"))
            {
                InteractWithBox(hitObj);
                return;
            }
        }
    }




    // *** Events *************************************************************


    // Called by the InputAction component on the Move event.
    public void OnMove(InputAction.CallbackContext context)
    {
        if(!gameObject.activeInHierarchy)
            return;
        
        moveInput = context.ReadValue<Vector2>().x;
    }


    // Called by the InputAction component on the Interact event.
    public void OnInteract(InputAction.CallbackContext context)
    {
        if(context.started && gameObject.activeInHierarchy)
            CheckForInteraction();
    }


    // Called when entering this mask transformation
    public void OnMaskEnter()
    {
        
    }

    // Called when leaving this mask transformation
    public void OnMaskExit()
    {
        moveInput = 0f;
    }
}
