using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPickupHandle : MonoBehaviour //checks for nearby pickups
{
    public float Range; //pickup range
    LayerMask pickupMask; //layer mask of pickup objects
    Vector3 extents; //dimensions of detection box
    void Start()
    {
        extents = new Vector3(Range, 2, Range);
        pickupMask = LayerMask.GetMask("Pickup");
    }
    void Update()
    {
        Collider[] collist = Physics.OverlapBox(gameObject.transform.position, extents, Quaternion.Euler(0, 0, 0), pickupMask);
        foreach (Collider collider in collist)
        {
            collider.gameObject.GetComponent<MainPickup>().Pickup();
        }
    }
}
