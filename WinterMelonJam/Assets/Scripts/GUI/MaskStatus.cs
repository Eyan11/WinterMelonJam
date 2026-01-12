using UnityEngine;
using UnityEngine.UI;

public class MaskStatus : MonoBehaviour
{
    [SerializeField] private bool maskEnabled = true;
    public MaskType maskType = MaskType.None;
    private bool managerLock = false;
    private bool maskSelected = false;
    private Image sliceSelector;
    private GameObject icon;
    private RectTransform iconTransform;
    private Vector2 originalIconSize;
    private Color originalColor;
    private Color transparentOriginalColor;
    private CanvasManager canvasManager;

    private void Awake()
    {
        canvasManager = this.transform.parent.transform.parent.GetComponent<CanvasManager>();
        ObtainData();
        updateButton();
    }

    private void ObtainData()
    {
        icon = transform.GetChild(0).gameObject;
        iconTransform = icon.GetComponent<RectTransform>();
        originalIconSize = iconTransform.rect.size;
        sliceSelector = GetComponent<Image>();
        originalColor = sliceSelector.color;
        originalColor.a = 1; // Just incase its grabbing data while its off
        transparentOriginalColor = sliceSelector.color;
        transparentOriginalColor.a = 0.1f;
    }

    private void updateButton()
    {
        if (sliceSelector == null) ObtainData();

        if (maskEnabled && managerLock == false)
        {
            icon.SetActive(true);
            if (maskSelected == true)
            {
                if(canvasManager != null)
                    canvasManager.PlayClickSFX();
                sliceSelector.color = originalColor;
                iconTransform.sizeDelta = originalIconSize * 1.5f;
            }
            else
            {
                sliceSelector.color = transparentOriginalColor;
                iconTransform.sizeDelta = originalIconSize;
            }
        }
        else
        {
            sliceSelector.color = new Color(0, 0, 0, 10);
            icon.SetActive (false);
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
