using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    public enum TitleSceneMenus { Title, Controls, Credits }
    private TitleSceneMenus curMenu = TitleSceneMenus.Title;

    [SerializeField] private GameObject titleMenu; 
    [SerializeField] private GameObject controlsMenu;
    [SerializeField] private GameObject creditsMenu;  


    
    private void CloseCurrentMenu()
    {
        switch(curMenu)
        {
            case TitleSceneMenus.Title:
                titleMenu.SetActive(false);
                break;
            case TitleSceneMenus.Controls:
                controlsMenu.SetActive(false);
                break;
            case TitleSceneMenus.Credits:
                creditsMenu.SetActive(false);
                break;
        }
    }

    // *** Events *************************************************************

    // Called on a button OnClick event
    public void ToggleTitleMenu()
    {
        CloseCurrentMenu();
        curMenu = TitleSceneMenus.Title;
        titleMenu.SetActive(true);
    }

    public void ToggleControlsMenu()
    {
        CloseCurrentMenu();
        if(curMenu == TitleSceneMenus.Controls)
        {
            curMenu = TitleSceneMenus.Title;
            titleMenu.SetActive(true);
        }
        else
        {
            curMenu = TitleSceneMenus.Controls;
            controlsMenu.SetActive(true);
        }
    }

    public void ToggleCreditsMenu()
    {
        CloseCurrentMenu();
        if(curMenu == TitleSceneMenus.Credits)
        {
            curMenu = TitleSceneMenus.Title;
            titleMenu.SetActive(true);
        }
        else
        {
            curMenu = TitleSceneMenus.Credits;
            creditsMenu.SetActive(true);
        }
    }

    public void OpenLevelSelect()
    {
        Debug.Log("TODO - OPEN LEVEL SELECT SCENE NOW");
    }

    public void QuitGame()
    {
        Debug.Log("Application.Quit() called, game should quit if playing an actual build");
        Application.Quit(); 
    }

}
