using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void SwitchFromScene()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(sceneName);
    }
}
