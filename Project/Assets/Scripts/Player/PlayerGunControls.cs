using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGunControls : MainControls //handles changing guns and shooting
{
    private PlayerStats pstats;
    FireHandle fireh; //handler script
    void Start()
    {
        pstats = gameObject.GetComponent<PlayerStats>();
        fireh = GlobalStats.handler.GetComponent<FireHandle>();
    }
    void Update()
    {
        //change weapon
        if (pstats.CanFire) //don't allow weapon changing when reloading
        {
            if (Input.GetButtonDown("Knife") && GlobalStats.HasGun[0])
            {
                pstats.ActiveGun = Guns.Knife;
            }
            if (Input.GetButtonDown("Pistol") && GlobalStats.HasGun[1])
            {
                pstats.ActiveGun = Guns.Pistol;
            }
            if (Input.GetButtonDown("Machine Gun") && GlobalStats.HasGun[2])
            {
                pstats.ActiveGun = Guns.MachineGun;
            }
            if (Input.GetButtonDown("Chain Gun") && GlobalStats.HasGun[3])
            {
                pstats.ActiveGun = Guns.ChainGun;
            }
        }
        //fire
        if (Input.GetButton("Fire") && (pstats.CanFire))
        {
            if (((GlobalStats.Ammo > 0 || GlobalStats.DebugMode) || (pstats.ActiveGun == Guns.Knife)) && ((pstats.ActiveGun > Guns.Pistol) || Input.GetButtonDown("Fire")))
            {
                if (!GlobalStats.DebugMode)
                {
                    fireh.Fire(gameObject, pstats.ActiveGun);
                }
                else
                {
                    fireh.Fire(gameObject, Guns.GodGun);
                }
                pstats.AddReload(FireHandle.Gunlist[(int)pstats.ActiveGun].Cooldown);
                if (pstats.ActiveGun != Guns.Knife && !GlobalStats.DebugMode)
                {
                    GlobalStats.Ammo -= 1;
                }
            }
        }
    }
}
