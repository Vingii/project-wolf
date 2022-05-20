using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class MainProperties : MonoBehaviour //handles most enemy values
{
    int hp; //current hp
    int maxhp; //maximum hp
    Guns gun; //equiped gun
    int score; //score increment for kill
    float moveSpeed;
    float visionRange;
    float visionAngle;

    public int HP { get { return hp; } set { hp = value; } }
    public int MaxHP { get { return maxhp; } }
    public Guns Gun { get { return gun; } }
    public int Score { get { return score; } }
    public float MoveSpeed { get { return moveSpeed; } }
    public float VisionRange { get { return visionRange; } }
    public float VisionAngle { get { return visionAngle; } }
    virtual protected void Initialize(int hp, Guns gun, int score, float moveSpeed, float visionRange, float visionAngle)
    {
        this.hp = hp;
        this.maxhp = hp;
        this.gun = gun;
        this.score = score;
        this.moveSpeed = moveSpeed;
        this.visionRange = visionRange;
        this.visionAngle = visionAngle;
    }
    abstract protected void Initialize(); //calls base Init with set parameters
    // Start is called before the first frame update
    protected void Start()
    {
        if (hp == 0) //dont initialize if entity was somehow already initialized
        {
            Initialize();
        }
    }
}
