using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    //[SerializeField] private RopeMovement ropeMove;
    //[SerializeField] private DoorUnlock door;
    //[SerializeField] private PlatformTrigger platf;
    [SerializeField] private PuzzleBase[] puzzleBases;
    public bool plateState = false;
    private Animator anim;
    

    void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }
    
    

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable") || interactor.CompareTag("Shell"))
        {
            plateState = true;
            anim.SetBool("isPressed", plateState);
            //if (ropeMove != null)
            //{
            //    ropeMove.LowerRope();
            //}
            //else if (door != null)
            //{
            //    door.OpenDoor();
            //}
            //else if (platf != null)
            //{
            //    platf.platOn();
            //}
            foreach (var p in puzzleBases)
            {
                Debug.Log("attempting activate");
                p.OnActivate();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable") || interactor.CompareTag("Shell"))
        {
            plateState = false;
            anim.SetBool("isPressed", plateState);
            //if (ropeMove != null)
            //{
            //    ropeMove.RaiseRope();
            //}
            //else if (door != null)
            //{
            //    door.CloseDoor();
            //}
            //else if (platf != null)
            //{
            //    platf.platOff();
            //}
            foreach (var p in puzzleBases)
            {
                Debug.Log("attempting deactivate");
                p.OnDeactivate();
            }
        }
    }
}
