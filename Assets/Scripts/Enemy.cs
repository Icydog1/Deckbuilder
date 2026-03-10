using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;

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
    //protected string[] movesets;
    //protected List<string> actionQueue = new List<string>();
    protected delegate void moveSetsMethod();
    protected moveSetsMethod currentMove;
    protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected delegate void currentPlanMethods();
    protected currentPlanMethods currentPlan;
    protected bool nextAction;

    protected bool isPlanning;
    protected List<string> displayedPlan = new List<string>();




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
        TurnManager.RoundStarted += GetPlan;
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void GetPlan(TurnManager turnManager)
    {
        isPlanning = true;
        currentMove = moveSets[Random.Range(0, moveSets.Count)];

    }
    public void StartOfTurn()
    {
        isPlanning = false;
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.white;
        nextAction = true;
    }

    public void EndTurn()
    {
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.black;
        turnManager.NextTurn();
    }

    public IEnumerator TakeTurn()
    {
        currentMove = moveSets[Random.Range(0, moveSets.Count)];
        StartOfTurn();
        yield return new WaitUntil(() => nextAction == true);
        nextAction = false;
        for (int i = 0; i < moveSets.Count; i++)
        {
            moveSets[i]();
            yield return new WaitUntil(() => nextAction == true);
            nextAction = false;
        }
        EndTurn();
    }
    public void ActionDone()
    {
        nextAction = true;
    }
    public void Move(int moveValue,int range = 1, bool isJump = true, bool isFly = false)
    {
        OneToOnePos = mapManager.PosToOneToOne(transform.position);
        StartCoroutine(pathfinder.PathfindTowards(OneToOnePos, playerControler.playerOneToOneCords, gameObject, moveValue, range, isJump, isFly));
        if (isPlanning)
        {
            string planString = "Move " + moveValue;
            if (isJump)
            {
                planString += " Jump";
            }
            displayedPlan.Add(planString);
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
