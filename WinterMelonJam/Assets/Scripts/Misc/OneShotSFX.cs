using UnityEngine;

public class OneShotSFX : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip clip;

    public void PlayOneShotSFX()
    {
        if(source != null && clip != null)
            source.PlayOneShot(clip);
    }
}
