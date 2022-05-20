using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTileQuad : MonoBehaviour //displays object on minimap
{
    public Material TileMaterial; //material used to represent object on map
    public void Show() //show object on map
    {
        gameObject.GetComponent<Renderer>().material = TileMaterial;
        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
