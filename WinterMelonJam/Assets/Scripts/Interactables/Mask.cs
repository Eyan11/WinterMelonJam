using UnityEngine;

public class Mask : MonoBehaviour
{
    // Allows you to choose which mask is being enabled.
    [SerializeField] private MaskStatus targetMask;
    [SerializeField] private bool setStatus = true;

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (targetMask == null)
        {
            Debug.LogError("ERROR: No mask set for " + gameObject.name);
            return;
        }

        if (interactor.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            targetMask.setStatus(setStatus);
        }
    }
}
