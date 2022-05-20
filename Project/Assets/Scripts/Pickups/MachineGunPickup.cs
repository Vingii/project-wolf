using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineGunPickup : MainPickup
{
    new void Start()
    {
        base.Start();
        bordercol = Color.white;
    }
    public int Ammo = 6;
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
        GlobalStats.HasGun[2] = true;
        GameObject.FindWithTag("Player").GetComponent<PlayerStats>().ActiveGun = Guns.MachineGun;
        return true;
    }
}
