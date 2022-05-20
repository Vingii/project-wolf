using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverKeyPickup : MainPickup
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
            return "";
        }
    }
    protected override bool pickupAdd()
    {
        GameObject.FindWithTag("Player").GetComponent<PlayerStats>().HasSilverKey = true;
        return true;
    }
}
