using UnityEngine;

public class PlatformTrigger : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void platOn()
    {
        this.gameObject.SetActive(true);
    }
    public void platOff()
    {
        this.gameObject.SetActive(false);
    }
}
