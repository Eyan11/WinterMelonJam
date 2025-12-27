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
    private MaskType masksUnlocked = MaskType.Default;
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
                defaultController.OnMaskExit();
                defaultObj.SetActive(false);
                break;
            case MaskType.Monkey:
                monkeyController.OnMaskExit();
                monkeyObj.SetActive(false);
                break;
            case MaskType.Rhino:
                //rhinoController.OnMaskExit();
                rhinoObj.SetActive(false);
                break;
            case MaskType.Turtle:
                turtleController.OnMaskExit();
                turtleObj.SetActive(false);
                break;
            case MaskType.Flamingo:
                flamingoController.OnMaskExit();
                flamingoObj.SetActive(false);
                break;
        }

        // Enter new mask
        switch(maskChar)
        {
            case 'D':
                curMask = MaskType.Default;
                defaultController.OnMaskEnter();
                defaultObj.SetActive(true);
                break;
            case 'M':
                curMask = MaskType.Monkey;
                monkeyController.OnMaskEnter();
                monkeyObj.SetActive(true);
                break;
            case 'R':
                curMask = MaskType.Rhino;
                //rhinoController.OnMaskEnter();
                rhinoObj.SetActive(true);
                break;
            case 'T':
                curMask = MaskType.Turtle;
                turtleController.OnMaskEnter();
                turtleObj.SetActive(true);
                break;
            case 'F':
                curMask = MaskType.Flamingo;
                flamingoController.OnMaskEnter();
                flamingoObj.SetActive(true);
                break;
            default:
                Debug.LogError("Incorrect character input in PlayerController.SwitchMask");
                break;
        }
    }




}
