using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SilverDoorInteract : DoorInteract
{
    public override void Interact(Vector3 norm)
    {
        if (GameObject.FindWithTag("Player").GetComponent<PlayerStats>().HasSilverKey)
        {
            base.Interact(norm);
        }
    }
}
