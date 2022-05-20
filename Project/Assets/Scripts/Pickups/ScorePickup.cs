using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePickup : MainPickup
{
    new void Start()
    {
        base.Start();
        bordercol = Color.yellow;
    }
    public int Score;
    public override string Value
    {
        get
        {
            return Score.ToString();
        }
    }
    protected override bool pickupAdd()
    {
        GlobalStats.Score += this.Score;
        return true;
    }
}
