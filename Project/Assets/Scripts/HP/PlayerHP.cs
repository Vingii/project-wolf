using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHP : MainHP //player health handler
{
    public GameObject Minimap; //minimap UI element
    ScreenHandle screenHandle; //handler script
    BorderHandle borderHandle; //handler script
    public GameObject deathIm; //death border UI element
    public override void Die()
    {
        StartCoroutine(DieCor());
    }
    IEnumerator DieCor() //disables control and plays death 'animation'
    {
        float tmax = 0.2f; //length of death animation
        float tcurr = tmax; //remaining animation time
        //stop time
        Time.timeScale = 0;
        foreach (MainControls controlScript in gameObject.GetComponents<MainControls>())
        {
            controlScript.enabled = false; //disables all player control
        }
        Minimap.GetComponent<MinimapHandle>().enabled = false; //prevents player from enabling map
        //fade in UI element
        while (tcurr > 0)
        {
            tcurr -= Time.unscaledDeltaTime;
            deathIm.GetComponent<Image>().color = Color.Lerp(new Color(1, 0, 0, 0), Color.red, 1 - tcurr / tmax);
            yield return null;
        }
        //return control
        Time.timeScale = 1;
        foreach (MainControls controlScript in gameObject.GetComponents<MainControls>())
        {
            controlScript.enabled = true;
        }
        Minimap.GetComponent<MinimapHandle>().enabled = true;
        //screen transition
        if (GlobalStats.Lives <= 1)
        {
            screenHandle.GameOver();
        }
        else
        {
            GlobalStats.Lives -= 1;
            GlobalStats.Level -= 1;
            GlobalStats.Health = 100;
            GlobalStats.Ammo = 8;
            GlobalStats.HasGun[3] = false;
            GlobalStats.HasGun[2] = false;
            SceneManager.LoadScene("Level");
        }
    }
    void Start()
    {
        screenHandle = GlobalStats.handler.GetComponent<ScreenHandle>();
        deathIm.GetComponent<Image>().color = new Color(1, 0, 0, 0);
        borderHandle = GameObject.FindGameObjectWithTag("UI").GetComponent<BorderHandle>();
    }
    public override void TakeDamage(int damage)
    {
        borderHandle.Flash(Color.red);
        if (!GlobalStats.DebugMode)
        {
            GlobalStats.Health -= damage;
            if (GlobalStats.Health <= 0)
            {
                GlobalStats.Health = 0;
                Die();
            }
        }
    }
}
