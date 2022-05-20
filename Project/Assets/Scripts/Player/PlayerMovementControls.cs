using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementControls : MainControls //handles moving, rotating and strafing
{
    float movespeed = 0.4f; //normal movement speed
    float rotatespeed = 2.25f;
    float strafespeed = 0.3f; //sideways speed when strafing
    PlayerStats pstats;
    void Start()
    {
        pstats = gameObject.GetComponent<PlayerStats>();
    }
    void FixedUpdate()
    {
        int rotate = 0;
        int move = 0;
        int strafe = 0;
        if (Input.GetButton("Strafe")) //move and strafe
        {
            move = System.Math.Sign(Input.GetAxisRaw("Vertical"));
            strafe = System.Math.Sign(Input.GetAxisRaw("Horizontal"));

            if (move != 0) //move diagonally
            {
                transform.Translate((transform.forward * move + Quaternion.Euler(0, 90, 0) * transform.forward * strafe * strafespeed).normalized * movespeed, Space.World);
            }
            else //move sideways
            {
                transform.Translate((Quaternion.Euler(0, 90, 0) * transform.forward * strafe).normalized * strafespeed, Space.World);
            }
        }
        else //move and rotate
        {
            rotate = System.Math.Sign(Input.GetAxisRaw("Horizontal"));
            move = System.Math.Sign(Input.GetAxisRaw("Vertical"));

            transform.Translate(transform.forward * move * movespeed, Space.World);
            transform.Rotate(0, rotate * rotatespeed, 0);
        }
    }
}
