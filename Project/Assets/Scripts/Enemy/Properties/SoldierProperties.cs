using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierProperties : MainProperties
{
    override protected void Initialize()
    {
        base.Initialize(40, Guns.Pistol, 100, 0.15f, 28, 120);
    }
}
