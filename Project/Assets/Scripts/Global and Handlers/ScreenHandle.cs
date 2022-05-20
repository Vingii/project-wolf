using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenHandle : MonoBehaviour //handles pause menu and transitions
{
    public GameObject pauseScreen; //pause UI element
    public GameObject fadeOut; //fadeout UI element before entering elevator
    public GameObject Player; //player instance
    public GameObject Minimap; //minimap UI element
    public float fadetime; //fadeout timer before entering elevator
    public void Pause() //pauses game
    {
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        pauseScreen.GetComponent<Menu>().InitialSelect.Select();
        foreach (MainControls controlScript in Player.GetComponents<MainControls>())
        {
            controlScript.enabled = false;
        }
        Minimap.GetComponent<MinimapHandle>().enabled = false;
    }
    public void Resume() //resumes game
    {
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
        foreach (MainControls controlScript in Player.GetComponents<MainControls>())
        {
            controlScript.enabled = true;
        }
        Minimap.GetComponent<MinimapHandle>().enabled = true;
    }
    public void GameOver() //end game after losing final life
    {
        FileHandle.DeleteSave();
        if (!GlobalStats.DebugModePersistent)
        {
            FileHandle.LeaderboardsAdd(5);
        }
        SceneManager.LoadScene("FailMenu");
    }
    public void FinishLevel() //enter elevator
    {
        Time.timeScale = 0;
        enabled = false;
        GlobalStats.Score += 1000 * GlobalStats.Level;
        foreach (MainControls controlScript in Player.GetComponents<MainControls>())
        {
            controlScript.enabled = false;
        }
        StartCoroutine(LevelEnd());
    }
    IEnumerator LevelEnd() //ending fadeout
    {
        float fade = fadetime;
        while (true)
        {
            fade -= Time.unscaledDeltaTime;
            fadeOut.GetComponent<Image>().color = new Color(fadeOut.GetComponent<Image>().color.r, fadeOut.GetComponent<Image>().color.g, fadeOut.GetComponent<Image>().color.b, 1 - fade / fadetime);
            if (fade <= 0)
            {
                Time.timeScale = 1;
                SceneManager.LoadScene("FinishMenu");
            }
            yield return null;
        }
    }
    void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (pauseScreen.activeInHierarchy)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
}