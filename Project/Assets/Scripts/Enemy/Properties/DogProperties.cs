using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogProperties : MainProperties
{
    override protected void Initialize()
    {
        base.Initialize(1, Guns.Knife, 100, 0.15f, 20, 100);
    }
}
