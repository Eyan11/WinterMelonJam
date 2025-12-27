using UnityEngine;


public class PressurePlate : MonoBehaviour
{
    [SerializeField] private RopeMovement ropeMove;
    [SerializeField] private DoorUnlock door;
    public bool plateState;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*void Start()
    {
        
    }
    */

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable"))
        {
            plateState = true;
            GetComponent<SpriteRenderer>().color = Color.blue; 
            if (ropeMove == null)
            {
                door.OpenDoor();
                return;
            }
            else
            {
                ropeMove.LowerRope();
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable"))
        {
            plateState = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            if (ropeMove == null)
            {
                door.CloseDoor();
                return;
            }
            else
            {
                ropeMove.RaiseRope();
            }
        }
    }
}
