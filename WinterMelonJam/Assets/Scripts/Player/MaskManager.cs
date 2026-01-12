using System.Runtime.CompilerServices;
using UnityEngine;

public class MaskManager : MonoBehaviour
{
    [SerializeField] private GameObject defaultObj;
    [SerializeField] private GameObject monkeyObj;
    [SerializeField] private GameObject rhinoObj;
    [SerializeField] private GameObject turtleObj;
    [SerializeField] private GameObject flamingoObj;
    [SerializeField] private GameObject maskTransitionObj;
    [SerializeField] private AudioClip exitMaskSfx;

    private SpriteRenderer defaultSpriteRend;
    private SpriteRenderer monkeySpriteRend;
    private SpriteRenderer rhinoSpriteRend;
    private SpriteRenderer turtleSpriteRend;
    private SpriteRenderer flamingoSpriteRend;
    private SpriteRenderer maskTransitionSpriteRend;

    private Animator anim;
    private Rigidbody2D body;
    private PlayerManager playerManager;
    private MaskType curMask = MaskType.Default;


    private void Awake()
    {
        anim = maskTransitionObj.GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        playerManager = GetComponent<PlayerManager>();

        defaultSpriteRend = defaultObj.GetComponent<SpriteRenderer>();
        monkeySpriteRend = monkeyObj.GetComponent<SpriteRenderer>();
        rhinoSpriteRend = rhinoObj.GetComponent<SpriteRenderer>();
        turtleSpriteRend = turtleObj.GetComponent<SpriteRenderer>();
        flamingoSpriteRend = flamingoObj.GetComponent<SpriteRenderer>();
        maskTransitionSpriteRend = maskTransitionObj.GetComponent<SpriteRenderer>();
    }


    // Switches mask by exiting/disabling previous mask and entering/enabling new mask
    public void SwitchMask(MaskType nextMask)
    {
        if (curMask == nextMask) return;

        anim.SetBool("enterDefault", false);
        anim.SetBool("enterMonkey", false);
        anim.SetBool("enterRhino", false);
        anim.SetBool("enterTurtle", false);
        anim.SetBool("enterFlamingo", false);

        maskTransitionObj.SetActive(true);
        body.linearVelocity = new Vector2(0f, body.linearVelocity.y);   // Stop x velocity

        if(GameManager.Instance != null)
            GameManager.Instance.SwitchMask();

        if(curMask != MaskType.Default)    // If player has to remove their mask, play exit mask sfx
            playerManager.PlayOneShotSFX(exitMaskSfx);

        // Exit old mask
        switch(curMask)
        {
            case MaskType.Default:
                defaultObj.SetActive(false);
                anim.SetTrigger("exitDefault");
                maskTransitionSpriteRend.flipX = defaultSpriteRend.flipX;
                break;
            case MaskType.Monkey:
                monkeyObj.SetActive(false);
                anim.SetTrigger("exitMonkey");
                maskTransitionSpriteRend.flipX = monkeySpriteRend.flipX;
                break;
            case MaskType.Rhino:
                rhinoObj.SetActive(false);
                anim.SetTrigger("exitRhino");
                maskTransitionSpriteRend.flipX = rhinoSpriteRend.flipX;
                break;
            case MaskType.Turtle:
                turtleObj.SetActive(false);
                anim.SetTrigger("exitTurtle");
                maskTransitionSpriteRend.flipX = turtleSpriteRend.flipX;
                break;
            case MaskType.Flamingo:
                flamingoObj.SetActive(false);
                anim.SetTrigger("exitFlamingo");
                maskTransitionSpriteRend.flipX = flamingoSpriteRend.flipX;
                break;
        }

        curMask = nextMask;

        // Queue up animation to enter new mask when exit mask animation finishes
        switch(nextMask)
        {
            case MaskType.Default:
                anim.SetBool("enterDefault", true);
                break;
            case MaskType.Monkey:
                anim.SetBool("enterMonkey", true);
                break;
            case MaskType.Rhino:
                anim.SetBool("enterRhino", true);
                break;
            case MaskType.Turtle:
                anim.SetBool("enterTurtle", true);
                break;
            case MaskType.Flamingo:
                anim.SetBool("enterFlamingo", true);
                break;
            default:
                curMask = MaskType.Default;
                anim.SetBool("enterDefault", true);
                Debug.LogError("Illegal mask input on switch!");
                break;
        }
    }


    // Called when the mask exit animation finishes by FinishedTransitionBehaviour script
    public void OnMaskAnimFinish()
    {
        maskTransitionObj.SetActive(false);

        // Enable new mask gameobject
        switch(curMask)
        {
            case MaskType.Default:
                defaultSpriteRend.flipX = maskTransitionSpriteRend.flipX;
                defaultObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Default);
                break;
            case MaskType.Monkey:
                monkeySpriteRend.flipX = maskTransitionSpriteRend.flipX;
                monkeyObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Monkey);
                break;
            case MaskType.Rhino:
                rhinoSpriteRend.flipX = maskTransitionSpriteRend.flipX;
                rhinoObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Rhino);
                break;
            case MaskType.Turtle:
                turtleSpriteRend.flipX = maskTransitionSpriteRend.flipX;
                turtleObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Turtle);
                break;
            case MaskType.Flamingo:
                flamingoSpriteRend.flipX = maskTransitionSpriteRend.flipX;
                flamingoObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Flamingo);
                break;
            default:
                curMask = MaskType.Default;
                defaultSpriteRend.flipX = maskTransitionSpriteRend.flipX;
                defaultObj.SetActive(true);
                CanvasManager.Instance.SwitchControlsUI(ControlsType.Default);
                Debug.LogError("Illegal mask input on finish!");
                break;
        }
    }

}
