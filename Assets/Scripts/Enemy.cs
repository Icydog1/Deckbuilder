using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Figure
{

    //protected Vector3 relativeHexPosToPlayer;

    protected delegate void moveSetsMethod();
    protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected bool nextAction;

    protected List<System.Action> currentPlan = new List<System.Action>();
    protected moveSetsMethod plannedMoveSet;
    protected List<string> displayedPlan = new List<string>();

    protected int actionNum;
    protected EnemyUi enemyStatsDisplayer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        enemyStatsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        statsDisplayer = enemyStatsDisplayer;
        isEnemy = true;
        base.Start();

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
        PrepareActions = currentPlan;
        PlanDescription = displayedPlan;
        isPlanning = true;
        preferedRange = int.MaxValue;
        currentPlan.Clear();
        displayedPlan.Clear();
        plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        plannedMoveSet();
        enemyStatsDisplayer.Plan(displayedPlan);
    }
    public void StartOfTurn()
    {
        base.baseStartTurn();
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
    public override void ActionDone()
    {
        CalculateValues();
        nextAction = true;
    }
    /*
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

    
    public void Block(int blockValue)
    {
        if (isPlanning)
        {
            currentPlan.Add(() => Block(blockValue));
            string planString = "Block " + blockValue;
            displayedPlan.Add(planString);
        }
        else
        {

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
    */
    public void showHideTooltip(bool show)
    {

    }

    public override void Die()
    {
        TurnManager.RoundStarted -= GetPlan;
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
    }
}
