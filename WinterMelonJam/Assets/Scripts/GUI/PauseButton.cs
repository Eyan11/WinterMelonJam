using UnityEngine;

public class PauseButton : MonoBehaviour
{
    private bool paused = false;

    public void TogglePause()
    {
        paused = !paused;
        if (paused == true) Time.timeScale = 0f;
        else Time.timeScale = 1f;

        FindAnyObjectByType<GameMenu>(FindObjectsInactive.Include).TogglePauseMenu(paused);
    }
}
