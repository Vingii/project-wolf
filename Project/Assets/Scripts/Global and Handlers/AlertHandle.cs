using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertHandle : MonoBehaviour //handles drawing UI alerts when seen by enemy
{
    public GameObject alertMarker1; //primary UI exclamation mark
    public GameObject alertMarker2; //secondary UI exclamation mark

    bool running; //is any mark currently being shown

    IEnumerator AlertCor(GameObject marker) //animate specified exclamation mark(er)
    {
        running = true;
        Color col = marker.GetComponent<Image>().color;
        float tmax = 0.5f;
        float tcurr = tmax;
        while (tcurr > 0)
        {
            tcurr -= Time.deltaTime;
            marker.GetComponent<Image>().color =
                Color.Lerp(new Color(col.r, col.g, col.b, 0), new Color(col.r, col.g, col.b, 1), tcurr / tmax);
            yield return null;
        }
        marker.GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
        running = false;
    }
    void Start()
    {
        //hide markers
        Color col = alertMarker1.GetComponent<Image>().color;
        alertMarker1.GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
        col = alertMarker2.GetComponent<Image>().color;
        alertMarker2.GetComponent<Image>().color = new Color(col.r, col.g, col.b, 0);
    }
    public void ShowAlert()
    {
        //works well enough for small amount of enemies
        if (running)
        {
            StartCoroutine(AlertCor(alertMarker2));
        }
        else
        {
            StartCoroutine(AlertCor(alertMarker1));
        }
    }
}
