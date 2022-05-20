using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractControls : MainControls //handles interact button
{
    RaycastHit hit;
    public float raylen; //max length of interaction
    int layerMask; //against what colliders we stop interaction ray
    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player", "Minimap", "Occlusion");
    }
    void Update()
    {
        if (Input.GetButtonDown("Interact"))
        {
            if (Physics.Raycast(transform.position, transform.forward * raylen, out hit, raylen, layerMask)
                && hit.collider.gameObject.tag == "Interactable")
            {
                hit.collider.gameObject.GetComponent<MainInteract>().Interact(hit.normal);
            }
        }
        if (Input.GetKey(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.J))
        {
            if (GlobalStats.DebugMode)
            {
                GlobalStats.DebugMode = false;
            }
            else
            {
                GlobalStats.DebugMode = true;
            }
        }
    }
}
