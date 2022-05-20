using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FollowCamera : MonoBehaviour //rotates object to always face the player
{
    GameObject Player; //player instance
    protected void Start()
    {
        Player = GameObject.FindWithTag("Player");
    }
    void Update() //rotate object
    {
        gameObject.transform.rotation =
            Quaternion.LookRotation(gameObject.transform.position - Player.transform.position - new Vector3(0, gameObject.transform.position.y - Player.transform.position.y, 0));
    }
}
