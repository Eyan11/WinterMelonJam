using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultObj;
    [SerializeField] private GameObject monkeyObj;
    [SerializeField] private GameObject rhinoObj;
    [SerializeField] private GameObject turtleObj;
    [SerializeField] private GameObject flamingoObj;

    private MaskType curMask = MaskType.Default;



    // Switches mask by exiting/disabling previous mask and entering/enabling new mask
    public void SwitchMask(MaskType nextMask)
    {

        // Exit old mask
        if (curMask == nextMask) return;

        switch(curMask)
        {
            case MaskType.Default:
                defaultObj.SetActive(false);
                break;
            case MaskType.Monkey:
                monkeyObj.SetActive(false);
                break;
            case MaskType.Rhino:
                rhinoObj.SetActive(false);
                break;
            case MaskType.Turtle:
                turtleObj.SetActive(false);
                break;
            case MaskType.Flamingo:
                flamingoObj.SetActive(false);
                break;
        }

        // Enter new mask
        switch(nextMask)
        {
            case MaskType.Default:
                curMask = MaskType.Default;
                defaultObj.SetActive(true);
                break;
            case MaskType.Monkey:
                curMask = MaskType.Monkey;
                monkeyObj.SetActive(true);
                break;
            case MaskType.Rhino:
                curMask = MaskType.Rhino;
                rhinoObj.SetActive(true);
                break;
            case MaskType.Turtle:
                curMask = MaskType.Turtle;
                turtleObj.SetActive(true);
                break;
            case MaskType.Flamingo:
                curMask = MaskType.Flamingo;
                flamingoObj.SetActive(true);
                break;
            default:
                curMask = MaskType.Default;
                defaultObj.SetActive(true);
                Debug.LogError("Illegal mask input!");
                break;
        }
    }




}
