using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class UpdateGun : MonoBehaviour //updates animated gun image based on active gun and reload time
{
    int[] animlen; //animlen[i] number of frames for gun with index i
    Sprite[][] gunsprites; //gunsprites[i][j] j-th frame of animation of gun with index i
    GameObject player; //player instance
    Image gunim; //UI element
    PlayerStats pstats; //script with current reload time
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        pstats = player.GetComponent<PlayerStats>();
        gunim = gameObject.GetComponent<Image>();
        gunsprites = new Sprite[4][];

        string animlentext = ((TextAsset)Resources.Load("Textures/UI/Guns/animlen")).text;
        StringReader reader = new StringReader(animlentext);
        for (int i = 0; i < 4; i++)
        {
            gunsprites[i] = new Sprite[int.Parse(reader.ReadLine())];
            for (int j = 0; j < gunsprites[i].Length; j++)
            {
                gunsprites[i][j] = Resources.Load<Sprite>("Textures/UI/Guns/Gun" + i + "/anim" + j);
            }
        }
    }
    void Update()
    {
        if (pstats.CanFire)
        {
            gunim.sprite = gunsprites[(int)pstats.ActiveGun][0];
        }
        else
        {
            int framenum = Mathf.Max(Mathf.CeilToInt(
                (gunsprites[(int)pstats.ActiveGun].Length - 1) *
                (1 - (pstats.ReloadTimeLeft() / FireHandle.Gunlist[(int)pstats.ActiveGun].Cooldown))
                ), 1);
            gunim.sprite = gunsprites[(int)pstats.ActiveGun][framenum];
        }
    }
}
