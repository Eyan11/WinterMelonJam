using UnityEngine;
using UnityEngine.UI;

public class WorldLock : MonoBehaviour
{
    [SerializeField] private bool overrideEnable = false;
    [SerializeField] private int levelToCheck;
    [Header("Game Objects")]
    [SerializeField] private GameObject objIndicator;
    [SerializeField] private GameObject objIcon;
    [Header("Locked Icon")]
    [SerializeField] private Sprite spriteIndicatorLock;
    [Header("Unlocked Icon")]
    [SerializeField] private Sprite spriteIndicatorUnlocked;

    private Image indicator;
    private Image icon;

    // Only ran once every time the scene opens
    private void Start()
    {

        indicator = objIndicator.GetComponent<Image>();
        icon = objIcon.GetComponent<Image>();

        int stars = GameManager.Instance.GetSavedScore(levelToCheck);
        if (stars <= 0 && overrideEnable == false) // Locked, no stars
        {
            indicator.sprite = spriteIndicatorLock;
            icon.enabled = false;
        }
        else
        {
            indicator.sprite = spriteIndicatorUnlocked;
            icon.enabled = true;
        }
    }
}
