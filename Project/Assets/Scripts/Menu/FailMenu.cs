using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FailMenu : Menu //displayed when player loses last life
{
    public TextMeshProUGUI Stats; //text element displaying statistics
    new void Start()
    {
        base.Start();
        Stats.text = "FINAL SCORE: " + GlobalStats.Score + '\n' + "FINAL FLOOR: " + GlobalStats.Level;
    }
    public void ReturnButton_Click()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
