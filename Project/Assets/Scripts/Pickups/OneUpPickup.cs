using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneUpPickup : MainPickup
{
    new void Start()
    {
        base.Start();
        bordercol = Color.white;
    }
    public override string Value
    {
        get
        {
            return "1"; //lives
        }
    }
    protected override bool pickupAdd()
    {
        GlobalStats.Health = 100;
        GlobalStats.Ammo += 25;
        GlobalStats.Lives += 1;
        return true;
    }
}
