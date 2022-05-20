using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SeedMenu : Menu //displays when choosing run seed
{
    public TMP_InputField seedField;
    new void Start()
    {
        base.Start();
    }
    public void StartButton_Click()
    {
        if (int.TryParse(seedField.text, out int seed))
        {
            GlobalStats.Initialize(seed);
            FileHandle.Loaded = false;
            SceneManager.LoadScene("Level");
        }
        else //ignore invalid input
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
