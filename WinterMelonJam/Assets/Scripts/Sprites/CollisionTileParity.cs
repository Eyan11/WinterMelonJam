using UnityEngine;

[ExecuteAlways]
public class CollisionTileParity : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool activeInRuntime = false;
    [SerializeField] private Vector3 transformScaleLock = Vector3.one;
    //[SerializeField] private SpriteRenderer[] secondarySprites;
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
    }
}
