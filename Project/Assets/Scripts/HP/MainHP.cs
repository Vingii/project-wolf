using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainHP : MonoBehaviour //abstract health handler
{
    public abstract void TakeDamage(int damage); //execute when hit
    public abstract void Die(); //execute on 0 hp
}
