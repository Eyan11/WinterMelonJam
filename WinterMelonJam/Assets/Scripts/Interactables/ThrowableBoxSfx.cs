using UnityEngine;

public class ThrowableBoxSfx : MonoBehaviour
{
    [SerializeField] private AudioClip impactSound;
    [SerializeField] private float minImpactVelocity = 2f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only play if impact is strong enough
        if (collision.relativeVelocity.magnitude < minImpactVelocity)
            return;

        audioSource.PlayOneShot(impactSound);
    }
}
