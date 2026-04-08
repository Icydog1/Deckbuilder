using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Figure
{

    //protected Vector3 relativeHexPosToPlayer;

    protected delegate void moveSetsMethod();
    //protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected List<List<System.Action>> moveSets = new List<List<System.Action>>();


    protected List<System.Action> currentPlan = new List<System.Action>();
    protected List<System.Action> plannedMoveSet;
    protected List<string> displayedPlan = new List<string>();

    protected int actionNum;
    protected EnemyUi enemyStatsDisplayer;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Awake()
    {
        LevelManager.LevelCleared += Remove;
        base.Awake();
    }
    public override void Start()
    {
        enemyStatsDisplayer = transform.Find("EnemyUI").GetComponent<EnemyUi>();
        statsDisplayer = enemyStatsDisplayer;
        isEnemy = true;
        base.Start();

        team = 1;
        turnManager.turnOrder.Add(gameObject);
        health = maxHealth;
        TurnManager.RoundStarted += GetNewPlan;
        GetNewPlan(null);
    }

    public void GetNewPlan(TurnManager turnManager)
    {
        PlanDescription = displayedPlan;
        currentPlan.Clear();
        displayedPlan.Clear();
        plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        currentPlan = new List<System.Action>(plannedMoveSet);
        //Debug.Log("gotInitialPlan");
        UpdatePlan();
    }

    public void UpdatePlan()
    {
        preferedRange = int.MaxValue;
        isPlanning = true;
        for (int i = 0; i < currentPlan.Count; i++)
        {
            currentPlan[i]();
        }
        enemyStatsDisplayer.Plan(displayedPlan);
        isPlanning = false;

    }
    public void StartOfTurn()
    {
        base.baseStartTurn();
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
        base.baseEndTurn();
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
    public void showHideTooltip(bool show)
    {

    }

    public override void Die()
    {
        TurnManager.RoundStarted -= GetNewPlan;
        LevelManager.LevelCleared -= Remove;
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
    }

    public override void Remove(LevelManager levelManager = null)
    {
        TurnManager.RoundStarted -= GetNewPlan;
        LevelManager.LevelCleared -= Remove;
        turnManager.RemoveFromTurnOrder(gameObject);
        Destroy(gameObject);
    }
}
