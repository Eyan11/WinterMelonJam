using UnityEngine;

public class DoorUnlock : MonoBehaviour
{
    
    [SerializeField] private KeyCollection doorKey;
    private GameMenu gameMenu;

    private void Start()
    {
        gameMenu = FindFirstObjectByType<GameMenu>(FindObjectsInactive.Include);
        if (gameMenu == null)
            Debug.LogError("ERROR: No game menu set!");
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        // Complete level by unlocking a door with a key
        if (coll.CompareTag("Player") && doorKey.keyCollected==true)
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
            gameMenu.EnableGameMenu();
        }
        else
            Debug.LogError("Cannot complete level because GameManager is not in the current scene!!!");
    }


}
