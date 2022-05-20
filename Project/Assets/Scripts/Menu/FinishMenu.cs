using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FinishMenu : Menu //displayed when player enters elevator
{
    public TextMeshProUGUI Stats;
    new void Start()
    {
        if (!GlobalStats.DebugModePersistent) //don't save debug runs
        {
            FileHandle.Save();
        }
        base.Start();
        Stats.text = "FLOOR " + GlobalStats.Level + " COMPLETE\nCURRENT SCORE:\n" + GlobalStats.Score;
    }
    public void ReturnButton_Click()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ContinueButton_Click()
    {
        SceneManager.LoadScene("Level");
    }
}
