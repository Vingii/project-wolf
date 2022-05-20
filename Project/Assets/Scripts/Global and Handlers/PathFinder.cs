using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapCell //node of heap
{
    public float dist;
    public int x;
    public int y;
}
public class Heap //distance based minimum heap in array
{
    int lengthMax; //array length
    int length; //currently used length
    HeapCell[] heap;
    public Heap(int lengthMax)
    {
        this.lengthMax = lengthMax;
        length = 0;
        heap = new HeapCell[lengthMax + 1];
        for (int i = 1; i <= lengthMax; i++)
        {
            heap[i] = new HeapCell();
        }
    }
    public void ToHeap(float dist, int x, int y) //add node to heap
    {
        if (length + 1 < lengthMax)
        {
            length += 1;
            heap[length].dist = dist;
            heap[length].x = x;
            heap[length].y = y;
            int i = length;

            while ((i != 1) && (heap[i].dist < heap[i / 2].dist))
            {
                (heap[i], heap[i / 2]) = (heap[i / 2], heap[i]);
                i /= 2;
            }
        }
    }
    public bool FromHeap(ref int x, ref int y) //get minimum from heap
    {
        if (length == 0)
        {
            return false;
        }
        (x, y) = (heap[1].x, heap[1].y);
        (heap[1], heap[length]) = (heap[length], heap[1]);
        length -= 1;
        int i = 1;
        while (((2 * i <= length) && (heap[2 * i].dist < heap[i].dist)) || ((2 * i + 1 <= length) && (heap[2 * i + 1].dist < heap[i].dist)))
        {
            if (heap[2 * i].dist < heap[i].dist)
            {
                (heap[i], heap[2 * i]) = (heap[2 * i], heap[i]);
                i = 2 * i;
            }
            else
            {
                (heap[i], heap[2 * i + 1]) = (heap[2 * i + 1], heap[i]);
                i = 2 * i + 1;
            }
        }
        return true;
    }
    public void ClearHeap()
    {
        length = 0;
    }
}
public class PathFinder : MonoBehaviour //can find shortest path using Theta*
{
    public float keepdist; //set in editor, distance entities should keep from wall when finding path

    Heap heap;
    LayerMask layerMask; //used when checking for visibility
    bool[,] passable; //if a tile can be walked through
    int dim;
    int Dim
    {
        get
        {
            if (dim == 0)
            {
                dim = GlobalStats.handler.GetComponent<GenerateLevel>().Leveldim;
            }
            return dim;
        }
        set { dim = value; }
    }
    void Start()
    {
        Dim = GlobalStats.handler.GetComponent<GenerateLevel>().Leveldim;
        heap = new Heap(4 * Dim * Dim); //heap adding doesnt check for duplicates
        layerMask = LayerMask.GetMask("Wall", "Environment");
        if (passable == null)
        {
            passable = new bool[Dim, Dim];
            for (int i = 0; i < Dim; i++)
            {
                for (int j = 0; j < Dim; j++)
                {
                    passable[i, j] = true;
                }
            }
        }
    }

    public void AddImpassable(int x, int y) //adds an impassable tile to passable[x,y]
    {
        if (passable == null)
        {
            passable = new bool[Dim, Dim];
            for (int i = 0; i < Dim; i++)
            {
                for (int j = 0; j < Dim; j++)
                {
                    passable[i, j] = true;
                }
            }
        }
        if ((x >= 0) && (x < Dim) && (y >= 0) && (y < Dim))
        {
            passable[x, y] = false;
        }
    }
    bool IsPassable(int x, int y) //returns value from passable[x,y]
    {
        if ((x >= 0) && (x < Dim) && (y >= 0) && (y < Dim) && passable != null)
        {
            return passable[x, y];
        }
        else
        {
            return false;
        }
    }
    bool Unobstructed(Vector3 from, Vector3 to) //if there is a clear path from from to to
    {
        Vector3 perv = (Quaternion.Euler(0, 90, 0) * (from - to)).normalized; //to keep distance from wall
        return !(
            Physics.Raycast(from + perv * keepdist, to - from, Vector3.Distance(to, from), layerMask)
            ||
            Physics.Raycast(from, to - from, Vector3.Distance(to, from), layerMask)
            ||
            Physics.Raycast(from - perv * keepdist, to - from, Vector3.Distance(to, from), layerMask)
            );
    }
    public Vector3 FindPath(Vector3 from3, Vector3 to3)
    //Theta*
    {
        //INIT
        Vector2Int from = new Vector2Int(Mathf.RoundToInt(from3.x / 4), Mathf.RoundToInt(from3.z / 4));
        Vector2Int to = new Vector2Int(Mathf.RoundToInt(to3.x / 4), Mathf.RoundToInt(to3.z / 4));
        int tox = to.x;
        int toy = to.y;
        int fromx = from.x;
        int fromy = from.y;
        Vector2Int[,] parent = new Vector2Int[Dim, Dim];
        float[,] shortest = new float[Dim, Dim];
        float[,] dist = new float[Dim, Dim];
        bool[,] closed = new bool[Dim, Dim];
        for (int i = 0; i < Dim; i++)
        {
            for (int j = 0; j < Dim; j++)
            {
                shortest[i, j] = 100 * Dim; //infinity
                dist[i, j] = 100 * Dim;
            }
        }
        if (from.x >= Dim || from.y >= Dim || to.x >= Dim || to.y >= Dim || from.x < 0 || from.y < 0 || to.x < 0 || to.y < 0)
        {
            return from3;
        }
        //FIRST NODE
        int currx = from.x;
        int curry = from.y;
        shortest[currx, curry] = 0;
        dist[currx, curry] = Mathf.Sqrt((tox - currx) * (tox - currx) + (toy - curry) * (toy - curry));
        parent[currx, curry] = new Vector2Int(currx, curry);
        //NODE LOOP
        while ((currx, curry) != (tox, toy))
        {
            if (currx >= 0 && currx < Dim && curry >= 0 && curry < Dim && !closed[currx, curry])
            {
                closed[currx, curry] = true;
                int parx = parent[currx, curry].x;
                int pary = parent[currx, curry].y;
                //CHECK NEIGHBOURS
                for (int i = 0; i <= 1; i++)
                {
                    for (int j = 0; j <= 1; j++)
                    {
                        int neix = currx + (i - 2 * j * i); //cycle through neighbours
                        int neiy = curry + ((1 - i) - 2 * (1 - j) * (1 - i));
                        //direct line between parent of curr and curr
                        if (neix >= 0 && neix < Dim && neiy >= 0 && neiy < Dim
                            && IsPassable(neix, neiy) && !closed[neix, neiy])
                        {
                            if (Unobstructed(new Vector3(4 * neix, 0, 4 * neiy), new Vector3(4 * parx, 0, 4 * pary))
                               &&
                               shortest[parx, pary] + Vector2Int.Distance(new Vector2Int(parx, pary), new Vector2Int(neix, neiy)) < shortest[neix, neiy])
                            {
                                shortest[neix, neiy] = shortest[parx, pary] +
                                    Vector2Int.Distance(new Vector2Int(parx, pary), new Vector2Int(neix, neiy));
                                parent[neix, neiy] = new Vector2Int(parx, pary);
                                dist[neix, neiy] = shortest[neix, neiy] +
                                        Mathf.Sqrt((tox - neix) * (tox - neix) + (toy - neiy) * (toy - neiy));
                                heap.ToHeap(dist[neix, neiy], neix, neiy);
                            }
                            //else just do A*
                            else if (shortest[currx, curry] + 1 < shortest[neix, neiy])
                            {
                                shortest[neix, neiy] = shortest[currx, curry] + 1;
                                parent[neix, neiy] = new Vector2Int(currx, curry);
                                dist[neix, neiy] = shortest[neix, neiy] +
                                        Mathf.Sqrt((tox - neix) * (tox - neix) + (toy - neiy) * (toy - neiy));
                                heap.ToHeap(dist[neix, neiy], neix, neiy);
                            }
                        }
                    }
                }
            }
            if (!heap.FromHeap(ref currx, ref curry)) //pulls new curr
            {
                return from3;
            }
        }
        heap.ClearHeap();
        //FIND FURTHEST REACHABLE NODE ON PATH
        while (!Unobstructed(new Vector3(currx, 0, curry) * 4, from3) && (currx, curry) != (fromx, fromy))
        {
            (currx, curry) = (parent[currx, curry].x, parent[currx, curry].y);
        }
        return new Vector3(currx, 0, curry) * 4;
    }
}
