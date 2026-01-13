using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    [SerializeField] private DoorUnlock doorUnlock;
    
    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Shell") || interactor.CompareTag("Throwable"))
        {
            this.gameObject.SetActive(false);
            doorUnlock.UnlockDoor();
        }
    }

}
