using UnityEngine;

public class MaskUpdate : MonoBehaviour
{
    // Allows you to choose which mask is being enabled.
    [SerializeField] private MaskWheelEnable enableMask; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player"))
        {
            this.gameObject.SetActive(false);
            enableMask.setTrue();
        }
    }
}
