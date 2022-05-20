using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Guns { Knife, Pistol, MachineGun, ChainGun, GodGun }; //ORDER CANT BE CHANGED

public class Gun //holds properties of a gun, there exists 1 gun for every entry in Guns enum
{
    int damage; //damage in HP
    float range; //range in unity units
    float acc; //accuracy in degrees in either direction
    float cooldown; //reload time in seconds
    bool autofire; //if holding fire button shoots
    public Gun(int damage, float range, float acc, float cooldown)
    {
        this.damage = damage;
        this.range = range;
        this.acc = acc;
        this.cooldown = cooldown;
    }
    public int Damage { get { return damage; } }
    public float Range { get { return range; } }
    public float Acc { get { return acc; } }
    public float Cooldown { get { return cooldown; } }
}

public class FireHandle : MonoBehaviour //handles gunfire
{
    public static Gun[] Gunlist = new Gun[5]; //list of guns with their properties

    public void Fire(GameObject source, Guns gunname) //fires a gun gungame from source in the direction its facing
    {
        RaycastHit hit;
        LayerMask layerMask = source.layer == LayerMask.NameToLayer("Player")
            ? ~LayerMask.GetMask("Player", "Environment", "Occlusion", "Minimap", "Pickup")
            : ~LayerMask.GetMask("Enemy", "Environment", "Occlusion", "Minimap", "Pickup");
        Gun gun = Gunlist[(int)gunname];
        //send ray
        if (Physics.Raycast(source.transform.position, Quaternion.Euler(0, Random.Range(-gun.Acc, gun.Acc), 0) * source.transform.forward, out hit, gun.Range, layerMask))
        {
            int dmgreceived = gun.Damage;
            if (source.layer != LayerMask.NameToLayer("Player") && GlobalStats.Level > 1)
            {
                dmgreceived += 5 * (GlobalStats.Level - 2); //scale enemy damage with floor
            }
            //damage falloff with distance
            if (hit.distance > 8)
            {
                dmgreceived = dmgreceived * 2 / 3;
            }
            if (hit.distance > 16)
            {
                dmgreceived = dmgreceived * 2 / 3;
            }
            if ((source.layer == LayerMask.NameToLayer("Player")) && (gunname == Guns.Knife) && hit.collider.gameObject.GetComponent<MainAI>() != null && !hit.collider.gameObject.GetComponent<MainAI>().SeePlayer())
            {
                dmgreceived = 4 * dmgreceived; //higher knife damage on backstab
            }
            if (hit.collider.gameObject.GetComponent<MainHP>() != null)
            {
                hit.collider.gameObject.GetComponent<MainHP>().TakeDamage(dmgreceived);
            }
        }
    }
    void Start()
    {
        Gunlist[0] = new Gun(15, 6, 0, 1f); //knife
        Gunlist[1] = new Gun(15, Mathf.Infinity, 2, 0.4f); //pistol
        Gunlist[2] = new Gun(15, Mathf.Infinity, 5, 0.2f); //machinegun
        Gunlist[3] = new Gun(15, Mathf.Infinity, 10, 0.15f); //chaingun
        Gunlist[4] = new Gun(1000, Mathf.Infinity, 0, 0.05f); //godgun cooldown doesnt matter
    }
}
