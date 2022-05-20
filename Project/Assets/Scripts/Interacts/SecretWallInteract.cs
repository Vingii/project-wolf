using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretWallInteract : MainInteract
{
    public float raylen; //length of collision ray
    public float movespeed; //push speed
    Vector3 movedir; //opposite from where wall was pushed
    RaycastHit hit;
    float boxrad; //half of wall length
    int layerMask; //against what objects wall should stop
    public override void Interact(Vector3 norm)
    {
        if (canInteract)
        {
            canInteract = false;
            enabled = true;
            movedir = -norm;
            boxrad = gameObject.GetComponent<BoxCollider>().bounds.extents.x; //assume square wall base
        }
    }
    void Start()
    {
        layerMask = LayerMask.GetMask("Wall");
        enabled = false;
    }
    public void Update()
    {
        if (!Physics.Raycast(transform.position + movedir * boxrad, movedir, out hit, raylen, layerMask))
        {
            transform.Translate(movedir * movespeed, Space.World);
        }
        else
        {
            transform.Translate(movedir * hit.distance, Space.World);
            enabled = false;
        }
        //Debug.DrawRay(transform.position + movedir * boxdiam, movedir * raylen);
    }
}
