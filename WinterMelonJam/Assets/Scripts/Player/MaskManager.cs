using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultObj;
    [SerializeField] private GameObject monkeyObj;
    [SerializeField] private GameObject rhinoObj;
    [SerializeField] private GameObject turtleObj;
    [SerializeField] private GameObject flamingoObj;

    private MaskType curMask = MaskType.Default;
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
    public void SwitchMask(MaskType nextMask)
    {

        // Exit old mask
        if (curMask == nextMask) return;

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
                rhinoController.OnMaskExit();
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
        switch(nextMask)
        {
            case MaskType.Default:
                curMask = MaskType.Default;
                defaultObj.SetActive(true);
                defaultController.OnMaskEnter();
                break;
            case MaskType.Monkey:
                curMask = MaskType.Monkey;
                monkeyObj.SetActive(true);
                monkeyController.OnMaskEnter();
                break;
            case MaskType.Rhino:
                curMask = MaskType.Rhino;
                rhinoObj.SetActive(true);
                rhinoController.OnMaskEnter();
                break;
            case MaskType.Turtle:
                curMask = MaskType.Turtle;
                turtleObj.SetActive(true);
                turtleController.OnMaskEnter();
                break;
            case MaskType.Flamingo:
                curMask = MaskType.Flamingo;
                flamingoObj.SetActive(true);
                flamingoController.OnMaskEnter();
                break;
            default:
                curMask = MaskType.Default;
                defaultObj.SetActive(true);
                defaultController.OnMaskEnter();
                Debug.LogError("Illegal mask input!");
                break;
        }
    }




}
