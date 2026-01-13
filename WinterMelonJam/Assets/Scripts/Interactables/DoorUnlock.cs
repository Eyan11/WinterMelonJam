using System.Dynamic;
using UnityEngine;

public class DoorUnlock : PuzzleBase
{
    private Animator anim;
    private GameMenu gameMenu;
    private bool isUnlocked = false;

    private void Start()
    {
        gameMenu = FindFirstObjectByType<GameMenu>(FindObjectsInactive.Include);
        if (gameMenu == null)
            Debug.LogError("ERROR: No game menu set!");
        
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        // Complete level by unlocking a door with a key
        if (coll.CompareTag("Player") && isUnlocked)
            CompleteLevel();
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

    public void UnlockDoor()
    {
        isUnlocked = true;
        anim.SetTrigger("unlockDoor");
    }


}
