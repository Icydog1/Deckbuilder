using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    protected PlayerControler playerControler;
    protected TurnManager turnManager;
    protected MapManager mapManager;
    protected Pathfinder pathfinder;
    protected bool isMyTurn;
    protected float distanceToPlayer;
    protected Vector3 relativeHexPosToPlayer;
    protected Vector2 OneToOnePos;
    protected string[] movesets;
    //protected List<string> actionQueue = new List<string>();
    protected string currentMove;


    public int maxHealth, health;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        turnManager.turnOrder.Add(gameObject);
        health = maxHealth;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (turnManager.currentTurn == gameObject)
        {
            GameObject border = transform.Find("Border").gameObject;
            border.GetComponent<SpriteRenderer>().color = Color.white;
        }
        else
        {
            GameObject border = transform.Find("Border").gameObject;
            border.GetComponent<SpriteRenderer>().color = Color.black;
        }

    }

    public void StartOfTurn()
    {
        currentMove = movesets[Random.Range(0, movesets.Length)];
        Invoke(currentMove, 0);
    }

    public void EndTurn()
    {
        turnManager.NextTurn();
    }

    public void Move(int moveValue,int range = 1, bool isJump = false, bool isFly = false)
    {
        OneToOnePos = mapManager.PosToOneToOne(transform.position);
        pathfinder.PathfindTowards(OneToOnePos, playerControler.playerOneToOneCords, gameObject, moveValue, range, isJump, isFly);
        //Debug.Log("attemteted to move");
        //pathfinder.findPathFromToRange(OneToOnePos, playerControler.playerOneToOneCords);
        //pathfinder.findPathFrom(OneToOnePos, playerControler.playerOneToOneCords);
        //pathfinder.MoveAlongPath(moveValue, gameObject, OneToOnePos);
    }


    public void MoveX(int moveValue, bool isJump = false)
    {
        //Debug.Log("AttemptToMove");
        //Debug.Log(transform.position + "self Pos, " + playerControler.gameObject.transform.position + "playerPos");
        distanceToPlayer = mapManager.GetDistanceTo(transform.position, playerControler.gameObject.transform.position);
        //Debug.Log(distanceToPlayer);
        relativeHexPosToPlayer = mapManager.GetDisanceInHexCordsTo(transform.position, playerControler.transform.position);
        for (int i = 0; i < moveValue; i++)
        {
            if (distanceToPlayer > 1)
            {
                //Debug.Log(distanceToPlayer + "distanceToPlayer");
                //Debug.Log(relativeHexPosToPlayer + "posToPlayer");
                if (Mathf.Abs(relativeHexPosToPlayer.x) > Mathf.Abs(relativeHexPosToPlayer.y) && Mathf.Abs(relativeHexPosToPlayer.x) > Mathf.Abs(relativeHexPosToPlayer.z))
                {
                    transform.position = mapManager.PosWithHexOffset(transform.position, -new Vector3(1, 0, 0) * Mathf.Sign(relativeHexPosToPlayer.x));
                    relativeHexPosToPlayer -= new Vector3(1, 0, 0) * Mathf.Sign(relativeHexPosToPlayer.x);
                    distanceToPlayer--;
                }
                else if (Mathf.Abs(relativeHexPosToPlayer.y) > Mathf.Abs(relativeHexPosToPlayer.z))
                {
                    transform.position = mapManager.PosWithHexOffset(transform.position, -new Vector3(0, 1, 0) * Mathf.Sign(relativeHexPosToPlayer.y));
                    relativeHexPosToPlayer -= new Vector3(0, 1, 0) * Mathf.Sign(relativeHexPosToPlayer.y);
                    distanceToPlayer--;
                }
                else
                {
                    transform.position = mapManager.PosWithHexOffset(transform.position, -new Vector3(0, 0, 1) * Mathf.Sign(relativeHexPosToPlayer.z));
                    relativeHexPosToPlayer -= new Vector3(0,0,1) * Mathf.Sign(relativeHexPosToPlayer.z);
                    distanceToPlayer--;
                }
            }
        }
    }

    public void AttackX(int attackValue, int attackRange = 1)
    {
        if (distanceToPlayer <= attackRange)
        {
            playerControler.AttackedForX(attackValue);
        }
    }

    public void AttackedForX(int attackValue)
    {
        health -= attackValue;
    }

    public void showHideTooltip(bool show)
    {

    }
}
