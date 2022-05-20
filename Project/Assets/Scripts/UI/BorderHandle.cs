using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BorderHandle : MonoBehaviour //handles flashing UI borders
{
    Coroutine fl; //flash coroutine
    bool corrunning; //if flash coroutine is running
    GameObject border; //flash border UI element
    void Start() //finds border in hierarchy
    {
        border = gameObject.transform.GetChild(2).gameObject; //assumes border is 3rd child of Canvas
        border.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }
    public void Flash(Color col) //flashes the border using color col
    {
        if (corrunning)
        {
            StopCoroutine(fl);
            corrunning = false;
        }
        fl = StartCoroutine(flash(col));
    }
    IEnumerator flash(Color col) //plays flash anim. using color col
    {
        corrunning = true;
        float tmax = 0.2f; //length of anim.
        float tcurr = tmax;
        while (tcurr > 0)
        {
            tcurr -= Time.deltaTime;
            border.GetComponent<Image>().color = Color.Lerp(new Color(col.r, col.g, col.b, 0), col, tcurr / tmax);
            yield return null;
        }
        border.GetComponent<Image>().color = new Color(0, 0, 0, 0);
        corrunning = false;
    }
}
