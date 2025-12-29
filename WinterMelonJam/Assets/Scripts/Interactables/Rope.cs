using UnityEngine;

public class Rope : PuzzleBase
{
    [SerializeField] private bool horizontalMode;
    [SerializeField] private bool ropeInitiallyEnabled = true;
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

        gameObject.SetActive(ropeInitiallyEnabled);
    }

    public override void OnActivate()
    {
        gameObject.SetActive(!ropeInitiallyEnabled);
    }

    public override void OnDeactivate()
    {
        gameObject.SetActive(ropeInitiallyEnabled);
    }

    public bool IsHorizontalMode() { return horizontalMode; }
}
