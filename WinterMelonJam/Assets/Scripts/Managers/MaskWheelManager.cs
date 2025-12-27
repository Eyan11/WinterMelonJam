using UnityEngine;
using UnityEngine.InputSystem;

public class MaskWheelManager : MonoBehaviour
{
    [SerializeField] private GameObject wheelObj;
    [SerializeField] private MaskManager maskManager;

    private enum MaskType { None, Default, Monkey, Rhino, Turtle, Flamingo }
    private MaskType selectedMask = MaskType.None;


    private void Awake()
    {
        if(maskManager == null)
            Debug.LogError("maskManager is null in MaskWheelManager script in the UICanvas object. Drag and drop the reference from the Player in the scene.");
    }

    public void ToggleMaskWheel(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            wheelObj.SetActive(true);
        }
        else if(context.canceled)
        {
            switch(selectedMask)
            {
                case MaskType.None:
                    break;
                case MaskType.Default:
                    maskManager.SwitchMask('D');
                    break;
                case MaskType.Monkey:
                    maskManager.SwitchMask('M');
                    break;
                case MaskType.Rhino:
                    maskManager.SwitchMask('R');
                    break;
                case MaskType.Turtle:
                    maskManager.SwitchMask('T');
                    break;
                case MaskType.Flamingo:
                    maskManager.SwitchMask('F');
                    break;
            }
            wheelObj.SetActive(false);
        }
    }



    // ******************************************************************
    public void OnHoverDefaultMask(bool isEntered)
    {
        if(isEntered)
            selectedMask = MaskType.Default;
        else
            selectedMask = MaskType.None;
    }

    public void OnHoverMonkeyMask(bool isEntered)
    {
        if(isEntered)
            selectedMask = MaskType.Monkey;
        else
            selectedMask = MaskType.None;
    }

    public void OnHoverRhinoMask(bool isEntered)
    {
        if(isEntered)
            selectedMask = MaskType.Rhino;
        else
            selectedMask = MaskType.None;
    }

    public void OnHoverTurtleMask(bool isEntered)
    {
        if(isEntered)
            selectedMask = MaskType.Turtle;
        else
            selectedMask = MaskType.None;
    }

    public void OnHoverFlamingoMask(bool isEntered)
    {
        if(isEntered)
            selectedMask = MaskType.Flamingo;
        else
            selectedMask = MaskType.None;
    }

    public void OnDefaultMaskClicked()
    {
        selectedMask = MaskType.None;
        maskManager.SwitchMask('D');
        wheelObj.SetActive(false);
    }

    public void OnMonkeyMaskClicked()
    {
        selectedMask = MaskType.None;
        maskManager.SwitchMask('M');
        wheelObj.SetActive(false);
    }

    public void OnRhinoMaskClicked()
    {
        selectedMask = MaskType.None;
        maskManager.SwitchMask('R');
        wheelObj.SetActive(false);
    }

    public void OnTurtleMaskClicked()
    {
        selectedMask = MaskType.None;
        maskManager.SwitchMask('T');
        wheelObj.SetActive(false);
    }

    public void OnFlamingoMaskClicked()
    {
        selectedMask = MaskType.None;
        maskManager.SwitchMask('F');
        wheelObj.SetActive(false);
    }

}
