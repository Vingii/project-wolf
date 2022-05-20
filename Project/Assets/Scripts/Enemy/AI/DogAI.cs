﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogAI : MainAI
{
    protected override bool WantsToAttack()
    {
        if ((Vector3.Distance(gameObject.transform.position, lastKnownCoords) < 4) && (Vector3.Angle(Player.transform.position - gameObject.transform.position, gameObject.transform.forward) < 30))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    new void Start()
    {
        base.Start();
        followMax = 5;
        turnRate = 8;
    }
}
