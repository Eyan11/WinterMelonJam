using UnityEngine;
using UnityEngine.UI;

public class MaskStatus : MonoBehaviour
{
    [SerializeField] private bool maskEnabled = true;
    public MaskType maskType = MaskType.None;
    private bool managerLock = false;
    private bool maskSelected = false;
    private Image sliceSelector;
    private Color originalColor;
    private Color transparentOriginalColor;

    private void Awake()
    {
        ObtainData();
        updateButton();
    }

    private void ObtainData()
    {
        sliceSelector = GetComponent<Image>();
        originalColor = sliceSelector.color;
        originalColor.a = 1; // Just incase its grabbing data while its off
        transparentOriginalColor = sliceSelector.color;
        transparentOriginalColor.a = 0.4f;
    }

    private void updateButton()
    {
        if (sliceSelector == null) ObtainData();

        if (maskEnabled && managerLock == false)
        {
            if (maskSelected == true) sliceSelector.color = originalColor;
            else sliceSelector.color = transparentOriginalColor;
        }
        else
        {
            sliceSelector.color = new Color(0, 0, 0, 20);
        }
    }

    public void setSelected(bool selected)
    {
        maskSelected = selected;
        updateButton();
    }

    public void setStatus(bool enabled)
    {
        maskEnabled = enabled;
        updateButton();
    }

    public void setIndividualManagerLock(bool lockStatus)
    {
        managerLock = lockStatus;
        updateButton();
    }

    public bool getStatus() { return maskEnabled; }

    public static void setManagerLock(bool lockStatus)
    {
        foreach (MaskStatus markStatus in FindObjectsByType<MaskStatus>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            markStatus.setIndividualManagerLock(lockStatus);
        }
    }
}
