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
    private MouseManager mouseManager;
    protected EnemyUi enemyUI;
    protected bool isMyTurn;
    protected float distanceToPlayer;
    protected Vector3 relativeHexPosToPlayer;
    protected Vector2 OneToOnePos;
    protected delegate void moveSetsMethod();
    protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected bool nextAction;

    protected List<System.Action> currentPlan = new List<System.Action>();
    protected moveSetsMethod plannedMoveSet;


    protected bool isPlanning;
    protected List<string> displayedPlan = new List<string>();

    public int maxHealth, health;
    private bool canFly = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        enemyUI = GameObject.Find("EnemyUI").GetComponent<EnemyUi>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();


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
        currentPlan.Clear();
        displayedPlan.Clear();
        plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        plannedMoveSet();

        enemyUI.Plan(displayedPlan);
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
        StartOfTurn();
        yield return new WaitUntil(() => nextAction == true);
        nextAction = false;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            currentPlan[i]();
            yield return new WaitUntil(() => nextAction == true);
            nextAction = false;
        }
        EndTurn();
    }
    public void ActionDone()
    {
        nextAction = true;
    }
    public void Move(int moveValue, int range = 1, bool isJump = false)
    {
        if (isPlanning)
        {
            currentPlan.Add(() => Move(moveValue, range, isJump));
            string planString = "Move " + moveValue;
            if (isJump)
            {
                planString += " Jump";
            }
            displayedPlan.Add(planString);
        }
        else
        {
            OneToOnePos = mapManager.PosToOneToOne(transform.position);
            StartCoroutine(pathfinder.PathfindTowards(OneToOnePos, playerControler.playerOneToOneCords, gameObject, moveValue, range, isJump, canFly));
        }
    }

    public void Attack(int attackValue, int attackRange = 1)
    {
        if (isPlanning)
        {
            currentPlan.Add(() => Attack(attackValue, attackRange));
            string planString = "Attack " + attackValue;
            displayedPlan.Add(planString);
        }
        else
        {
            if (distanceToPlayer <= attackRange)
            {
                playerControler.AttackedFor(attackValue);
            }
            ActionDone();
        }
    }

    public void AttackedFor(int attackValue)
    {
        health -= attackValue;
        enemyUI.SetHealth(health);
        if (health < 0)
        {
            Die();
        }
    }

    public void showHideTooltip(bool show)
    {

    }

    public void Die()
    {
        TurnManager.RoundStarted -= GetPlan;
        mouseManager.MouseOffObject(gameObject);
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
    }
}
