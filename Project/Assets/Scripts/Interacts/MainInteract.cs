using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainInteract : MonoBehaviour
{
    public bool canInteract = true;
    public abstract void Interact(Vector3 norm);
}
