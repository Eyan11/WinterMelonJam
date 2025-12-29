using UnityEngine;

public class WaterDeathTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            if(GameManager.Instance != null)
                GameManager.Instance.RestartLevel();
            else
                Debug.LogError("Cannot restart level upon entering water since there is no GameManager in scene!!!");
        }
    }
}
