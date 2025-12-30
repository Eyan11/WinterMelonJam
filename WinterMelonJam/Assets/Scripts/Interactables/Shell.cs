using UnityEngine;

public class Shell : MonoBehaviour
{
    // Shell max lifetime
    [SerializeField] private float maxLifetime;
    [SerializeField] private float maxDistanceSquared;
    // Shell data
    private BoxCollider2D collider;
    private Rigidbody2D body;
    private bool platformMode = false;
    public float lifetime;
    private Vector3 shellStartPos;
    private ContactFilter2D contactFilter;
    private bool isTouching => body.IsTouching(contactFilter);

    // Sets the life time to max
    private void Awake()
    {
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.GetMask("Default", "Interactable", "Floor");

        collider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();
        lifetime = maxLifetime;
        shellStartPos = transform.position;
    }

    // FixedUpdate is called once per frame to check on platform validity and when to break
    private void FixedUpdate()
    {
        if (platformMode)
        {
            lifetime -= Time.fixedDeltaTime;
            if (lifetime < 0) BreakShell();
        }
        else if ((shellStartPos - transform.position).sqrMagnitude >= maxDistanceSquared || isTouching)
        {
            body.linearVelocity = Vector3.zero;
            body.bodyType = RigidbodyType2D.Static;
            collider.excludeLayers = 0;
            platformMode = true;
        }
    }

    // Plays shell breaking animation
    public void BreakShell()
    {
        // TODO: josue do the shell break jig dance thing i'm sleepy
        Destroy(gameObject);
    }

    public float getMaxDistanceSquared() { return maxDistanceSquared; }
}
