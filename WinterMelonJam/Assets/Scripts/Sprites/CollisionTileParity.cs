using UnityEngine;

[ExecuteAlways]
public class CollisionTileParity : MonoBehaviour
{
    [System.Serializable]
    private struct SecondarySpriteScaling
    {
        public SpriteRenderer secondarySprite;
        public Vector2 relativeSize;
        public bool lockXSize;
        public bool lockYSize;
    }

    [Header("Settings")]
    [SerializeField] private bool activeInRuntime = false;
    [SerializeField] private Vector3 transformScaleLock = Vector3.one;
    [SerializeField] private SecondarySpriteScaling[] secondarySprites;
    private BoxCollider2D boxCollider;
    private SpriteRenderer baseSprite;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        baseSprite = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    ///  For editor only (unless activeInRuntime is true): locks the transform to a 
    ///  specific scale to prevent accidental edits. Then, it rescales the box collider
    ///  to be the same size as the base sprite. 
    /// </summary>
    private void Update()
    {
        if (Application.IsPlaying(gameObject) && !activeInRuntime) return; 

        if (transform.localScale != transformScaleLock) transform.localScale = transformScaleLock;

        boxCollider.size = baseSprite.size;
        boxCollider.offset = Vector2.zero;
        foreach (var sprite in secondarySprites)
        {
            Vector2 newSize = baseSprite.size;
            if (sprite.lockXSize) newSize.x = sprite.secondarySprite.size.x;
            if (sprite.lockYSize) newSize.y = sprite.secondarySprite.size.y;
            sprite.secondarySprite.size = newSize;
        }
    }
}
