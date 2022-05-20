using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpdateInfo : MonoBehaviour //updates UI bottom bar (except for minimap)
{
    public Sprite[] gunsprites; //images of guns
    GameObject player; //player instance
    TextMeshProUGUI[] textcomp; //text components of UI blocks
    Image[] keycomp; //image components of key UI blocks
    Image guncomp; //image component of gun UI block
    PlayerStats pstats;
    void Initialize()
    {
        player = GameObject.FindWithTag("Player");
        pstats = player.GetComponent<PlayerStats>();
        textcomp = new TextMeshProUGUI[5];
        keycomp = new Image[2];
        for (int i = 0; i < 5; i++) //hierarchy order sensitive
        {
            textcomp[i] = gameObject.transform.GetChild(i).transform.GetChild(3).GetComponent<TextMeshProUGUI>();
        }
        keycomp[0] = gameObject.transform.GetChild(5).transform.GetChild(3).GetComponent<Image>();
        keycomp[1] = gameObject.transform.GetChild(6).transform.GetChild(3).GetComponent<Image>();
        guncomp = gameObject.transform.GetChild(7).transform.GetChild(3).GetComponent<Image>();
    }
    public void UpdateAll()
    {
        UpdateLevel();
        UpdateScore();
        UpdateLives();
        UpdateHealth();
        UpdateAmmo();
        UpdateKeys();
        UpdateGun();
    }
    public void UpdateLevel()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        textcomp[0].text = GlobalStats.Level.ToString();
    }
    public void UpdateScore()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        textcomp[1].text = GlobalStats.Score.ToString();
    }
    public void UpdateLives()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        textcomp[2].text = GlobalStats.Lives.ToString();
    }
    public void UpdateHealth()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        textcomp[3].text = GlobalStats.Health.ToString() + '%';
    }
    public void UpdateAmmo()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        textcomp[4].text = GlobalStats.Ammo.ToString();
    }
    public void UpdateKeys()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        keycomp[0].enabled = pstats.HasGoldKey;
        keycomp[1].enabled = pstats.HasSilverKey;
    }
    public void UpdateGun()
    {
        if (textcomp == null)
        {
            Initialize();
        }
        guncomp.sprite = gunsprites[(int)pstats.ActiveGun];
    }
}
