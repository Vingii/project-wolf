using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionHandle : MonoBehaviour
{
    public GameObject Controller; //controller prefab
    public GameObject ControllerGroup; //parent object of controllers
    public void GenerateControllers(int leveldim, int blockdim) //create controllers on grid
    {
        for (int i = 0; i < leveldim / blockdim; i++)
        {
            for (int j = 0; j < leveldim / blockdim; j++)
            {
                Vector3 pos = new Vector3(2 * blockdim + i * blockdim * 4, 0, 2 * blockdim + j * blockdim * 4);
                Instantiate(Controller, pos, Quaternion.identity, ControllerGroup.transform);
            }
        }
    }
    void OccludeObject(GameObject obj, GameObject controller) //add obj to controller's ocludees
    {
        controller.GetComponent<Occluder>().AddOccludee(obj);
    }
    public void OccludeObject(GameObject obj) //find nearest controller and add obj to ocludees
    {
        GameObject controller = null;
        for (int i = 0; i < ControllerGroup.transform.childCount; i++)
        {
            if (controller == null ||
                Vector3.Distance(obj.transform.position, controller.transform.position) >
                Vector3.Distance(obj.transform.position, ControllerGroup.transform.GetChild(i).transform.position))
            {
                controller = ControllerGroup.transform.GetChild(i).gameObject;
            }
        }
        if (controller != null)
        {
            OccludeObject(obj, controller);
        }
    }
}
