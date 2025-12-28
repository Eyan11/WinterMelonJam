using UnityEngine;

public class OneShotVFX : MonoBehaviour
{
    private Animator anim;
    private AudioSource audioSource;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    

    // Plays a specific animation and audio clip
    public void PlayVFX(string animStateName, AudioClip clip)
    {
        anim.Play(animStateName, 0, 0f);

        if (clip != null)
            audioSource.PlayOneShot(clip);
    }

    // Called by animation event, destroys gameobject
    public void OnAnimationFinished()
    {
        Destroy(gameObject);
    }


}
