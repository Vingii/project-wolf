using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MainPickup
{
    public int Ammo;
    new void Start()
    {
        base.Start();
        bordercol = Color.green;
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
        if (GlobalStats.Ammo < 99)
        {
            GlobalStats.Ammo += this.Ammo;
            return true;
        }
        else
        {
            return false;
        }
    }
}
