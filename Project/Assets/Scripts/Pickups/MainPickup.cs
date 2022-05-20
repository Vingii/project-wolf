using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainPickup : MonoBehaviour //abstract script used by every object which can be picked up
{
    protected abstract bool pickupAdd(); //return true and add values if object can be picked up
    protected Color bordercol; //color of screen flash on successful pickup
    protected static BorderHandle borderHandle; //handler script
    public abstract string Value //readonly value used to display text over the item
    {
        get;
    }
    protected void Start()
    {
        borderHandle = GameObject.FindGameObjectWithTag("UI").GetComponent<BorderHandle>();
    }
    public void Pickup()
    {
        if (pickupAdd()) //adds stats in pickupAdd
        {
            borderHandle.Flash(bordercol);
            Destroy(gameObject);
        }
    }
}
