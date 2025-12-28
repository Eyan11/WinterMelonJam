using UnityEngine;
using UnityEngine.SceneManagement;  // To open scenes
using System.Collections.Generic;   // For dictionary

public class GameManager : MonoBehaviour
{
    //global reference to this script
    public static GameManager Instance;

    [System.Serializable]
    private struct LevelStarInfo
    {
        [Header("Max allowed mask switches")]
        public int threeStar;
        public int twoStar;
        public int oneStar;
    }
    [SerializeField] private LevelStarInfo[] levelStarInfo;

    private int[] levelStarScore;    // The actual star score player achieved
    private int curLevelMaskSwitches = 0;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }


    // Called when level is finished. Updates the level star score and returns true if it's a new high score.
    private bool SaveScore(int levelNum)
    {
        // Get new score and reset mask switches
        int levelIndex = levelNum-1;
        int newLevelScore = GetScore(levelNum);
        curLevelMaskSwitches = 0;

        // Set new score and return if it's a new high score or not
        if(levelStarScore[levelIndex] < newLevelScore)
        {
            levelStarScore[levelIndex] = newLevelScore;
            return true;
        }
        else
        {
            levelStarScore[levelIndex] = newLevelScore;
            return false;
        }
    }

    // Returns score using current number of mask switches
    public int GetScore(int levelNum)
    {
        int levelIndex = levelNum-1;
        if(curLevelMaskSwitches <= levelStarInfo[levelIndex].threeStar)
            return 3;
        else if(curLevelMaskSwitches <= levelStarInfo[levelIndex].twoStar)
            return 2;
        return 1;
    }


    // Increments number of mask switches
    public void SwitchMask()
    {
        curLevelMaskSwitches++;
    }


    public void RestartLevel()
    {
        curLevelMaskSwitches = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteLevel(int levelNum)
    {
        SaveScore(levelNum);
        SceneManager.LoadScene("LevelSelectScene");
    }
}
