using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainGunPickup : MainPickup
{
    public int Ammo = 12;
    new void Start()
    {
        base.Start();
        bordercol = Color.white;
    }
    public override string Value
    {
        get
        {
            return Ammo.ToString();
        }
    }
    protected override bool pickupAdd()
    {
        GlobalStats.Ammo += this.Ammo;
        GlobalStats.HasGun[3] = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerStats>().ActiveGun = Guns.ChainGun;
        return true;
    }
}
