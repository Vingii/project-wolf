using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class HelpMenu : Menu //controls menu
{
    new void Start()
    {
        base.Start();
    }
    public void ReturnButton_Click()
    {
        SceneManager.LoadScene("MainMenu");
    }
    public void SeedButton_Click()
    {
        SceneManager.LoadScene("SeedMenu");
    }
}
