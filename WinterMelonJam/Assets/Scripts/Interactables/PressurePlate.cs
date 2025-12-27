using UnityEngine;


public class PressurePlate : MonoBehaviour
{
    [SerializeField] private RopeMovement ropeMove;
    private int plateState = 0;

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
            GetComponent<SpriteRenderer>().color = Color.blue; 
            ropeMove.LowerRope();
        }
    }

    private void OnTriggerExit2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Non-Breakable"))
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            ropeMove.RaiseRope(); 
        }
    }
}
