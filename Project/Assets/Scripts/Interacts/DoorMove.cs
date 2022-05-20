using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMove : MonoBehaviour
{
    DoorInteract doorInteract; //script which triggers movement
    public float openspeed; //door movespeed
    public float openmaxt; //how long door remains open
    float opentimer; //how much is left until door closes
    bool opening; //if door is opening (compared to closing)
    Vector3 opendir; //direction of opening
    LayerMask layerMask; //against which colliders should door reopen
    float doorrad; //half of door width
    public float raylen; //length of collision ray
    public GameObject Side; //thin side quad of the door

    public void Open()
    {
        enabled = true;
        opening = true;
    }
    void Start()
    {
        doorrad = gameObject.GetComponent<BoxCollider>().bounds.extents.x;
        layerMask = LayerMask.GetMask("Player", "Enemy");
        doorInteract = gameObject.GetComponent<DoorInteract>();
        enabled = false;
        opendir = gameObject.transform.TransformVector(Vector3.forward).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        if (opening) //opening
        {
            if (gameObject.transform.localPosition.z < 4) //open more
            {
                gameObject.transform.Translate(opendir * openspeed * 60 * Time.deltaTime, Space.World);
            }
            else //snap to open position
            {
                gameObject.transform.Translate(0, 0, 4 - gameObject.transform.localPosition.z);
                opening = false;
                opentimer = openmaxt;
            }
        }
        else if (opentimer > 0) //open
        {
            opentimer -= Time.deltaTime;
        }
        else //closing
        {
            if (gameObject.transform.localPosition.z > 0) //close more
            {
                if (!Physics.Raycast(Side.transform.position + doorrad * (Quaternion.Euler(0, -90, 0) * opendir), -opendir, raylen, layerMask) &&
                    !Physics.Raycast(Side.transform.position, -opendir, raylen, layerMask) &&
                    !Physics.Raycast(Side.transform.position + doorrad * (Quaternion.Euler(0, 90, 0) * opendir), -opendir, raylen, layerMask))
                {
                    gameObject.transform.Translate(-opendir * openspeed * 60 * Time.deltaTime, Space.World);
                }
                else //reopen
                {
                    opening = true;
                }
            }
            else //snap to closed position
            {
                gameObject.transform.Translate(0, 0, -gameObject.transform.localPosition.z);
                doorInteract.canInteract = true;
                enabled = false;
            }
        }
    }
}