using UnityEngine;

public class Rope : MonoBehaviour
{
    [SerializeField] private bool horizontalMode;
    public bool ropeEnabled = true;
    private bool lastRopeState = true;
    private float minRopeBound;
    public float MinRopeBound { get { return minRopeBound; } }
    private float maxRopeBound;
    public float MaxRopeBound { get { return maxRopeBound; } }
    private Collider2D collider;

    private void Awake()
    {
        collider = gameObject.GetComponent<Collider2D>();
        if (horizontalMode == false) // Vertical
        {
            minRopeBound = collider.bounds.min.y;
            maxRopeBound = collider.bounds.max.y;
        }
        else
        {
            minRopeBound = collider.bounds.min.x;
            maxRopeBound = collider.bounds.max.x;
        }
    }

    private void Update()
    {
        if (lastRopeState != ropeEnabled)
        {
            lastRopeState = ropeEnabled;
            ToggleRope();
        }
    }

    private void ToggleRope()
    {
        // TEMP: this feature is unfinished, but the rope would essentially unfurl
    }

    public bool IsHorizontalMode() { return horizontalMode; }
}
