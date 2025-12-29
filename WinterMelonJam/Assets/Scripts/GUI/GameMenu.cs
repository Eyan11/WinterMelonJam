using UnityEngine;
using UnityEngine.UI;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private bool enableNextLevelButton = true;
    [Header("Game Objects")]
    [SerializeField] private GameObject objFirstStar;
    [SerializeField] private GameObject objSecondStar;
    [SerializeField] private GameObject objThirdStar;
    [SerializeField] private GameObject objNextLevelButton;
    [Header("Star Icons")]
    [SerializeField] private Sprite emptyStar;
    [SerializeField] private Sprite fullStar;

    private Image imageFirstStar;
    private Image imageSecondStar;
    private Image imageThirdStar;

    private bool gameMenuEnabled = false;

    private void SetEmptyStars()
    {
        imageSecondStar.sprite = emptyStar;
        imageThirdStar.sprite = emptyStar;
    }

    public void EnableGameMenu()
    {
        if (gameMenuEnabled == true) return;
        gameMenuEnabled = true;

        gameObject.SetActive(true);

        Time.timeScale = 0f;

        imageFirstStar = objFirstStar.GetComponent<Image>();
        imageSecondStar = objSecondStar.GetComponent<Image>();
        imageThirdStar = objThirdStar.GetComponent<Image>();

        int stars = GameManager.Instance.GetSavedScore(GameManager.Instance.GetLevelNumber());

        SetEmptyStars();

        if (stars >= 1) imageFirstStar.sprite = fullStar;
        if (stars >= 2) imageSecondStar.sprite = fullStar;
        if (stars >= 3) imageThirdStar.sprite = fullStar;

        if (enableNextLevelButton == false) objNextLevelButton.SetActive(false);
    }
}
