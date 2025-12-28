using UnityEngine;

public class KeyCollection : MonoBehaviour
{
    public bool keyCollected;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    void Start()
    {
        keyCollected = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter2D(Collider2D interactor)
    {
        if (interactor.CompareTag("Player") || interactor.CompareTag("Shell"))
        {
            keyCollected = true;
            this.gameObject.SetActive(false);
        }
    }

}
