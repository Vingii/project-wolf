using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextHandler : FollowCamera //displays pickup value over the item
{
    new void Start()
    {
        base.Start();
        gameObject.GetComponent<TextMeshPro>().text = gameObject.GetComponentInParent<MainPickup>().Value;
    }
}
