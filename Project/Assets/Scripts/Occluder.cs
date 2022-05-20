using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Occluder : MonoBehaviour //hides object groups that are far from the player to save resources
{
    private List<GameObject> ocludees = new List<GameObject>(); //objects the occluder should hide
    private void Start() //hide everything at the start, OnTriggerEnter is triggered even on frame 1
    {
        HideOcludees();
    }
    // Update is called once per frame
    void ShowOcludees() //show all connected objects
    {
        foreach (GameObject obj in ocludees)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }
    void HideOcludees() //hide all connected objects
    {
        foreach (GameObject obj in ocludees)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }
    private void OnTriggerEnter(Collider other) //shows objects when player gets close
    {
        if (other.CompareTag("Player"))
        {
            ShowOcludees();
        }
    }
    private void OnTriggerExit(Collider other) //hides objects when player gets far
    {
        if (other.CompareTag("Player"))
        {
            HideOcludees();
        }
    }
    public void AddOccludee(GameObject obj) //adds obj to list of connected objects
    {
        ocludees.Add(obj);
    }
}
