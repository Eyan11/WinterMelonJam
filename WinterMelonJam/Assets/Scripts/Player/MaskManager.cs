using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultObj;
    [SerializeField] private GameObject monkeyObj;
    [SerializeField] private GameObject rhinoObj;
    [SerializeField] private GameObject turtleObj;
    [SerializeField] private GameObject flamingoObj;

    private enum MaskType { Default, Monkey, Rhino, Turtle, Flamingo }
    private MaskType curMask = MaskType.Default;
    private int numMasksUnlocked = 4;   // Start with all masks unlocked
    private DefaultController defaultController;
    private MonkeyController monkeyController;
    private RhinoController rhinoController;
    private TurtleController turtleController;
    private FlamingoController flamingoController;


    private void Awake()
    {   
        defaultController = defaultObj.GetComponent<DefaultController>();
        monkeyController = monkeyObj.GetComponent<MonkeyController>();
        rhinoController = rhinoObj.GetComponent<RhinoController>();
        turtleController = turtleObj.GetComponent<TurtleController>();
        flamingoController = flamingoObj.GetComponent<FlamingoController>();
    }



    // Switches mask by exiting/disabling previous mask and entering/enabling new mask
    public void SwitchMask(char maskChar)
    {

        // Exit old mask
        switch(curMask)
        {
            case MaskType.Default:
                if(maskChar == 'D')
                    return;
                
                defaultController.OnMaskExit();
                defaultObj.SetActive(false);
                break;
            case MaskType.Monkey:
                if (numMasksUnlocked < 1 || maskChar == 'M')
                    return;
                monkeyController.OnMaskExit();
                monkeyObj.SetActive(false);
                break;
            case MaskType.Rhino:
                if (numMasksUnlocked < 2 || maskChar == 'R')
                    return;
                rhinoController.OnMaskExit();
                rhinoObj.SetActive(false);
                break;
            case MaskType.Turtle:
                if (numMasksUnlocked < 3 || maskChar == 'T')
                    return;
                turtleController.OnMaskExit();
                turtleObj.SetActive(false);
                break;
            case MaskType.Flamingo:
                if (numMasksUnlocked < 4 || maskChar == 'F')
                    return;
                flamingoController.OnMaskExit();
                flamingoObj.SetActive(false);
                break;
        }

        // Enter new mask
        switch(maskChar)
        {
            case 'D':
                curMask = MaskType.Default;
                defaultObj.SetActive(true);
                defaultController.OnMaskEnter();
                break;
            case 'M':
                curMask = MaskType.Monkey;
                monkeyObj.SetActive(true);
                monkeyController.OnMaskEnter();
                break;
            case 'R':
                curMask = MaskType.Rhino;
                rhinoObj.SetActive(true);
                rhinoController.OnMaskEnter();
                break;
            case 'T':
                curMask = MaskType.Turtle;
                turtleObj.SetActive(true);
                turtleController.OnMaskEnter();
                break;
            case 'F':
                curMask = MaskType.Flamingo;
                flamingoObj.SetActive(true);
                flamingoController.OnMaskEnter();

                break;
            default:
                Debug.LogError("Incorrect character input in PlayerController.SwitchMask");
                break;
        }
    }




}
