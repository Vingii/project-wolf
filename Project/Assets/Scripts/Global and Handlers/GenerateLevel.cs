using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;

public class GenerateLevel : MonoBehaviour //handler level creation, includes the only Awake function
{
    OcclusionHandle occHandle; //script which handles 'occlusion culling'

    public TextMeshProUGUI SeedText; //text in UI which displays current seed
    public GameObject Player; //player instance
    public GameObject Wallgroup; //groups are parent objects of createdobj instances
    public GameObject Doorgroup;
    public GameObject Pickupgroup;
    public GameObject Enemygroup;
    public GameObject Environmentalgroup;
    public Material WoodMatBrown; //wall materials
    public Material BrickMatGray;
    public Material BrickMatBlue;

    public GameObject Wall; //w+color
    public GameObject SecretWall; //W+color
    public GameObject Door; //d+rot
    public GameObject GoldDoor; //Dg+rot
    public GameObject SilverDoor; //Ds+rot
    public GameObject ElevatorDoor; //De+rot
    public GameObject Elevator; //l+rot

    public GameObject Ammo; //pa+value
    public GameObject Health; //ph+value
    public GameObject Score; //pp+value
    public GameObject OneUp; //pu
    public GameObject GoldKey; //pkg
    public GameObject SilverKey; //pks
    public GameObject MachineGun; //pgm
    public GameObject ChainGun; //pgc

    public GameObject EnemyBasic; //eb+rot
    public GameObject EnemyDog; //ed+rot

    public GameObject Table; //nt
    public GameObject CeilLight; //nc
    public GameObject Bars; //+b
    public GameObject Flower; //nf
    public GameObject Pot; //np
    public GameObject Picture; //+p
    public GameObject Flag; //+f
    public GameObject Chandelier; //nC
    public GameObject Lamp; //nl

    int leveldim; //length of counted in wall lengths
    public int Leveldim { get { return leveldim; } set { leveldim = value; } }

    class TileNode //node in TileTree (binary tree with 2 values)
    {
        public string tilepath; //name of the folder
        public int rot; //rotation in multiples of 90 degrees clockwise
        public TileNode f; //left node (if wall isnt present)
        public TileNode t; //right node (if wall is present)
    }
    static class TileTree //binary tree
    //leaves hold shape and rotation corresponding to the path from root
    //t choice in 1st path step corresponds to an existing wall north
    //f choice in 2nd path step corresponds to no wall east and so on
    {
        static public TileNode root;
        static TileTree() //constructs the whole tree from the bottom
        {
            TileNode[] nodearr = new TileNode[16];
            for (int k = 0; k < 16; k++)
            {
                nodearr[k] = new TileNode();
            }

            string d = "DeadEnd/";
            string i = "IShape/";
            string l = "LShape/";
            string x = "XShape/";
            string t = "TShape/";

            string[] paths = { x, t, t, l, t, i, l, d, t, l, i, d, l, d, d, "" };
            int[] rots = { 0, 1, 2, 2, 3, 1, 3, 3, 0, 1, 0, 2, 0, 1, 0, 0 };
            for (int j = 0; j < 16; j++)
            {
                nodearr[j].tilepath = paths[j];
                nodearr[j].rot = rots[j];
            }

            for (int j = 16; j > 1; j /= 2) //connect up the tree
            {
                int jh = j / 2;
                for (int k = j - 1; k >= jh; k--)
                {
                    TileNode cach = nodearr[k - jh];
                    nodearr[k - jh] = new TileNode();
                    nodearr[k - jh].f = cach;
                    nodearr[k - jh].t = nodearr[k];
                }
            }
            root = nodearr[0];
        }
    }

    Quaternion SetOrientation(string inp) //set rotation according to its char representation
    {
        switch (inp)
        {
            case "r":
                return Quaternion.Euler(0, 0, 0);
            case "d":
                return Quaternion.Euler(0, 90, 0);
            case "l":
                return Quaternion.Euler(0, 180, 0);
            case "u":
                return Quaternion.Euler(0, 270, 0);
            default:
                return Quaternion.Euler(0, 0, 0);
        }
    }
    private void Awake() //executes before any Start()
    {
        //Initialization has to be in this order
        GlobalStats.handler = GameObject.FindGameObjectWithTag("Handler"); //cache handler globally
        GlobalStats.updateInfo = GameObject.FindWithTag("Player").GetComponent<PlayerStats>().updateInfo;
        GlobalStats.updateInfo.UpdateAll();
        occHandle = GlobalStats.handler.GetComponent<OcclusionHandle>();
        if (FileHandle.Loaded)
        {
            FileHandle.Load();
            FileHandle.Loaded = false;
        }
        if (!GlobalStats.DebugModePersistent)
        {
            FileHandle.Save();
        }
        GlobalStats.Level += 1;
        SeedText.text = "[Seed: " + GlobalStats.Currseed.ToString() + "]";
        int levelcount = int.Parse(((TextAsset)Resources.Load("Levels/count")).text);
        if (GlobalStats.Level <= levelcount) //generate firts levelcount levels from complete files
        {
            GenerateFromFile("level" + levelcount);
        }
        else //generate remaining levels procedurally
        {
            int seed = GlobalStats.Currseed;
            for (int i = 0; i < GlobalStats.Level; i++) //nonlinear transformation so that floor 2 of seed 100 != floor 3 of seed 99
            {
                seed = (seed >> 1) | (seed << 31);
                seed += 1;
            }
            GenerateFromSeed(seed);
        }
    }

    enum Direction { none, up, down, left, right };
    void randomWalk(int i, int j, bool[,,] walls, bool[,] visited) //completes 1 randomwalk for maze-generating alg.
    {
        (int origi, int origj) = (i, j);
        bool success;
        Direction[,] previous = new Direction[visited.Length, visited.Length];
        Direction dir;
        while (true)
        {
            success = false; //chose a valid direction
            dir = (Direction)(Random.Range(1, 5));
            while (dir == previous[i, j]) //dont return
            {
                dir = (Direction)(Random.Range(1, 5));
            }
            switch (dir)
            {
                case Direction.up:
                    if (j > 0)
                    {
                        success = true;
                        j -= 1;
                    }
                    break;
                case Direction.down:
                    if (j < visited.GetLength(0) - 2)
                    {
                        success = true;
                        j += 1;
                    }
                    break;
                case Direction.left:
                    if (i > 0)
                    {
                        success = true;
                        i -= 1;
                    }
                    break;
                case Direction.right:
                    if (i < visited.GetLength(0) - 2)
                    {
                        success = true;
                        i += 1;
                    }
                    break;
                default:
                    break;
            }
            if (success)
            {
                if (previous[i, j] != Direction.none || (origi, origj) == (i, j)) //cut path if we made a loop
                {
                    (int endi, int endj) = (i, j);
                    switch (dir) //return 1 step
                    {
                        case Direction.up:
                            j += 1;
                            break;
                        case Direction.down:
                            j -= 1;
                            break;
                        case Direction.left:
                            i += 1;
                            break;
                        case Direction.right:
                            i -= 1;
                            break;
                        default:
                            break;
                    }
                    while (endi != i || endj != j) //cut path
                    {
                        switch (previous[i, j])
                        {
                            case Direction.up:
                                previous[i, j] = Direction.none;
                                j -= 1;
                                break;
                            case Direction.down:
                                previous[i, j] = Direction.none;
                                j += 1;
                                break;
                            case Direction.left:
                                previous[i, j] = Direction.none;
                                i -= 1;
                                break;
                            case Direction.right:
                                previous[i, j] = Direction.none;
                                i += 1;
                                break;
                            default:
                                break;
                        }
                    }
                }
                else //valid move
                {
                    switch (dir)
                    {
                        case Direction.up:
                            previous[i, j] = Direction.down;
                            break;
                        case Direction.down:
                            previous[i, j] = Direction.up;
                            break;
                        case Direction.left:
                            previous[i, j] = Direction.right;
                            break;
                        case Direction.right:
                            previous[i, j] = Direction.left;
                            break;
                        default:
                            break;
                    }
                    if (visited[i, j]) //return to original maze
                    {
                        while (origi != i || origj != j) //trace back and break walls
                        {
                            switch (previous[i, j])
                            {
                                case Direction.up:
                                    j -= 1;
                                    walls[i, j, 1] = false;
                                    break;
                                case Direction.down:
                                    walls[i, j, 1] = false;
                                    j += 1;
                                    break;
                                case Direction.left:
                                    i -= 1;
                                    walls[i, j, 0] = false;
                                    break;
                                case Direction.right:
                                    walls[i, j, 0] = false;
                                    i += 1;
                                    break;
                                default:
                                    break;
                            }
                            visited[i, j] = true;
                        }
                        return;
                    }
                }
            }
        }
    }
    bool[,,] generateGrid(int dim) //generates wall[,,], [i,j,0] wall from [i,j] going right, [i,j,1] going down
                                   //generates maze-like grid using Wilson's alg.
    {
        bool[,,] walls = new bool[dim, dim, 2]; //returning array
        bool[,] visited = new bool[dim, dim]; //if a node is already a part of maze (Wilson)
        visited[0, 0] = true;
        for (int i = 0; i < dim; i++) for (int j = 0; j < dim; j++)
                (walls[i, j, 0], walls[i, j, 1]) = (true, true); //assume all walls
        for (int i = 0; i < dim; i++) for (int j = 0; j < dim; j++)
            {
                if (!visited[i, j])
                {
                    randomWalk(i, j, walls, visited);
                }
            }
        return walls;
    }

    (string tilepath, int rot) FindShape(bool[] walls) //return tile directory and rotation based on adjacent walls (sorted clockwise starting north) by TileTree traversal
    {
        TileNode currnode = TileTree.root;
        for (int i = 0; i < 4; i++) currnode = walls[i] ? currnode.t : currnode.f;
        (int rot, string path) = (currnode.rot, currnode.tilepath);
        //RANDOMLY ROTATE ISHAPE, XSHAPE
        if (currnode.tilepath == "XShape/")
        {
            currnode.rot = Random.Range(0, 4);
        }
        if (currnode.tilepath == "IShape/" && Random.Range(0, 0.5f) > 0.5f)
        {
            rot = (rot + 2) % 4;
        }

        return ("LevelTiles/" + path, rot);
    }
    void fillGrid(bool[,,] walls, int blockdim) //generates objects based on level layout
    {
        (int starti, int startj) = (Random.Range(0, walls.GetLength(0)), Random.Range(0, walls.GetLength(0)));
        int enddiff = Random.Range(-walls.GetLength(0) / 2, walls.GetLength(0) / 2 + 1); //end offset from start in x dim
        int enddir = Random.Range(0, 2) == 1 ? 1 : -1;
        (int endi, int endj) =
            ((starti + enddiff) % walls.GetLength(0),
            (startj + enddir * (walls.GetLength(0) / 2 - enddiff)) % walls.GetLength(0));
        if (endi < 0)//% of negatives is wrong
        {
            endi += walls.GetLength(0);
        }
        if (endj < 0)
        {
            endj += walls.GetLength(0);
        }

        for (int i = 0; i < walls.GetLength(0); i++)
        {
            for (int j = 0; j < walls.GetLength(0); j++)
            {
                (string tilepath, int tilerot) = FindShape(new bool[] { (j > 0 ? walls[i, j - 1, 1] : true), walls[i, j, 0], walls[i, j, 1], (i > 0 ? walls[i - 1, j, 0] : true) });
                //CHOOSE TILE
                string tilename;
                if ((starti, startj) == (i, j))
                {
                    tilename = "start";
                    Quaternion prot = Quaternion.identity;
                    for (int r = 0; r < tilerot; r++)
                    {
                        prot *= Quaternion.Euler(0, 90, 0);
                    }
                    Player.transform.SetPositionAndRotation(
                        new Vector3(4 * j * blockdim + 2 * blockdim - 2, 0, 4 * i * blockdim + 2 * blockdim - 2),
                        prot);
                }
                else if ((endi, endj) == (i, j))
                {
                    tilename = "end";
                }
                else
                {
                    tilename = Random.Range(1, 1 + int.Parse(((TextAsset)Resources.Load(tilepath + "count")).text)).ToString();
                }
                //GENERATE OBJECTS
                char[] splitters = new char[1];
                splitters[0] = ' ';
                TextAsset tiledata = (TextAsset)Resources.Load(tilepath + tilename);
                string tilestring = tiledata.text;
                StringReader reader = new StringReader(tilestring);
                Vector3Int pos = new Vector3Int();
                Quaternion rot = Quaternion.identity;
                for (int r = 0; r < tilerot; r++)
                {
                    rot *= Quaternion.Euler(0, 90, 0);
                }
                for (int k = 0; k < blockdim; k++)
                {
                    string line = reader.ReadLine();
                    string[] linestrings = line.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
                    for (int l = 0; l < blockdim; l++)
                    {
                        pos.Set(4 * j * blockdim, 0, 4 * i * blockdim);
                        switch (tilerot) //determine position offset by tilerot
                        {
                            case 0:
                                pos += new Vector3Int(4 * k, 0, 4 * l);
                                break;
                            case 1:
                                pos += new Vector3Int(4 * l, 0, 4 * (blockdim - 1 - k));
                                break;
                            case 2:
                                pos += new Vector3Int(4 * (blockdim - 1 - k), 0, 4 * (blockdim - 1 - l));
                                break;
                            case 3:
                                pos += new Vector3Int(4 * (blockdim - 1 - l), 0, 4 * k);
                                break;
                            default:
                                break;
                        }
                        CreateObj(linestrings[l], pos, rot);
                    }
                }
            }
        }
    }

    void GenerateFromSeed(int seed) //generates all objects in level based on run seed
    {
        Random.InitState(seed);
        Leveldim = 75;
        int blockdim = 15;
        //[i,j,0] wall from [i,j] going right, [i,j,1] going down
        bool[,,] walls = generateGrid(Leveldim / blockdim);
        occHandle.GenerateControllers(Leveldim, blockdim);
        fillGrid(walls, blockdim);
    }
    void GenerateFromFile(string levelName) //generates all objects in level based on a text file
    {
        char[] splitters = new char[1]; //loading level layout
        splitters[0] = ' ';
        TextAsset leveldata = (TextAsset)Resources.Load("Levels/" + levelName);
        string levelstring = leveldata.text;
        StringReader reader = new StringReader(levelstring);
        Vector3Int pos = new Vector3Int();
        Quaternion rot = Quaternion.identity;
        int i = 0;

        string line = reader.ReadLine();
        string[] linestrings = line.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        leveldim = linestrings.Length;
        while (line != "end")
        {
            if (i > 0)
            {
                linestrings = line.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
            }
            for (int j = 0; j < leveldim; j++)
            {
                pos.Set(4 * i, 0, 4 * j);
                CreateObj(linestrings[j], pos, rot);
            }
            i++;
            line = reader.ReadLine();
        }
        line = reader.ReadLine();
        linestrings = line.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
        pos.Set(4 * int.Parse(linestrings[0]), 0, 4 * int.Parse(linestrings[1]));
        rot = Quaternion.Euler(0, float.Parse(linestrings[2]), 0);
        Player.transform.SetPositionAndRotation(pos, rot);
    }
    void CreateObj(string s, Vector3Int pos, Quaternion rot) //creates object with key s
    {
        GameObject newobj = null;
        switch (s.Substring(0, 1).ToLower())
        {
            case "w": //walls
                if (s.Substring(0, 1) == "w") //default walls
                {
                    GlobalStats.handler.GetComponent<PathFinder>().AddImpassable(pos.x / 4, pos.z / 4);
                    newobj = Instantiate(Wall, pos, rot, Wallgroup.transform);
                }
                else //secret walls 
                {
                    newobj = Instantiate(SecretWall, pos, rot, Wallgroup.transform);
                }
                switch (s.Substring(1, 1)) //set material
                {
                    case "l":
                        newobj.GetComponent<MeshRenderer>().material = BrickMatBlue;
                        newobj.GetComponentInChildren<AddTileQuad>().TileMaterial = BrickMatBlue;
                        break;
                    case "b":
                        newobj.GetComponent<MeshRenderer>().material = WoodMatBrown;
                        newobj.GetComponentInChildren<AddTileQuad>().TileMaterial = WoodMatBrown;
                        break;
                    case "g":
                        newobj.GetComponent<MeshRenderer>().material = BrickMatGray;
                        newobj.GetComponentInChildren<AddTileQuad>().TileMaterial = BrickMatGray;
                        break;
                    default:
                        break;
                }
                if (s.Length >= 3) switch (s.Substring(2, 1)) //set decoration
                    {
                        case "b":
                            Instantiate(Bars, pos, rot, newobj.transform);
                            break;
                        case "p":
                            Instantiate(Picture, pos, rot, newobj.transform);
                            break;
                        case "f":
                            Instantiate(Flag, pos, rot, newobj.transform);
                            break;
                        default:
                            break;
                    }
                break;
            case "d": //doors
                if (s.Substring(0, 1) == "d") //lowercase normal doors
                {
                    rot *= SetOrientation(s.Substring(1, 1));
                    newobj = Instantiate(Door, pos, rot, Doorgroup.transform);
                }
                else //uppercase special doors
                {
                    rot *= SetOrientation(s.Substring(2, 1));
                    switch (s.Substring(1, 1))
                    {
                        case "e":
                            newobj = Instantiate(ElevatorDoor, pos, rot, Doorgroup.transform);
                            break;
                        case "g":
                            newobj = Instantiate(GoldDoor, pos, rot, Doorgroup.transform);
                            break;
                        case "s":
                            newobj = Instantiate(SilverDoor, pos, rot, Doorgroup.transform);
                            break;
                        default:
                            break;
                    }
                }
                break;
            case "p": //pickups
                switch (s.Substring(1, 1))
                {
                    case "a":
                        newobj = Instantiate(Ammo, pos, rot, Pickupgroup.transform);
                        newobj.GetComponent<AmmoPickup>().Ammo = int.Parse(s.Substring(2, s.Length - 2));
                        break;
                    case "h":
                        newobj = Instantiate(Health, pos, rot, Pickupgroup.transform);
                        newobj.GetComponent<HealthPickup>().Health = int.Parse(s.Substring(2, s.Length - 2)) * 10;
                        break;
                    case "p":
                        newobj = Instantiate(Score, pos, rot, Pickupgroup.transform);
                        newobj.GetComponent<ScorePickup>().Score = int.Parse(s.Substring(2, s.Length - 2)) * 100;
                        break;
                    case "P":
                        newobj = Instantiate(Score, pos, rot, Pickupgroup.transform);
                        newobj.GetComponent<ScorePickup>().Score = int.Parse(s.Substring(2, s.Length - 2)) * 1000;
                        break;
                    case "u":
                        newobj = Instantiate(OneUp, pos, rot, Pickupgroup.transform);
                        break;
                    case "k":
                        switch (s.Substring(2, 1))
                        {
                            case "g":
                                newobj = Instantiate(GoldKey, pos, rot, Pickupgroup.transform);
                                break;
                            case "s":
                                newobj = Instantiate(SilverKey, pos, rot, Pickupgroup.transform);
                                break;
                            default:
                                break;
                        }
                        break;
                    case "g":
                        switch (s.Substring(2, 1))
                        {
                            case "m":
                                newobj = Instantiate(MachineGun, pos, rot, Pickupgroup.transform);
                                break;
                            case "c":
                                newobj = Instantiate(ChainGun, pos, rot, Pickupgroup.transform);
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                break;
            case "e": //enemies
                switch (s.Substring(1, 1))
                {
                    case "b":
                        rot *= SetOrientation(s.Substring(2, 1));
                        newobj = Instantiate(EnemyBasic, pos, rot, Enemygroup.transform);
                        break;
                    case "d":
                        rot *= SetOrientation(s.Substring(2, 1));
                        newobj = Instantiate(EnemyDog, pos, rot, Enemygroup.transform);
                        break;
                    default:
                        break;
                }
                break;
            case "l": //elevator
                rot *= SetOrientation(s.Substring(1, 1));
                newobj = Instantiate(Elevator, pos, rot);
                break;
            case "n": //environmentals
                switch (s.Substring(1, 1))
                {
                    case "t":
                        GlobalStats.handler.GetComponent<PathFinder>().AddImpassable(pos.x / 4, pos.z / 4);
                        newobj = Instantiate(Table, pos, rot, Environmentalgroup.transform);
                        break;
                    case "p":
                        GlobalStats.handler.GetComponent<PathFinder>().AddImpassable(pos.x / 4, pos.z / 4);
                        newobj = Instantiate(Pot, pos, rot, Environmentalgroup.transform);
                        break;
                    case "f":
                        GlobalStats.handler.GetComponent<PathFinder>().AddImpassable(pos.x / 4, pos.z / 4);
                        newobj = Instantiate(Flower, pos, rot, Environmentalgroup.transform);
                        break;
                    case "c":
                        newobj = Instantiate(CeilLight, pos, rot, Environmentalgroup.transform);
                        break;
                    case "C":
                        newobj = Instantiate(Chandelier, pos, rot, Environmentalgroup.transform);
                        break;
                    case "l":
                        GlobalStats.handler.GetComponent<PathFinder>().AddImpassable(pos.x / 4, pos.z / 4);
                        newobj = Instantiate(Lamp, pos, rot, Environmentalgroup.transform);
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
        if (newobj != null) //set occlusion
        {
            occHandle.OccludeObject(newobj);
        }
    }
}
