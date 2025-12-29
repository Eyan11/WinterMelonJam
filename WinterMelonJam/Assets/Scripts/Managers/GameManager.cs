using UnityEngine;
using UnityEngine.SceneManagement;  // To open scenes
using System.Collections.Generic;  // For dictionary

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;    //global reference to this script

    [System.Serializable]
    private struct LevelStarInfo
    {
        [Header("Max allowed mask switches")]
        public int threeStar;
        public int twoStar;
        public int oneStar;
    }
    [SerializeField] private LevelStarInfo[] levelStarInfo = new LevelStarInfo[12];

    private int[] levelStarScore = new int[12];    // The actual star score player achieved
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
    private bool SaveScore()
    {
        // Get new score and reset mask switches
        int levelIndex = GetCurLevelIndex();
        int newLevelScore = GetScoreFromCurMaskSwitches(levelIndex);
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

    // Returns score using current number of mask switches - SaveScore() helper method
    private int GetScoreFromCurMaskSwitches(int levelIndex)
    {
        if(curLevelMaskSwitches <= levelStarInfo[levelIndex].threeStar)
            return 3;
        else if(curLevelMaskSwitches <= levelStarInfo[levelIndex].twoStar)
            return 2;
        return 1;
    }

    // Returns the level number according to current scene name
    private int GetCurLevelIndex()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (!sceneName.StartsWith("Level"))
        {
            Debug.LogError("Invalid level scene name: " + sceneName);
            return -1;
        }

        string numberPart = sceneName.Substring(5); // after "Level"
        
        if (!int.TryParse(numberPart, out int levelNumber))
        {
            Debug.LogError("Could not parse level number from scene name: " + sceneName);
            return -1;
        }

        return levelNumber - 1;
    }



    // *** Public Methods ***************************************************************

    // Returns the score that is saved for the player, disregarding the current number of mask switches. For level select screen.
    public int GetSavedScore(int levelNumber)
    {
        return levelStarScore[levelNumber - 1];
    }


    // Increments number of mask switches
    public void SwitchMask()
    {
        curLevelMaskSwitches++;
    }

    public int GetLevelNumber()
    {
        int levelIndex = GetCurLevelIndex();
        if (levelIndex != -1) return levelIndex + 1;

        Debug.LogError("ERROR: Invalid level!");
        return -1;
    }

    public void RestartLevel()
    {
        curLevelMaskSwitches = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void CompleteLevel()
    {
        SaveScore();
    }

}
