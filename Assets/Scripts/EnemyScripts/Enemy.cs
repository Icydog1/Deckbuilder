using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Figure
{

    //protected Vector3 relativeHexPosToPlayer;
    private string enemyName;
    protected delegate void moveSetsMethod();
    //protected List<moveSetsMethod> moveSets = new List<moveSetsMethod>();
    protected List<List<System.Action>> moveSets = new List<List<System.Action>>();
    protected List<int> movesSetOrder = new List<int>() { -1};
    protected int currentmove = 0;
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
        if (moveSets.Count == 0)
        {
            Debug.Log("Warning no planed movesets on " + gameObject);
        }
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
        if (currentmove == movesSetOrder.Count)
        {
            currentmove = 0;
        }
        if (movesSetOrder[currentmove] == -1)
        {
            plannedMoveSet = moveSets[Random.Range(0, moveSets.Count)];
        }
        else
        {
            //Debug.Log(movesSetOrder[currentmove]);
            plannedMoveSet = moveSets[movesSetOrder[currentmove]];
        }
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
        currentmove++;
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
