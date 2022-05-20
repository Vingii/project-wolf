using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateY : MonoBehaviour //rotates the gameobject with constant speed around Y axis
{
    public float rotatespeed;

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(0, rotatespeed, 0, Space.Self);
    }
}
