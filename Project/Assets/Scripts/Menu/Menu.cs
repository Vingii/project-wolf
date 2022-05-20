using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public abstract class Menu : MonoBehaviour //abstract menu screen script
{
    public Selectable InitialSelect; //which UI element should be selected by default
    protected void Start()
    {
        InitialSelect.Select();
    }
}
