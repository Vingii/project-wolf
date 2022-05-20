using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldDoorInteract : DoorInteract
{
    public override void Interact(Vector3 norm)
    {
        if (GameObject.FindWithTag("Player").GetComponent<PlayerStats>().HasGoldKey)
        {
            base.Interact(norm);
        }
    }
}
