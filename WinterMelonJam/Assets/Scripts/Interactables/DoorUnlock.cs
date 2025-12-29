using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    
    [SerializeField] private KeyCollection doorKey;
    
    private void OnTriggerEnter2D(Collider2D interactor)
    {
        // Complete level by unlocking a door with a key
        if (doorKey.keyCollected==true)
            CompleteLevel();
    }

    public void OpenDoor()
    {
        this.gameObject.SetActive(false);
    }
    public void CloseDoor()
    {
        this.gameObject.SetActive(true);
    }

    private void CompleteLevel()
    {
        this.gameObject.SetActive(false);
        if(GameManager.Instance != null)
        {
            GameManager.Instance.CompleteLevel();
        }
        else
            Debug.LogError("Cannot complete level because GameManager is not in the current scene!!!");
        

    }


}
