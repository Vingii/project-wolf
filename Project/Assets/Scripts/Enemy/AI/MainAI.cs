using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MainAI : MonoBehaviour //takes care of movement and enemy decisionmaking, base script for specific enemy scripts
{
    float raylen = 2; //todo private length of interact rays

    protected GameObject Player; //player instance
    protected FireHandle Firehandle; //firehandle script
    protected PathFinder Pathfinder; //pathfinder script

    protected float reloadTimer; //time left until reloaded
    protected float followTimer; //time left until enemy stops following
    protected Vector3 baseCoords; //spawn coords of enemy
    protected Quaternion baseRot; //default rotation
    protected Vector3 lastKnownCoords; //where was player last seen
    protected Vector3 oldTarget; //place toward which Move was last executed
    protected int lastHP; //HP before getting shot
    protected float followMax; //how long enemy should follow player after losing line of sight
    protected float turnRate;

    protected LayerMask layerMask; //mask used for line of sight check
    protected MainProperties properties; //own properties script
    protected Coroutine moveCopy; //coroutine of movement
    protected bool alerted; //if alertMarker was already shown for this enemy
    protected bool moveCorRunning; //if moveCopy is running

    protected abstract bool WantsToAttack(); //if enemy is confident enough to fire
    protected virtual bool IsAlert() //if enemy is following
    {
        if (followTimer > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public virtual bool SeePlayer() //if enemy is close enough to player to see him
    {
        float trueRange = properties.VisionRange;
        if (IsAlert())
        {
            trueRange *= 2; //increased vision range when alerted
        }
        if ((Vector3.Distance(gameObject.transform.position, Player.transform.position) < trueRange)
            &&
            (Vector3.Angle(gameObject.transform.forward, Player.transform.position - gameObject.transform.position) < properties.VisionAngle)
            )
        {
            return true;
        }
        //longer vision is a narrow angle
        else if ((Vector3.Distance(gameObject.transform.position, Player.transform.position) < trueRange * 5)
            &&
            (Vector3.Angle(gameObject.transform.forward, Player.transform.position - gameObject.transform.position) < properties.VisionAngle / 4)
            )
        {
            return true;
        }
        //visionangle is ignored in close range
        else if (Vector3.Distance(gameObject.transform.position, Player.transform.position) < trueRange / 4)
        {
            return true;
        }
        return false;
    }
    protected bool HasSightLine() //if there is no obstacle
    {
        RaycastHit hit;
        for (int i = -10; i < 10; i++)
        {
            if (Physics.Raycast(gameObject.transform.position, Quaternion.Euler(0, i, 0) * Player.transform.position - gameObject.transform.position, out hit, Mathf.Infinity, layerMask)
            && hit.collider.gameObject == Player)
            {
                return true;
            }
        }
        return false;
    }
    protected IEnumerator MoveTowardCor(Vector3 target)
    //moves toward target position using PathFinder to find directly reachable locations on "shortest" path
    {
        moveCorRunning = true;
        float coltimer = 1;
        while (Vector3.Distance(target, gameObject.transform.position) > 0.3) //dont check for perfect position
        {
            Vector3 subtarget = Pathfinder.FindPath(gameObject.transform.position, target); //directly reachable
            while (Vector3.Distance(subtarget, gameObject.transform.position) > 0.3) //dont check for perfect position
            {
                Vector3 movedir = (subtarget - gameObject.transform.position).normalized;
                RaycastHit hit;
                //rotate
                if (Vector3.Angle(gameObject.transform.forward, movedir) > turnRate) //dont check for perfect rotation
                {
                    int dir = 1;
                    if (Vector3.SignedAngle(gameObject.transform.forward, movedir, Vector3.up) < 0)
                    {
                        dir = -1;
                    }
                    gameObject.transform.Rotate(0, turnRate * dir * Time.deltaTime * 60, 0);
                }
                //open doors
                else if ((Physics.Raycast(transform.position, transform.forward, out hit, raylen, layerMask)) && (hit.collider.gameObject.CompareTag("Interactable")))
                {
                    hit.collider.gameObject.GetComponent<MainInteract>().Interact(hit.normal);
                }
                else //move
                {
                    gameObject.transform.rotation = Quaternion.LookRotation(movedir);
                    //collisions with other enemies
                    if (!Physics.Raycast(transform.position, transform.forward, raylen, LayerMask.GetMask("Enemy"))
                        && coltimer > 0)
                    {
                        coltimer -= Time.deltaTime;
                        gameObject.transform.Translate(movedir * properties.MoveSpeed * Time.deltaTime * 60, Space.World);
                    }
                    else
                    {
                        coltimer = 1;
                    }
                }
                yield return null;
            }
            gameObject.transform.position = subtarget; //correct for imperfect checks
            yield return null;
        }
        gameObject.transform.position = target; //correct for imperfect checks
        moveCorRunning = false;
    }
    protected void MoveToward(Vector3 target) //start new move routine if new target is different enough
    {
        if (Mathf.RoundToInt(target.x * 2) != Mathf.RoundToInt(oldTarget.x * 2) || Mathf.RoundToInt(target.y * 2) != Mathf.RoundToInt(oldTarget.y * 2) || Mathf.RoundToInt(target.z * 2) != Mathf.RoundToInt(oldTarget.z * 2))
        {
            oldTarget = target;
            if (moveCorRunning)
            {
                StopCoroutine(moveCopy);
                moveCorRunning = false;
            }
            moveCopy = StartCoroutine(MoveTowardCor(target));
        }
    }
    protected void Start()
    {
        reloadTimer = 0;
        followTimer = 0;
        baseCoords = gameObject.transform.position;
        oldTarget = baseCoords;
        lastKnownCoords = Vector3.zero;
        properties = gameObject.GetComponent<MainProperties>();
        layerMask = ~LayerMask.GetMask("Environment", "Minimap", "Occlusion", "Pickup");
        Player = GameObject.FindWithTag("Player");
        Firehandle = GlobalStats.handler.GetComponent<FireHandle>();
        Pathfinder = GlobalStats.handler.GetComponent<PathFinder>();
        baseRot = gameObject.transform.rotation;
        lastHP = properties.HP;

    }
    // Update is called once per frame
    protected void Update()
    {
        if (reloadTimer > 0)
        {
            reloadTimer -= Time.deltaTime;
        }
        if (reloadTimer < 0)
        {
            reloadTimer = 0;
        }
        if (followTimer > 0)
        {
            followTimer -= Time.deltaTime;
        }
        if (followTimer < 0)
        {
            followTimer = 0;
        }
        if (lastHP > properties.HP)
        {
            lastKnownCoords = Player.transform.position;
            followTimer = followMax;
        }
        lastHP = properties.HP;

        //disable AI when far from player
        if (Vector3.Distance(gameObject.transform.position, Player.transform.position) < 100 && HasSightLine() && SeePlayer())
        {
            if (!alerted)
            {
                GlobalStats.handler.GetComponent<AlertHandle>().ShowAlert();
                alerted = true;
            }
            lastKnownCoords = Player.transform.position;
            followTimer = followMax;
            if (WantsToAttack())
            {
                if (moveCorRunning)
                {
                    StopCoroutine(moveCopy);
                    moveCorRunning = false;
                    oldTarget = Vector3.zero;
                }
                gameObject.transform.rotation = Quaternion.LookRotation(Player.transform.position - gameObject.transform.position);
                if (reloadTimer == 0)
                {
                    reloadTimer = FireHandle.Gunlist[(int)properties.Gun].Cooldown * 2;
                    Firehandle.Fire(gameObject, properties.Gun);
                }
            }
            else
            {
                MoveToward(lastKnownCoords);
            }
        }
        else if (followTimer > 0) //move to last known position
        {
            MoveToward(lastKnownCoords);
        }
        else if (Vector3.Distance(gameObject.transform.position, baseCoords) > 0.2) //return home
        {
            if (alerted)
            {
                alerted = false;
            }
            MoveToward(baseCoords);
        }
        else //if home, reset
        {
            gameObject.transform.position = baseCoords;
            gameObject.transform.rotation = baseRot;
        }
    }
}