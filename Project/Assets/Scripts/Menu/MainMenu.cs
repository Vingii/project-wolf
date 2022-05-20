using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenu : Menu
{
    new void Start()
    {
        base.Start();
        GlobalStats.DebugModePersistent = false; //clear debug flag when returning to main menu
    }
    public void PlayButton_Click()
    {
        FileHandle.DeleteSave();
        GlobalStats.Initialize();
        FileHandle.Loaded = false;
        SceneManager.LoadScene("Level");
    }
    public void LoadButton_Click()
    {
        FileHandle.Loaded = true;
        SceneManager.LoadScene("Level");
    }
    public void LeaderboardsButton_Click()
    {
        SceneManager.LoadScene("LeaderboardsMenu");
    }
    public void HelpButton_Click()
    {
        SceneManager.LoadScene("HelpMenu");
    }
    public void ExitButton_Click()
    {
        Application.Quit();
    }
}
