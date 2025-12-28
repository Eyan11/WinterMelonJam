using UnityEngine;
using UnityEngine.UI;

public class MaskStatus : MonoBehaviour
{
    [SerializeField] private bool maskEnabled = true;
    public MaskType maskType = MaskType.None;
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
        transparentOriginalColor = sliceSelector.color;
        transparentOriginalColor.a = 0.4f;
    }

    private void updateButton()
    {
        if (sliceSelector == null) ObtainData();

        if (maskEnabled)
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

    public bool getStatus() { return maskEnabled; }
}
