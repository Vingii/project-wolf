using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MainPickup
{
    public int Health;
    new void Start()
    {
        base.Start();
        bordercol = new Color(1, 0.65f, 0);
    }
    public override string Value
    {
        get
        {
            return Health.ToString();
        }
    }
    protected override bool pickupAdd()
    {
        if (GlobalStats.Health < 100)
        {
            GlobalStats.Health += this.Health;
            return true;
        }
        else
        {
            return false;
        }
    }
}
