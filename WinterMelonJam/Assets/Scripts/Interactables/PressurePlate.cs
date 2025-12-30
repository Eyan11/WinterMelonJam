using UnityEngine;
using System.Collections.Generic;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] private PuzzleBase[] puzzleBases;
    public bool plateState = false;
    private Animator anim;
    

    private void Awake()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }
    
    private bool IsAValidInteraction(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") ||
            interactor.CompareTag("Non-Breakable") ||
            interactor.CompareTag("Shell") ||
            interactor.CompareTag("Throwable")) return true;

        return false;
    }

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (IsAValidInteraction(interactor))
        {
            plateState = true;
            anim.SetBool("isPressed", plateState);

            foreach (var p in puzzleBases)
            {
                p.OnActivate();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D interactor)
    {
        if (IsAValidInteraction(interactor))
        {
            plateState = false;
            anim.SetBool("isPressed", plateState);

            foreach (var p in puzzleBases)
            {
                p.OnDeactivate();
            }
        }
    }
}
