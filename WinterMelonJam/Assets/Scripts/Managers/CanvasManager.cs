using UnityEngine;
using TMPro;  // Needed to reference level text component

public enum ControlsType { None, Default, Monkey, MonkeyThrow, MonkeyClimb, Rhino, Turtle, Flamingo, FlamingoFly}

public class CanvasManager : MonoBehaviour
{
    [SerializeField] private AudioClip clickSfx;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private TMP_Text controlsText;
    private AudioSource audioSource;
    private bool paused = false;

    public static CanvasManager Instance { get; private set; }

    private void Awake()
    {
        // Only allow one canvas manager to exist in the scene
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;

        audioSource = GetComponent<AudioSource>();
        levelText.text = GameManager.Instance.GetLevelNumber().ToString();
        SwitchControlsUI(ControlsType.Default);
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


    // Updates the UI that displays the controls for the current mask
    public void SwitchControlsUI(ControlsType newControls)
    {
        switch(newControls)
        {
            case ControlsType.None:
                controlsText.text = "";
                break;
            case ControlsType.Default:
                controlsText.text = "";
                break;
            case ControlsType.Monkey:
                controlsText.text = "Pickup Box - Left Mouse Button";
                break;
            case ControlsType.MonkeyThrow:
                controlsText.text = "Throw - Hold Space to Charge \nDrop - Left Mouse Button";
                break;
            case ControlsType.MonkeyClimb:
                controlsText.text = "Jump - Space";
                break;
            case ControlsType.Rhino:
                controlsText.text = "Charge - Space";
                break;
            case ControlsType.Turtle:
                controlsText.text = "Throw Shell - Hold Space to Aim";
                break;
            case ControlsType.Flamingo:
                controlsText.text = "Glide - Hold Space";
                break;
            case ControlsType.FlamingoFly:
                controlsText.text = "Glide - Hold Space \nUpdraft - Left Mouse Button";
                break;
        }
    }


}
