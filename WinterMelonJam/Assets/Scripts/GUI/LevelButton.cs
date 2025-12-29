using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private bool overrideEnable = false;
    [Header("Levels")]
    [SerializeField] private int level;
    [SerializeField] private int levelToUnlock;
    [Header("Game Objects")]
    [SerializeField] private GameObject objLock;
    [SerializeField] private GameObject objButton;
    [SerializeField] private GameObject firstStar;
    [SerializeField] private GameObject secondStar;
    [SerializeField] private GameObject thirdStar;
    [Header("Star Icons")]
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    private Button levelButton;
    private Image imageFirstStar;
    private Image imageSecondStar;
    private Image imageThirdStar;

    private void SetEmptyStars()
    {
        imageFirstStar.sprite = emptyStar;
        imageSecondStar.sprite = emptyStar;
        imageThirdStar.sprite = emptyStar;
    }

    // Only ran once every time the scene opens
    private void Start()
    {
        imageFirstStar = firstStar.GetComponent<Image>();
        imageSecondStar = secondStar.GetComponent<Image>();
        imageThirdStar = thirdStar.GetComponent<Image>();

        levelButton = objButton.GetComponent<Button>();

        SetEmptyStars();

        bool unlocked = GameManager.Instance.GetSavedScore(levelToUnlock) > 0;
        if (unlocked == false && overrideEnable == false) // Locked, no stars
        {
            
            levelButton.enabled = false;
            objLock.SetActive(true);
        }
        else
        {
            int stars = GameManager.Instance.GetSavedScore(level);

            if (stars >= 1) imageFirstStar.sprite = fullStar;
            if (stars >= 2) imageSecondStar.sprite = fullStar;
            if (stars >= 3) imageThirdStar.sprite = fullStar;

            levelButton.enabled = true;
            objLock.SetActive(false);
        }
    }
}
