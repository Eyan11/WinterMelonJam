using UnityEngine;

public class PlatformTrigger : PuzzleBase
{
    [SerializeField] bool initialState = false;

    private void Start()
    {
        this.gameObject.SetActive(initialState);
    }

    public override void OnActivate()
    {
        this.gameObject.SetActive(!initialState);
    }

    public override void OnDeactivate()
    {
        this .gameObject.SetActive(initialState);
    }
}
