using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum MaskType { None, Default, Monkey, Rhino, Turtle, Flamingo }

public class MaskWheelManager : MonoBehaviour
{
    [SerializeField] private GameObject wheelObj;
    [SerializeField] private MaskManager maskManager;

    private bool lockWheel = false;
    public bool LockWheel { get { return lockWheel; } set { ToggleLock(value); } }

    private MaskType selectedMask = MaskType.None;
    private GameObject selectedMaskButton;

    private GameObject[] masks = new GameObject[5];


    private void Start()
    {
        if (maskManager == null)
        {
            Debug.LogError("maskManager is null in MaskWheelManager script in the UICanvas object. Drag and drop the reference from the Player in the scene.");
            return;
        }

        int i = 0;
        foreach (Transform child in wheelObj.transform)
        {
            if (child.GetComponent<MaskStatus>() == null) continue;

            masks[i] = child.gameObject;
            i++;
        }
    }

    // Calculates which mask is selected based on which button is enabled
    private void Update()
    {
        if (wheelObj.activeInHierarchy == false) return;

        // Highlights the closest button
        Vector3 mousePos = Mouse.current.position.ReadValue();

        GameObject closestMaskButton = null;
        float closestDistSquared = Mathf.Infinity;

        foreach (GameObject mask in masks)
        {
            if (mask.GetComponent<MaskStatus>().getStatus() == false) continue;

            RectTransform rectTransform = mask.GetComponent<RectTransform>();
            float distSquared = (mousePos - rectTransform.position).sqrMagnitude;
            if (closestMaskButton == null || distSquared < closestDistSquared)
            {
                closestDistSquared = distSquared;
                closestMaskButton = mask;
            }
        }

        MaskStatus closestMaskStatus = closestMaskButton.GetComponent<MaskStatus>();
        MaskType hoverType = closestMaskStatus.maskType;
        if (hoverType != selectedMask)
        {
            if (selectedMaskButton != null) selectedMaskButton.GetComponent<MaskStatus>().setSelected(false);

            selectedMask = hoverType;
            selectedMaskButton = closestMaskButton;
            closestMaskStatus.setSelected(true);
        }
    }

    public void ToggleMaskWheel(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if (Time.timeScale <= 0.01f) return;
            wheelObj.SetActive(true);
        }
        else if(context.canceled)
        {
            if (lockWheel == false && Time.timeScale > 0.01f) maskManager.SwitchMask(selectedMask);
            wheelObj.SetActive(false);
        }
    }

    // Locks all buttons if false; unlocks if true
    private void ToggleLock(bool status)
    {
        lockWheel = status;
        MaskStatus.setManagerLock(lockWheel);
    }
}
