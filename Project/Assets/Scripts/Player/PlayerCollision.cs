using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour //handles all player collisions using raycasting
{
    public float raylen; //length of collision rays
    RaycastHit hit;
    LayerMask layerMask; //against what colliders we should check collisions
    float sq2; //cached sqrt(2)
    void Start()
    {
        layerMask = ~LayerMask.GetMask("Player", "Occlusion", "Minimap");
        sq2 = Mathf.Sqrt(2);
    }
    void FixedUpdate()
    {
        for (int j = 0; j < 4; j++) //casts 8 rays total creating a square collisionbox
        {
            Vector3 raydir = Quaternion.Euler(0, 90 * j, 0) * Vector3.forward;
            if (Physics.Raycast(transform.position, raydir, out hit, raylen, layerMask, QueryTriggerInteraction.Ignore))
            {
                transform.Translate(raydir * (hit.distance - raylen), Space.World);
            }
            if (Physics.Raycast(transform.position, Quaternion.Euler(0, 45, 0) * raydir, out hit, raylen * sq2, layerMask, QueryTriggerInteraction.Ignore))
            {
                transform.Translate(hit.normal * (raylen - hit.distance / sq2), Space.World);
            }
        }
    }
}
