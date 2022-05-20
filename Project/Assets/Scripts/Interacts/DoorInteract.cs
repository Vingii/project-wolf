using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorInteract : MainInteract
{
    DoorMove doorMove; //door opening script
    public void Start()
    {
        doorMove = gameObject.GetComponent<DoorMove>();
    }
    public override void Interact(Vector3 norm)
    {
        if (canInteract)
        {
            canInteract = false;
            doorMove.Open();
        }
    }
}
