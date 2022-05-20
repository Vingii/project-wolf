using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorInteract : MainInteract
{
    public Material onmat; //material of triggered elevator
    ScreenHandle screenHandle; //handler script
    void Start()
    {
        screenHandle = GameObject.FindWithTag("Handler").GetComponent<ScreenHandle>();
    }
    public override void Interact(Vector3 norm)
    {
        gameObject.GetComponent<MeshRenderer>().material = onmat;
        screenHandle.FinishLevel();
    }
}
