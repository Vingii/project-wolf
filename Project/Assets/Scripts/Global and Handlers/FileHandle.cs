using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class FileHandle //STATIC handles savefile and leaderboards file management
{
    public static bool Loaded; //used by GenerateLevel to know if it should get data from savegame.dat
    public static void Save()
    {
        //saves current state to savegame.dat
        using (BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + "/savegame.dat", FileMode.Create)))
        {
            writer.Write(GlobalStats.Level);
            writer.Write(GlobalStats.Score);
            writer.Write(GlobalStats.Lives);
            writer.Write(GlobalStats.Health);
            writer.Write(GlobalStats.Ammo);
            writer.Write(GlobalStats.Currseed);
            writer.Write(GlobalStats.HasGun[0]);
            writer.Write(GlobalStats.HasGun[1]);
            writer.Write(GlobalStats.HasGun[2]);
            writer.Write(GlobalStats.HasGun[3]);
        }
    }
    public static void Load() //retreives data from savegame.dat and updates gamestate
    {
        if (!File.Exists(Application.persistentDataPath + "/savegame.dat")) //if there is no file, assume new game
        {
            GlobalStats.Initialize();
            Loaded = false;
            return;
        }
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/savegame.dat", FileMode.Open)))
            {
                GlobalStats.Level = reader.ReadInt32();
                GlobalStats.Score = reader.ReadInt32();
                GlobalStats.Lives = reader.ReadInt32();
                GlobalStats.Health = reader.ReadInt32();
                GlobalStats.Ammo = reader.ReadInt32();
                GlobalStats.Currseed = reader.ReadInt32();
                GlobalStats.HasGun[0] = reader.ReadBoolean();
                GlobalStats.HasGun[1] = reader.ReadBoolean();
                GlobalStats.HasGun[2] = reader.ReadBoolean();
                GlobalStats.HasGun[3] = reader.ReadBoolean();
            }
        }
        catch (EndOfStreamException)
        {
            GlobalStats.Initialize(); //if an error is found, assume new game
        }
        Loaded = false;
    }
    public static void DeleteSave() //removes savegame
    {
        File.Delete(Application.persistentDataPath + "/savegame.dat");
    }

    public static void LeaderboardsAdd(int limit) //adds current score and floor to leadeboards.dat if it belongs to top 'limit' runs recorded
    {
        //writes to leaderboards.dat_new while reading from leaderboards.dat, then renames
        using (BinaryWriter writer = new BinaryWriter(File.Open(Application.persistentDataPath + "/leaderboards.dat_new", FileMode.Create)))
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/leaderboards.dat", FileMode.OpenOrCreate)))
            {
                bool created = true; //if a new file was created
                bool holding = false; //if we've read an entry we haven't written yet
                int sc = 0; //score
                int fl = 0; //floor
                int i = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length && i < limit) //read first limit entries or until end of file
                {
                    created = false;
                    sc = reader.ReadInt32();
                    if (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        fl = reader.ReadInt32();
                    }
                    else
                    {
                        return; //LeaderboardsShow will display error
                    }
                    if ((GlobalStats.Score > sc) || (GlobalStats.Score == sc && GlobalStats.Level > fl))
                    {
                        holding = true;
                        break; //found where our score belongs
                    }
                    else
                    {
                        writer.Write(sc);
                        writer.Write(fl);
                    }
                    i++;
                }
                if (i < limit)
                {
                    writer.Write(GlobalStats.Score); //write our score
                    writer.Write(GlobalStats.Level);
                    i++;
                    if (!created && i < limit && holding) //write the score we're holding
                    {
                        writer.Write(sc);
                        writer.Write(fl);
                        i++;
                    }
                    while ((reader.BaseStream.Position != reader.BaseStream.Length) && i < limit) //continue writing until limit has been reached or eof
                    {
                        writer.Write(reader.ReadInt32());
                        if (reader.BaseStream.Position != reader.BaseStream.Length)
                        {
                            writer.Write(reader.ReadInt32());
                        }
                        else
                        {
                            //LeaderboardsShow will display error
                            return;
                        }
                        i++;
                    }
                }
            }
        }
        File.Delete(Application.persistentDataPath + "/leaderboards.dat"); //move .dat_new to .dat
        File.Move(Application.persistentDataPath + "/leaderboards.dat_new", Application.persistentDataPath + "/leaderboards.dat");
    }
    public static void LeaderboardsClear() //remove leaderboards
    {
        File.Delete(Application.persistentDataPath + "/leaderboards.dat");
    }
    public static string[] LeaderboardsShow(int boardlen) //draw leaderboards
    {
        string[] retarr = new string[2 * boardlen];
        if (!File.Exists(Application.persistentDataPath + "/leaderboards.dat"))
        {
            return retarr; //return empty if there is no data
        }
        try
        {
            using (BinaryReader reader = new BinaryReader(File.Open(Application.persistentDataPath + "/leaderboards.dat", FileMode.Open)))
            {
                int i = 0;
                while (reader.BaseStream.Position != reader.BaseStream.Length && i < boardlen)
                {
                    retarr[2 * i] = reader.ReadInt32().ToString(); //left column
                    retarr[2 * i + 1] = reader.ReadInt32().ToString(); //right column
                    i++;
                }
                reader.Close();
            }
        }
        catch (EndOfStreamException) //return invalid if there is error anywhere
        {
            for (int i = 0; i < 2 * boardlen; i++)
            {
                retarr[i] = "";
            }
            retarr[0] = "Invalid";
            retarr[1] = "file";
            retarr[2] = "Please";
            retarr[3] = "restart";
            retarr[4] = "leader";
            retarr[5] = "boards";
        }
        return retarr;
    }
}
