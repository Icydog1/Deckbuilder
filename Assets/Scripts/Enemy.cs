using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    protected Vector2 oneToOnePos;

    protected delegate void moveSetsMethod();
    protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected bool nextAction;

    protected List<System.Action> currentPlan = new List<System.Action>();
    protected moveSetsMethod plannedMoveSet;
    protected bool isPlanning;
    protected List<string> displayedPlan = new List<string>();

    protected int actionNum;
    protected int preferedRange;


    public int maxHealth, health;
    private bool canFly = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        playerControler = GameObject.Find("Player").GetComponent<PlayerControler>();
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        pathfinder = GameObject.Find("Pathfinder").GetComponent<Pathfinder>();
        enemyUI = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        mouseManager = GameObject.Find("MouseManager").GetComponent<MouseManager>();


        turnManager.turnOrder.Add(gameObject);
        health = maxHealth;
        TurnManager.RoundStarted += GetPlan;
        GetPlan(null);
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public void GetPlan(TurnManager turnManager)
    {
        isPlanning = true;
        preferedRange = int.MaxValue;
        currentPlan.Clear();
        displayedPlan.Clear();
        plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        plannedMoveSet();
        enemyUI.Plan(displayedPlan);
    }
    public void StartOfTurn()
    {
        isPlanning = false;
        if (preferedRange == int.MaxValue)
        {
            preferedRange = 1;
        }
        GameObject border = transform.Find("Border").gameObject;
        border.GetComponent<SpriteRenderer>().color = Color.white;
        CalculateValues();
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

    public void CalculateValues()
    {
        oneToOnePos = mapManager.PosToOneToOne(transform.position);
        distanceToPlayer = mapManager.GetDistanceBetweenOneToOne(oneToOnePos, playerControler.playerOneToOneCords);
    }
    public void ActionDone()
    {
        CalculateValues();
        nextAction = true;
    }
    public void Move(int moveValue, bool isJump = false)
    {
        if (isPlanning)
        {
            currentPlan.Add(() => Move(moveValue, isJump));
            string planString = "Move " + moveValue;
            if (isJump)
            {
                planString += " Jump";
            }
            displayedPlan.Add(planString);
        }
        else
        {
            StartCoroutine(pathfinder.PathfindTowards(oneToOnePos, playerControler.playerOneToOneCords, gameObject, moveValue, preferedRange, isJump, canFly));
        }
    }

    public void Attack(int attackValue, int attackRange = 1)
    {
        if (isPlanning)
        {
            if (preferedRange > attackRange)
            {
                preferedRange = attackRange;
            }
            currentPlan.Add(() => Attack(attackValue, attackRange));
            string planString = "Attack " + attackValue;
            if (attackRange > 1)
            {
                planString += " range " + attackRange;
            }
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
        if (health <= 0)
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
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
    }
}
