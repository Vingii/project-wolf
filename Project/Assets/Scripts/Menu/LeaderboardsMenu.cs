using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class LeaderboardsMenu : Menu
{
    int boardlen = 5; //number of leaderboard entries
    public GameObject Stats;
    new void Start()
    {
        base.Start();
        string[] boardtxt = new string[10]; //2 text elements per entry
        boardtxt = FileHandle.LeaderboardsShow(boardlen);
        for (int i = 0; i < boardlen; i++)
        {
            Stats.transform.GetChild(i + 1).GetChild(0).GetComponent<TextMeshProUGUI>().text = boardtxt[2 * i];
            Stats.transform.GetChild(i + 1).GetChild(1).GetComponent<TextMeshProUGUI>().text = boardtxt[2 * i + 1];
        }
    }
    public void ReturnButton_Click()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void ResetButton_Click()
    {
        FileHandle.LeaderboardsClear();
        SceneManager.LoadScene("LeaderboardsMenu");
    }
}
