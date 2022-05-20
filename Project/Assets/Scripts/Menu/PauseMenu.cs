using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseMenu : Menu //ingame pause menu overlay
{
    ScreenHandle screenHandle; //handler script
    new void Start()
    {
        screenHandle = GameObject.FindWithTag("Handler").GetComponent<ScreenHandle>();
    }
    public void ResumeButton_Click()
    {
        screenHandle.Resume();
    }
    public void ExitButton_Click()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
