using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GlobalStats //holds floor-persistent stats and cached handler
{
    public static UpdateInfo updateInfo; //UI update script, set in GenerateLevel
    static int level, score, lives, health, ammo, currseed;
    public static bool[] HasGun;
    public static GameObject handler; //global cached handler, set in GenerateLevel
    static bool debugMode; //if godmode is enabled
    static bool debugModePersistent; //if godmode was ever enabled or seed was set
    public static bool DebugMode
    {
        get { return debugMode; }
        set { debugMode = value; if (value) debugModePersistent = true; }
    }
    public static bool DebugModePersistent
    {
        get { return debugModePersistent; }
        set { debugMode = value; debugModePersistent = value; }
    }
    static GlobalStats()
    {
        HasGun = new bool[4]; //knife, pistol, machine gun, chain gun
    }

    public static void Initialize(int seed = 0) //called on new game, default to no seed set -> create new
    {
        Level = 0;
        Score = 0;
        Lives = 3;
        Health = 100;
        Ammo = 8;
        HasGun[(int)Guns.Knife] = true;
        HasGun[(int)Guns.Pistol] = true;
        if (seed == 0)
        {
            GlobalStats.Currseed = Random.Range(0, int.MaxValue);
            FileHandle.Save();
        }
        else
        {
            debugModePersistent = true;
            GlobalStats.Currseed = seed;
        }
    }

    public static int Level
    {
        get { return level; }
        set { level = value; if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level")) { updateInfo.UpdateLevel(); } }
    }
    public static int Score
    {
        get { return score; }
        set
        {
            if ((value / 40000 > score / 40000) && (score != 0)) Lives += 1;
            score = value;
            if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level"))
            {
                updateInfo.UpdateScore();
            }
        }
    }
    public static int Lives
    {
        get { return lives; }
        set { lives = Mathf.Min(value, 9); if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level")) { updateInfo.UpdateLives(); } }
    }
    public static int Health
    {
        get { return health; }
        set
        {
            health = Mathf.Min(value, 100); if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level")) { updateInfo.UpdateHealth(); }
        }
    }
    public static int Ammo
    {
        get { return ammo; }
        set { ammo = Mathf.Min(value, 99); if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Level")) { updateInfo.UpdateAmmo(); } }
    }
    public static int Currseed
    {
        get { return currseed; }
        set { currseed = value; }
    }
}
