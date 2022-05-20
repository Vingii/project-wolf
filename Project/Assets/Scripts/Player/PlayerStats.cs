using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour //handles non-persistent player stats
{
    public UpdateInfo updateInfo; //UI script
    Guns activeGun; //currently held gun
    bool hasGoldKey;
    bool hasSilverKey;
    private float reloadTimer; //time left until gun cools down
    public bool CanFire { get { return reloadTimer <= 0; } }
    public Guns ActiveGun { get { return activeGun; } set { activeGun = value; updateInfo.UpdateGun(); } }
    public bool HasGoldKey { get { return hasGoldKey; } set { hasGoldKey = value; updateInfo.UpdateKeys(); } }
    public bool HasSilverKey { get { return hasSilverKey; } set { hasSilverKey = value; updateInfo.UpdateKeys(); } }
    public float ReloadTimeLeft()
    {
        return (reloadTimer > 0) ? (reloadTimer) : 0;
    }
    public void AddReload(float dtime)
    {
        reloadTimer += dtime;
    }
    void Start()
    {
        ActiveGun = Guns.Pistol;
        HasGoldKey = false;
        HasSilverKey = false;
    }
    void Update()
    {
        if (reloadTimer < 0)
        {
            reloadTimer = 0;
        }
        if (!CanFire)
        {
            reloadTimer -= Time.deltaTime;
        }
    }
}
