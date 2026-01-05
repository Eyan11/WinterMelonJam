using UnityEngine;

public class PlayClickSfx : MonoBehaviour
{
    [SerializeField] private AudioClip clickSfx;
    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Plays a click sound. Called when selecting a mask or clicking on UI buttons.
    public void PlayClickSFX()
    {
        if (clickSfx != null)
            audioSource.PlayOneShot(clickSfx);
    }

}
