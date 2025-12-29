using UnityEditor.SceneManagement;
using UnityEngine;

public class ObjectClipping : MonoBehaviour
{
    [SerializeField] private float delayActivation = 1f;
    public GameObject playerObj;
    private Collider2D playerCollider;
    private Collider2D objCollider;
    private int excludePlayerMask;
    private float minDistSquared;

    // Gets the necessary data to detect when the player is far away enough to not be crushed
    void ObtainData()
    {
        excludePlayerMask = ~LayerMask.GetMask("Player");
        playerCollider = playerObj.GetComponent<Collider2D>();
        objCollider = GetComponent<Collider2D>();
        float maxPlayerSize = Mathf.Max(playerCollider.bounds.size.x, playerCollider.bounds.size.y) / 2;
        float maxObjSize = Mathf.Max(objCollider.bounds.size.x, objCollider.bounds.size.y) / 2;
        minDistSquared = Mathf.Pow(maxPlayerSize + maxObjSize, 2);

        objCollider.forceSendLayers = excludePlayerMask;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (playerObj != null && playerCollider == null)
        {
            ObtainData();
            return;
        }

        if (delayActivation > 0f)
        {
            delayActivation -= Time.deltaTime;
            return;
        }
        if ((playerObj.transform.position-transform.position).sqrMagnitude > minDistSquared)
        {
            objCollider.forceSendLayers = Physics2D.AllLayers;
            Destroy(this);
        }
    }
}
