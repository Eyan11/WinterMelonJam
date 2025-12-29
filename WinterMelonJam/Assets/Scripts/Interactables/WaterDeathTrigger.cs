using UnityEngine;

public class WaterDeathTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.CompareTag("Player"))
        {
            // Apply death impulse and deactivate input
            PlayerManager playerManager = coll.GetComponent<PlayerManager>();
            if(playerManager != null)
            {
                if(!playerManager.TriggerPlayerDeath()) // If death is already happening, ignore
                    return;
            }

            // Play water die sfx
            AudioSource source = GetComponent<AudioSource>();
            if(source != null && source.clip != null)
                source.Play();

            Invoke("RestartLevel", 2.2f);
        }
    }


    private void RestartLevel()
    {
        if(GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
        else
            Debug.LogError("Cannot restart level upon entering water since there is no GameManager in scene!!!");
    }
}
