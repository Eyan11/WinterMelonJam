using UnityEngine;
using TMPro;  // Needed to reference level text component

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private TMP_Text levelText;
    private AudioSource audioSource;
    private bool paused = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        levelText.text = GameManager.Instance.GetLevelNumber().ToString();
    }


    // Plays a click sound. Called when selecting a mask or clicking on UI buttons.
    public void PlayClickSFX()
    {
        if (clickSfx != null)
            audioSource.PlayOneShot(clickSfx);
    }


    public void TogglePause()
    {
        PlayClickSFX();
        paused = !paused;
        if (paused == true) Time.timeScale = 0f;
        else Time.timeScale = 1f;

        FindAnyObjectByType<GameMenu>(FindObjectsInactive.Include).TogglePauseMenu(paused);
    }


    public void RestartLevel()
    {
        if(paused)
            TogglePause();  // Unpause game when restarting
        else
            PlayClickSFX();
        
        GameManager.Instance.RestartLevel();
    }
}
