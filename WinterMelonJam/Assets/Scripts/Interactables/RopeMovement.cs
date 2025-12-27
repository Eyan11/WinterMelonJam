using UnityEngine;

public class RopeMovement : MonoBehaviour
{
    public Vector3 sizeIncrement = new Vector3(0, 0.1f, 0);
    private bool lower = false;
    private bool raise = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*
    void Start()
    {
        
    }
    */
    // Update is called once per frame
    void Update()
    {
        
    }
    

    public void LowerRope()
    {
        lower = true;
        raise = false;
    }
    public void RaiseRope()
    {
        lower = false;
        raise = true;
    }
}
