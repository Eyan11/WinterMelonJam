using UnityEngine;


public class PressurePlate : MonoBehaviour
{
    [SerializeField] private RopeMovement ropeMove;
    [SerializeField] private DoorUnlock door;
    [SerializeField] private PlatformTrigger platf;
    public bool plateState;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        plateState = false;
    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable") || interactor.CompareTag("Shell"))
        {
            plateState = true;
            GetComponent<SpriteRenderer>().color = Color.blue; 
            if (ropeMove != null)
            {
                ropeMove.LowerRope();
            }
            else if (door != null)
            {
                door.OpenDoor();
            }
            else if (platf != null)
            {
                platf.platOn();
            }
           
        }
    }

    private void OnTriggerExit2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable") || interactor.CompareTag("Shell"))
        {
            plateState = false;
            GetComponent<SpriteRenderer>().color = Color.red;
            if (ropeMove != null)
            {
                ropeMove.RaiseRope();
            }
            else if (door != null)
            {
                door.CloseDoor();
            }
            else if (platf != null)
            {
                platf.platOff();
            }
        }
    }
}
